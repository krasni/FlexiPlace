using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using FlexiPlace.Extensions;
using FlexiPlace.Models;
using FlexiPlace.Models.DB;
using FlexiPlace.Services;
using FlexiPlace.Utilities;
using FlexiPlace.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace FlexiPlace.Controllers
{
    public class AdministracijaController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly ILogger<AdministracijaController> logger;
        private readonly IMailer mailer;
        private readonly IMemoryCache cache;
        private readonly IConfiguration configuration;

        public AdministracijaController(AppDbContext dbContext,
                                 ILogger<AdministracijaController> logger,
                                 IMailer mailer,
                                 IMemoryCache cache,
                                 IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mailer = mailer;
            this.cache = cache;
            this.configuration = configuration;
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpGet]
        public ActionResult ParametriAplikacije()
        {
            Parametar parametar = dbContext.Parametar.FirstOrDefault();

            // provjeri ovdje da li je job već kreiran !!!!

            ParametarViewModel parametarViewModel = new ParametarViewModel
            {
                Id = parametar.Id,
                DozvoljeniBrojDanaMjesec = parametar.DozvoljeniBrojDanaMjesec,
                DozvoljeniBrojDanaTjedan = parametar.DozvoljeniBrojDanaTjedan,
                DozvoljeniBrojaDanaOdobrenje = parametar.DozvoljeniBrojaDanaOdobrenje,
            };

            return View(parametarViewModel);
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpPost]
        public IActionResult ParametriAplikacije(ParametarViewModel model)
        {
            if (ModelState.IsValid)
            {
                Parametar parametar = dbContext.Parametar.FirstOrDefault();
                parametar.DozvoljeniBrojDanaMjesec = model.DozvoljeniBrojDanaMjesec;
                parametar.DozvoljeniBrojDanaTjedan = model.DozvoljeniBrojDanaTjedan;
                parametar.DozvoljeniBrojaDanaOdobrenje = model.DozvoljeniBrojaDanaOdobrenje;

                dbContext.Parametar.Update(parametar);

                logger.LogInformation($"Korisnik {User.Identity.Name} je promijenio parametere aplikacije.");

                dbContext.SaveChanges();
            }

            return View();
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpGet]
        public ActionResult ImportNeradniDan()
        {
            NeradniDanExcel neradniDanExcel = new NeradniDanExcel();
            return View(neradniDanExcel);
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpPost]
        public async Task<NeradniDaniResponse<List<Utilities.NeradniDan>>> ImportNeradniDan(IFormFile formFile, CancellationToken cancellationToken)
        {
            try
            {
                if (formFile == null || formFile.Length <= 0)
                {
                    return NeradniDaniResponse<List<Utilities.NeradniDan>>.GetResult(-1, "Datoteka je prazna.");
                }

                if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    return NeradniDaniResponse<List<Utilities.NeradniDan>>.GetResult(-1, "Extenzija mora biti xlsx.");
                }

                var list = new List<Utilities.NeradniDan>();

                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream, cancellationToken);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            if (worksheet.Cells[row, 1].Value != null)
                            {
                                list.Add(new Utilities.NeradniDan
                                {
                                    DatumNeradniDan = worksheet.Cells[row, 1].Value.ToString().Trim()
                                });
                            }
                        }
                    }
                }

                string message = string.Empty;
                List<NeradniDanHanfa> neradniDaniHanfa = new List<NeradniDanHanfa>();

                foreach (var neradniDan in list)
                {
                    if (DateTime.TryParse(neradniDan.DatumNeradniDan, out DateTime dateTime))
                    {
                        neradniDaniHanfa.Add(new NeradniDanHanfa() { NeradniDan = dateTime });
                    }
                    else
                    {
                        message += "Neispravan datum:" + neradniDan.DatumNeradniDan + ", ";
                    }
                }

                if (string.IsNullOrEmpty(message))
                {
                    dbContext.NeradniDanHanfa.Clear();
                    dbContext.AddRange(neradniDaniHanfa);

                    dbContext.SaveChanges();

                    logger.LogInformation($"Korisnik {User.Identity.Name} je importirao praznike.");

                    return NeradniDaniResponse<List<Utilities.NeradniDan>>.GetResult(0, "OK");
                }
                else
                {
                    return NeradniDaniResponse<List<Utilities.NeradniDan>>.GetResult(-1, message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return NeradniDaniResponse<List<Utilities.NeradniDan>>.GetResult(-1, "Dogodila se greška prilikom importa.");
            }
        }

        public static string GetString(IHtmlContent content)
        {
            using (var writer = new System.IO.StringWriter())
            {
                content.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpGet]
        public ActionResult ImportNeradnoMjesto()
        {
            // dovuci iz bp2 radna mjesta            
            var radnaMjestaBP2 = dbContext.SpGetRadnaMjesta();

            // dovuci iz Flexi baze neradna mjesta
            var neradnaMjestaFlexi = dbContext.NeradnoMjesto.ToList();

            // iteriraj po bp2 radna mjesta i dodavaj new viewmodel i selected ako je selektirano u flexi
            var model = new List<RadnaMjestaHanfaViewModel>();

            foreach (var radnoMjestoBP2 in radnaMjestaBP2)
            {
                var radnoMjestoHanfaViewModel = new RadnaMjestaHanfaViewModel()
                {
                    RadnoMjestoNaziv = radnoMjestoBP2.name
                };

                // provjeri da li postoji u flexi place i stavi selected = true
                if (neradnaMjestaFlexi.Any(m => m.Naziv == radnoMjestoBP2.name))
                {
                    radnoMjestoHanfaViewModel.IsSelected = true;
                }

                model.Add(radnoMjestoHanfaViewModel);
            }

            return View(model);
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpPost]
        public ActionResult ImportNeradnoMjesto(List<RadnaMjestaHanfaViewModel> model)
        {
            // dohvati označena mjesta
            //dbContext.NeradniDanHanfa.Clear();
            //dbContext.AddRange(neradniDaniHanfa);

            var neradnaMjesta = new List<NeradnoMjesto>();

            foreach(var radnoMjesto in model)
            {
                if (radnoMjesto.IsSelected)
                {
                    neradnaMjesta.Add(new NeradnoMjesto { Naziv = radnoMjesto.RadnoMjestoNaziv });
                }
            }

            //pobriši iz baze sve i dodaj opet označene
            dbContext.NeradnoMjesto.Clear();
            dbContext.AddRange(neradnaMjesta);
            dbContext.SaveChanges();

            return RedirectToAction("Index","Zahtjev");
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpGet]
        public ActionResult CreateZahtjev(string returnUrl)
        {
            var zahtjevCreateAdminViewModel = new ZahtjevCreateAdminViewModel
            {
                UserName = "",
                Komentar = "",
                DatumOdsustva = DateTime.Now,
                VrijemeOdsustvaOd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0),
                VrijemeOdsustvaDo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 0, 0),
                Djelatnici = GetDjelatnici(),
            };

            return View(zahtjevCreateAdminViewModel);
        }

        [Authorize(Policy = "IsAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreateZahtjev(ZahtjevCreateAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                // dohvati podatke o UserName, odobravatelj, org jedinica itd.
                var userDetails = dbContext.GetUserDataBP2(model.UserName);

                Zahtjev newZahtjev = new Zahtjev()
                {
                    Podnositelj = userDetails[0].UserName,
                    PodnositeljEmail = userDetails[0].Email,
                    ImePrezimePodnositelj = userDetails[0].ImePrezime,
                    OrganizacijskaJedinicaPodnositelj = userDetails[0].NazivOrganizacijskeJedinice,
                    OrganizacijskaJedinicaPutanjaPodnositelj = userDetails[0].Putanja,
                    DatumOtvaranja = DateTime.Now,
                    DatumOdsustva = model.DatumOdsustva,
                    VrijemeOdsustvaOd = model.VrijemeOdsustvaOd,
                    VrijemeOdsustvaDo = model.VrijemeOdsustvaOd.AddHours(8),
                    Komentar = model.Komentar,
                    StatusId = GetStatusi().FirstOrDefault(s => s.Naziv == "Za odobravanje").Id,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity.Name,

                    OdobravateljImePrezime = userDetails[0].OdobravateljImePrezime,
                    OdobravateljNazivOrganizacijskeJedinice = userDetails[0].OdobravateljNazivOrganizacijskeJedinice,
                    OdobravateljADName = userDetails[0].OdobravateljADName,
                    OdobravateljPutanja = userDetails[0].OdobravateljPutanja,
                    OdobravateljEmail = userDetails[0].OdobravateljEmail
                };

                dbContext.Zahtjev.Add(newZahtjev);
                dbContext.SaveChanges();

                string returnUrl = this.Url.Action("Index", "Zahtjev");
                string relativeDetailUrl = this.Url.Action("Details", "Zahtjev", new { returnUrl, id = newZahtjev.Id });
                string fullDetailUrl = $"{this.Request.Scheme}://{this.Request.Host.Value}{relativeDetailUrl}";

                HtmlContentBuilder htmlBuilder = new HtmlContentBuilder();
                htmlBuilder.AppendFormat("<a href={0}>ID: {1} </a>", fullDetailUrl, newZahtjev.Id);
                htmlBuilder.AppendFormat("<h3>Podnositelj: {0} </h3>", newZahtjev.ImePrezimePodnositelj);
                htmlBuilder.AppendFormat("<h4>Organizacijska jedinica: {0} </h4>", newZahtjev.OrganizacijskaJedinicaPodnositelj);
                htmlBuilder.AppendFormat("<h4>Odobravatelj: {0} </h4>", newZahtjev.OdobravateljImePrezime);
                htmlBuilder.AppendFormat("<h4>Datum i vrijeme otvaranja: {0} </h4>", newZahtjev.DatumOtvaranja.ToString("dd.MM.yyyy HH:mm"));
                htmlBuilder.AppendFormat("<h4>Flexi datum: {0} </h4>", newZahtjev.DatumOdsustva.ToString("dd.MM.yyyy"));
                htmlBuilder.AppendFormat("<h4>Flexi vrijeme od: {0} </h4>", newZahtjev.VrijemeOdsustvaOd.ToString("HH:mm"));
                htmlBuilder.AppendFormat("<h4>Flexi vrijeme do: {0} </h4>", newZahtjev.VrijemeOdsustvaDo.ToString("HH:mm"));
                htmlBuilder.AppendFormat("<h4>Komentar: {0} </h4>", newZahtjev.Komentar);
                htmlBuilder.AppendFormat("<h4>Status: {0} </h4>", GetStatusi().FirstOrDefault(s => s.Id == newZahtjev.StatusId).Naziv);

                var outputHtml = GetString(htmlBuilder);

                // pošalji mail podnositelju i odobravatelju i adminu koji je kreirao zapis, sa html-om cijelog zahjeva i linkom na details !!!
                await mailer.SendEmailAsync(userDetails[0].OdobravateljEmail, "Flexiplace - Kreiran je novi zahtjev", outputHtml);
                await mailer.SendEmailAsync(userDetails[0].Email, "Flexiplace - Kreiran je novi zahtjev", outputHtml);
                await mailer.SendEmailAsync(User.FindFirstValue("Email"), "Flexiplace - Kreiran je novi zahtjev", outputHtml);

                return RedirectToAction("Index", "Zahtjev");
            }

            return View(model);
        }

        [Produces("application/json")]
        [HttpGet]
       public IActionResult PretraziDjelatnike()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var names = GetDjelatnici().Where(d => d.ImePrezime.Contains(term, StringComparison.OrdinalIgnoreCase)).Select(d => d.ImePrezime).ToList();
                return Ok(names);
            }
            catch
            {
                return BadRequest();
            }
        }

        public List<SpGetDjelatnici> GetDjelatnici()
        {
            if (!cache.TryGetValue("Djelatnici", out List<SpGetDjelatnici> djelatnici))
            {
                var refreshPermissionsPeriodInSeconds = int.Parse(configuration.GetSection("RefreshCodeBooksPeriodInSeconds").Value);

                djelatnici = dbContext.SpGetDjelatnici();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(refreshPermissionsPeriodInSeconds));

                cache.Set("Djelatnici", djelatnici, cacheEntryOptions);
            }

            return djelatnici;
        }

        [HttpPost]
        public JsonResult ComputeVrijemeOdsustvaDo(string VrijemeOdsustvaOd)
        {
            DateTime vrijemeOdsustvaOd;
            try
            {
                vrijemeOdsustvaOd = DateTime.ParseExact(VrijemeOdsustvaOd, "HH:mm", CultureInfo.InvariantCulture);
            }
            catch
            {
                return Json(new { VrijemeOdsustvaDo = "09:00", Poruka = "Neispravno" });
            }

            // da li je vrijeme između 07:00 i 09:00
            var danas = DateTime.Now;
            var startDate = danas.Date.Add(new TimeSpan(07, 00, 0));

            var endDate = danas.Date.Add(new TimeSpan(09, 00, 0));

            if (vrijemeOdsustvaOd > endDate || vrijemeOdsustvaOd < startDate)
            {
                return Json(new { VrijemeOdsustvaDo = "09:00", Poruka = "Neispravno" });
            }

            // dodaj 2 sata na VrijemeOdsustvaDo i vrati time porciju
            vrijemeOdsustvaOd = vrijemeOdsustvaOd.AddHours(8);

            return Json(new { VrijemeOdsustvaDo = vrijemeOdsustvaOd.ToString("HH:mm"), Poruka = "" });
        }

        public JsonResult IsVrijemeOdsustvaOdValid(string VrijemeOdsustvaOd)
        {
            DateTime vrijemeOdsustvaOd;
            try
            {
                vrijemeOdsustvaOd = DateTime.ParseExact(VrijemeOdsustvaOd, "HH:mm", CultureInfo.InvariantCulture);
            }
            catch
            {
                return Json("Flexi vrijeme od je neispravno", new Newtonsoft.Json.JsonSerializerSettings());
            }

            // da li je vrijeme između 07:00 i 09:00
            var danas = DateTime.Now;
            var startDate = danas.Date.Add(new TimeSpan(07, 00, 0));

            var endDate = danas.Date.Add(new TimeSpan(09, 00, 0));

            if (vrijemeOdsustvaOd > endDate || vrijemeOdsustvaOd < startDate)
            {
                return Json("Flexi vrijeme mora biti između 07:00 i 09:00", new Newtonsoft.Json.JsonSerializerSettings());
            }

            return Json(true, new Newtonsoft.Json.JsonSerializerSettings());
        }

        public JsonResult IsFlexiDatumValid(string DatumOdsustva, string UserName)
        {
            DateTime datumOdsustva;
            try
            {
                datumOdsustva = DateTime.ParseExact(DatumOdsustva, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                return Json("Flexi datum je neispravan", new Newtonsoft.Json.JsonSerializerSettings());
            }

            if (datumOdsustva < DateTime.Now.Date)
            {
                return Json("Flexi datum je u prošlosti", new Newtonsoft.Json.JsonSerializerSettings());
            }

            if (datumOdsustva.DayOfWeek == DayOfWeek.Friday)
            {
                return Json("Flexi datum je petak", new Newtonsoft.Json.JsonSerializerSettings());
            }

            if (datumOdsustva.DayOfWeek == DayOfWeek.Monday)
            {
                return Json("Flexi datum je ponedjeljak", new Newtonsoft.Json.JsonSerializerSettings());
            }

            if (datumOdsustva.DayOfWeek == DayOfWeek.Sunday)
            {
                return Json("Flexi datum je nedjelja", new Newtonsoft.Json.JsonSerializerSettings());
            }

            if (datumOdsustva.DayOfWeek == DayOfWeek.Saturday)
            {
                return Json("Flexi datum je subota", new Newtonsoft.Json.JsonSerializerSettings());
            }

            var zahtjevPostojiZaTajDatum = dbContext.Zahtjev.Where(z => z.Podnositelj == UserName)
                                            .Any(z => z.DatumOdsustva == datumOdsustva);

            if (zahtjevPostojiZaTajDatum)
            {
                return Json("Već postoji zahtjev za taj datum", new Newtonsoft.Json.JsonSerializerSettings());
            }

            if (dbContext.NeradniDanHanfa.Any(d => d.NeradniDan == datumOdsustva))
            {
                return Json("Zadani datum zahtjeva je neradni dan", new Newtonsoft.Json.JsonSerializerSettings());
            }

            var dozvoljeniBrojDanaTjedan = dbContext.Parametar.FirstOrDefault().DozvoljeniBrojDanaTjedan;

            var prviDanUTjednu = datumOdsustva.FirstDayOfWeek();
            var zadnjiDanUTjednu = datumOdsustva.LastDayOfWeek().AddDays(-2);

            var brojZahtjevaZaTjedanDatumOdsustva = dbContext.Zahtjev.Where(z => z.Podnositelj == UserName)
                     .Where(z => z.DatumOdsustva >= prviDanUTjednu && z.DatumOdsustva <= zadnjiDanUTjednu).Count();

            if (brojZahtjevaZaTjedanDatumOdsustva >= dozvoljeniBrojDanaTjedan)
            {
                return Json($"Za razdoblje {prviDanUTjednu:dd.MM.yyyy} - {zadnjiDanUTjednu:dd.MM.yyyy} već postoji jedan zahtjev", new Newtonsoft.Json.JsonSerializerSettings());
            }

            return Json(true, new Newtonsoft.Json.JsonSerializerSettings());
        }

        public List<Status> GetStatusi()
        {
            if (!cache.TryGetValue("Statusi", out List<Status> statusi))
            {
                var refreshPermissionsPeriodInSeconds = int.Parse(configuration.GetSection("RefreshCodeBooksPeriodInSeconds").Value);

                statusi = dbContext.Status.ToList();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(refreshPermissionsPeriodInSeconds));

                cache.Set("Statusi", statusi, cacheEntryOptions);
            }

            return statusi;
        }
    }
}
