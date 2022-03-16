using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FlexiPlace.Models;
using Microsoft.Extensions.Logging;
using FlexiPlace.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using FlexiPlace.Utilities;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using FlexiPlace.Extensions;
using Microsoft.AspNetCore.Authorization;
using FlexiPlace.Models.DB;
using System.Data.SqlClient;
using System.Security.Claims;
using FlexiPlace.Services;
using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;
using FlexiPlace.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using MailKit;
using Microsoft.Extensions.Configuration;
using FlexiPlace.Security;
using System.Globalization;

namespace FlexiPlace.Controllers
{
    public class ZahtjevController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly ILogger<ZahtjevController> logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IMailer mailer;
        private readonly IMemoryCache cache;
        private readonly IConfiguration configuration;
        public int PageSize = 20;

        public ZahtjevController(AppDbContext dbContext,
                                 ILogger<ZahtjevController> logger,
                                 IAuthorizationService authorizationService,
                                 IMailer mailer,
                                 IMemoryCache cache,
                                 IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.authorizationService = authorizationService;
            this.mailer = mailer;
            this.cache = cache;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index([ModelBinder(typeof(DateTimeModelBinder))] DateTime? DatumOtvaranjaOd, [ModelBinder(typeof(DateTimeModelBinder))] DateTime? DatumOtvaranjaDo, [ModelBinder(typeof(DateTimeModelBinder))] DateTime? DatumOdsustvaOd, [ModelBinder(typeof(DateTimeModelBinder))] DateTime? DatumOdsustvaDo, string Podnositelj, string Odobravatelj, string StatusNaziv = "Za odobravanje", string PodnositeljOrganizacijskaJedinicaNaziv = "Sve", string OdobravateljOrganizacijskaJedinicaNaziv = "Sve", int ZahtjevStrana = 1)
        {
            if (DatumOtvaranjaDo != null)
            {
                DatumOtvaranjaDo = DatumOtvaranjaDo.Value.AddHours(23.99);
            }
            if (DatumOdsustvaDo != null)
            {
                DatumOdsustvaDo = DatumOdsustvaDo.Value.AddHours(23.99);
            }

            var zahtjevi = dbContext.Zahtjev.Include(z => z.Status)

                            .Where(z => StatusNaziv == "Svi" || z.Status.Naziv == StatusNaziv)

                            .Where(z => DatumOtvaranjaOd == null || z.DatumOtvaranja >= DatumOtvaranjaOd)

                            .Where(z => DatumOtvaranjaDo == null || z.DatumOtvaranja <= DatumOtvaranjaDo)

                            .Where(z => DatumOdsustvaOd == null || z.DatumOdsustva >= DatumOdsustvaOd)

                            .Where(z => DatumOdsustvaDo == null || z.DatumOdsustva <= DatumOdsustvaDo)

                            .Where(z => PodnositeljOrganizacijskaJedinicaNaziv == "Sve" || z.OrganizacijskaJedinicaPodnositelj == PodnositeljOrganizacijskaJedinicaNaziv)

                            .Where(z => OdobravateljOrganizacijskaJedinicaNaziv == "Sve" || z.OrganizacijskaJedinicaPodnositelj == OdobravateljOrganizacijskaJedinicaNaziv)

                            .Where(z => Podnositelj == null || z.ImePrezimePodnositelj.Contains(Podnositelj))

                            .Where(z => Odobravatelj == null || z.OdobravateljImePrezime.Contains(Odobravatelj))

                              .Where(z => (User.FindFirstValue("Uloga") == "Voditelj" && z.OrganizacijskaJedinicaPutanjaPodnositelj.StartsWith(User.FindFirstValue("Putanja"))) ||

                             (User.FindFirstValue("Uloga") == "Korisnik" && z.Podnositelj == User.FindFirstValue("Username")) ||

                              (!string.IsNullOrEmpty(User.FindFirstValue("IsAdmin"))));

            var model = new ZahtjevListViewModel
            {
                Zahtjevi = zahtjevi
                            .OrderByDescending(z => z.DatumOtvaranja)
                            .ThenBy(z => z.ImePrezimePodnositelj)
                            .Skip((ZahtjevStrana - 1) * PageSize)
                            .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = ZahtjevStrana,
                    ItemsPerPage = PageSize,
                    TotalItems = zahtjevi.Count()
                },

                StatusNaziv = StatusNaziv,

                Podnositelj = Podnositelj,
                Odobravatelj = Odobravatelj,

                PodnositeljOrganizacijskaJedinicaNaziv = PodnositeljOrganizacijskaJedinicaNaziv,
                OdobravateljOrganizacijskaJedinicaNaziv = OdobravateljOrganizacijskaJedinicaNaziv,

                Statusi = GetStatusi(),
                PodnositeljOrganizacijskeJedinice = GetOrganizacijskeJedinice(),
                OdobravateljOrganizacijskeJedinice = GetOrganizacijskeJedinice(),

                DatumOtvaranjaOd = DatumOtvaranjaOd,
                DatumOtvaranjaDo = DatumOtvaranjaDo,
                DatumOdsustvaOd = DatumOdsustvaOd,
                DatumOdsustvaDo = DatumOdsustvaDo
            };

            return View(model);
        }

        [HttpGet]
        public ViewResult Create(string returnUrl)
        {
            // dohvati kvotu radnih dana, da li je prećerao?
            var dozvoljeniBrojDanaZahtjev = dbContext.Parametar.FirstOrDefault().DozvoljeniBrojDanaMjesec;

            var prviDanUMjesecu = DateTime.Now.FirstDayOfMonth();
            var zadnjiDanUMjesecu = DateTime.Now.LastDayOfMonth();

            // dohvati za ovog korisnika ukupni broj odobrenih zahtjeva u trenutnom mjesecu
            var brojMjesecnihOdobrenihZahtjevaTrenutnogKorisnika = dbContext.Zahtjev.Where(z => z.Podnositelj == User.FindFirstValue("Username"))
                  .Where(z => z.DatumOtvaranja >= prviDanUMjesecu && z.DatumOtvaranja <= zadnjiDanUMjesecu)
                  .Where(z => z.DatumOdsustva >= prviDanUMjesecu && z.DatumOdsustva <= zadnjiDanUMjesecu)
                  .Where(z => z.StatusId == GetStatusi().FirstOrDefault(s => s.Naziv == "Odobren").Id).Count(); ;

            bool prekoracenBrojOdobrenihZahtjevaZaTrenutniMjesec = dozvoljeniBrojDanaZahtjev <= brojMjesecnihOdobrenihZahtjevaTrenutnogKorisnika;

            // nađi da li se za ovo radno mjesto može podnjeti zahtjev
            bool nedozvoljenoRadnoMjesto = dbContext.NeradnoMjesto.Any(nm => nm.Naziv == User.FindFirstValue("UlogaHanfa"));

            var zahtjevCreateViewModel = new ZahtjevCreateViewModel()
            {
                ImePrezimePodnositelj = User.FindFirstValue("ImePrezime"),
                OdobravateljImePrezime = User.FindFirstValue("OdobravateljImePrezime"),
                Komentar = "",
                DatumOdsustva = DateTime.Now,
                VrijemeOdsustvaOd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0),
                VrijemeOdsustvaDo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 0, 0),
                ReturnUrl = returnUrl,
                OrganizacijskaJedinicaPodnositelj = User.FindFirstValue("NazivOrganizacijskeJedinice"),
            };

            if (prekoracenBrojOdobrenihZahtjevaZaTrenutniMjesec)
            {
                ModelState.AddModelError(string.Empty, "Broj dozvoljenih zahtjeva za mjesec je prekoračen.");
            }

            if (nedozvoljenoRadnoMjesto)
            {
                ModelState.AddModelError(string.Empty, "Nije dozvoljen zahtjev za ovo radno mjesto");
            }

            return View(zahtjevCreateViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ZahtjevCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Zahtjev newZahtjev = new Zahtjev()
                {
                    Podnositelj = User.Identity.Name,
                    ImePrezimePodnositelj = User.FindFirstValue("ImePrezime"),
                    OrganizacijskaJedinicaPodnositelj = User.FindFirstValue("NazivOrganizacijskeJedinice"),
                    OrganizacijskaJedinicaPutanjaPodnositelj = User.FindFirstValue("Putanja"),
                    OdobravateljImePrezime = User.FindFirstValue("OdobravateljImePrezime"),
                    DatumOtvaranja = DateTime.Now,
                    DatumOdsustva = model.DatumOdsustva,
                    VrijemeOdsustvaOd = model.VrijemeOdsustvaOd,
                    VrijemeOdsustvaDo = model.VrijemeOdsustvaOd.AddHours(8),
                    Komentar = model.Komentar,
                    StatusId = GetStatusi().FirstOrDefault(s => s.Naziv == "Za odobravanje").Id,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity.Name,

                    OdobravateljNazivOrganizacijskeJedinice = User.FindFirstValue("OdobravateljNazivOrganizacijskeJedinice"),

                    OdobravateljADName = User.FindFirstValue("OdobravateljADName"),

                    OdobravateljPutanja = User.FindFirstValue("OdobravateljPutanja"),

                    PodnositeljEmail = User.FindFirstValue("Email"),

                    OdobravateljEmail = User.FindFirstValue("OdobravateljEmail")
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

                // pošalji mail podnositelju i odobravatelju, sa html-om cijelog zahjeva i linkom na details !!!
                await mailer.SendEmailAsync(User.FindFirstValue("Email"), "Flexiplace - Kreiran je novi zahtjev", outputHtml);
                await mailer.SendEmailAsync(User.FindFirstValue("OdobravateljEmail"), "Flexiplace - Kreiran je novi zahtjev", outputHtml);

                if (Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string returnUrl, int id)
        {
            Zahtjev zahtjev = dbContext.Zahtjev.Include(z => z.Status).FirstOrDefault(z => z.Id == id);

            if (zahtjev == null)
            {
                Response.StatusCode = 404;
                logger.LogWarning($"User: {User.Identity.Name}, Zahtjev nije nađen, Id = {id} ");
                return View("ZahtjevNotFound", id);
            }

            if (!(await authorizationService.AuthorizeAsync(User, zahtjev, "CanEditZahtjev")).Succeeded)
            {
                return new ForbidResult();
            }

            var statusi = User.FindFirstValue("Uloga") == "Admin" ? GetStatusi() : GetStatusi().Where(s => s.Naziv == "Odobren" || s.Naziv == "Odbijen" || s.Naziv == "Za odobravanje");

            //var statusi = GetStatusi();

            ZahtjevEditViewModel zahtjevEditViewModel = new ZahtjevEditViewModel
            {
                Id = zahtjev.Id,
                ImePrezimePodnositelj = zahtjev.ImePrezimePodnositelj,
                OrganizacijskaJedinicaPodnositelj = zahtjev.OrganizacijskaJedinicaPodnositelj,
                OdobravateljImePrezime = zahtjev.OdobravateljImePrezime,
                DatumOtvaranja = zahtjev.DatumOtvaranja,
                DatumOdsustva = zahtjev.DatumOdsustva,
                Komentar = zahtjev.Komentar,
                RazlogOdbijanja = zahtjev.RazlogOdbijanja,
                StatusID = zahtjev.StatusId,
                StatusNaziv = zahtjev.Status.Naziv,
                ReturnUrl = returnUrl,
                Statusi = statusi.Select(s =>
                                  new SelectListItem()
                                  {
                                      Value = s.Id.ToString(),
                                      Text = s.Naziv
                                  }),
            };

            return View(zahtjevEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ZahtjevEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Zahtjev zahtjev = dbContext.Zahtjev.Include(z => z.Status).FirstOrDefault(z => z.Id == model.Id);

                if (!(await authorizationService.AuthorizeAsync(User, zahtjev, "CanEditZahtjev")).Succeeded)
                {
                    return new ForbidResult();
                }

                var stariOdobravateljEmail = zahtjev.OdobravateljEmail;

                //zapamti stare i nove vrijednosti zbog mailanja razlika
                var stariStatus = zahtjev.Status.Naziv;
                var noviStatus = model.StatusNaziv;

                var stariRazlogOdbijanja = zahtjev.RazlogOdbijanja;
                var noviRazlogOdbijanja = model.RazlogOdbijanja;

                var stariOdobravateljImePrezime = zahtjev.OdobravateljImePrezime;
                var noviOdobravateljImePrezime = User.FindFirstValue("ImePrezime");

                zahtjev.RazlogOdbijanja = model.RazlogOdbijanja;
                zahtjev.StatusId = model.StatusID;
                zahtjev.ModifedBy = User.Identity.Name;
                zahtjev.ModifiedDate = DateTime.Now;

                zahtjev.OdobravateljADName = User.FindFirstValue("Username");
                zahtjev.OdobravateljImePrezime = User.FindFirstValue("ImePrezime");
                zahtjev.OdobravateljNazivOrganizacijskeJedinice = User.FindFirstValue("OdobravateljNazivOrganizacijskeJedinice");
                zahtjev.OdobravateljPutanja = User.FindFirstValue("Putanja");
                zahtjev.OdobravateljEmail = User.FindFirstValue("Email");

                dbContext.Zahtjev.Update(zahtjev);
                dbContext.SaveChanges();

                // ako je promijenjen status ili razlog odbijanja ili odobravatelj, onda pošalji mail podnositelju i odobravatelju !!!

                string returnUrl = this.Url.Action("Index", "Zahtjev");
                string relativeDetailUrl = this.Url.Action("Details", "Zahtjev", new { returnUrl, id = zahtjev.Id });
                string fullDetailUrl = $"{this.Request.Scheme}://{this.Request.Host.Value}{relativeDetailUrl}";

                HtmlContentBuilder htmlBuilder = new HtmlContentBuilder();

                htmlBuilder.AppendFormat($"<h4>Vrijeme izmjene: { zahtjev.ModifiedDate:dd.MM.yyyy HH:mm} </h4>");

                if (stariStatus != noviStatus)
                {
                    htmlBuilder.AppendFormat($"<h4>Promijenjen je status iz <b>{stariStatus}</b> u <b>{noviStatus}</b> </h4>");
                }

                if (stariRazlogOdbijanja != noviRazlogOdbijanja)
                {
                    htmlBuilder.AppendFormat($"<h4>Promijenjen je razlog odbijanja iz <b>{stariRazlogOdbijanja}</b> u <b>{noviRazlogOdbijanja}</b> </h4>");
                }

                if (stariOdobravateljImePrezime != noviOdobravateljImePrezime)
                {
                    htmlBuilder.AppendFormat($"<h4>Promijenjen je odobravatelj iz <b>{stariOdobravateljImePrezime}</b> u <b>{noviOdobravateljImePrezime}</b> </h4>");
                }

                htmlBuilder.AppendFormat("<a href={0}>ID: {1} </a>", fullDetailUrl, zahtjev.Id);
                htmlBuilder.AppendFormat("<h3>Podnositelj: {0} </h3>", zahtjev.ImePrezimePodnositelj);
                htmlBuilder.AppendFormat("<h4>Organizacijska jedinica: {0} </h4>", zahtjev.OrganizacijskaJedinicaPodnositelj);
                htmlBuilder.AppendFormat("<h4>Odobravatelj: {0} </h4>", zahtjev.OdobravateljImePrezime);
                htmlBuilder.AppendFormat("<h4>Datum i vrijeme otvaranja: {0} </h4>", zahtjev.DatumOtvaranja.ToString("dd.MM.yyyy HH:mm"));
                htmlBuilder.AppendFormat("<h4>Flexi datum: {0} </h4>", zahtjev.DatumOdsustva.ToString("dd.MM.yyyy"));
                htmlBuilder.AppendFormat("<h4>Flexi vrijeme od: {0} </h4>", zahtjev.VrijemeOdsustvaOd.ToString("HH:mm"));
                htmlBuilder.AppendFormat("<h4>Flexi vrijeme do: {0} </h4>", zahtjev.VrijemeOdsustvaDo.ToString("HH:mm"));
                htmlBuilder.AppendFormat("<h4>Komentar: {0} </h4>", zahtjev.Komentar);
                htmlBuilder.AppendFormat("<h4>Status: {0} </h4>", model.StatusNaziv);
                htmlBuilder.AppendFormat("<h4>Razlog odbijanja: {0} </h4>", model.RazlogOdbijanja);

                var outputHtml = GetString(htmlBuilder);

                // pošalji mail podnositelju i odobravatelju, sa html-om cijelog zahjeva i linkom na details !!!
                // pošalji i ako je netko preuzeo zahtjev, netko tko nije odobravatelj
                // samo treba paziti da se ne pošalje 2 ista maila odobravatelju
                await mailer.SendEmailAsync(zahtjev.PodnositeljEmail, $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);
                await mailer.SendEmailAsync(User.FindFirstValue("Email"), $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);

                // šalji i originalnom odobravatelju ako nije isti kao trenutni user koji je editirao zahtjev
                if (User.FindFirstValue("Email") != stariOdobravateljEmail)
                {
                    await mailer.SendEmailAsync(stariOdobravateljEmail, $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);
                }

                if (Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
            }

            // napuni Statuse jer su null nakon posta

            var statusi = User.FindFirstValue("Uloga") == "Admin" ? GetStatusi() : GetStatusi().Where(s => s.Naziv == "Odobren" || s.Naziv == "Odbijen" || s.Naziv == "Za odobravanje");

            model.Statusi = statusi.Select(s =>
                                  new SelectListItem()
                                  {
                                      Value = s.Id.ToString(),
                                      Text = s.Naziv
                                  });

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(string returnUrl, int id)
        {
            Zahtjev zahtjev = dbContext.Zahtjev.Include(z => z.Status).FirstOrDefault(z => z.Id == id);

            if (zahtjev == null)
            {
                Response.StatusCode = 404;
                logger.LogWarning($"User: {User.Identity.Name}, Zahtjev nije nađen, Id = {id} ");
                return View("ZahtjevNotFound", id);
            }

            if (!(await authorizationService.AuthorizeAsync(User, zahtjev, "CanViewDetailsZahtjev")).Succeeded)
            {
                return new ForbidResult();
            }

            ZahtjevDetailsViewModel zahtjevDetailsViewModel = new ZahtjevDetailsViewModel
            {
                Id = zahtjev.Id,
                ImePrezimePodnositelj = zahtjev.ImePrezimePodnositelj,
                OrganizacijskaJedinicaPodnositelj = zahtjev.OrganizacijskaJedinicaPodnositelj,
                OdobravateljImePrezime = zahtjev.OdobravateljImePrezime,
                DatumOtvaranja = zahtjev.DatumOtvaranja,
                DatumOdsustva = zahtjev.DatumOdsustva,
                VrijemeOdsustvaOd = zahtjev.VrijemeOdsustvaOd,
                VrijemeOdsustvaDo = zahtjev.VrijemeOdsustvaDo,
                Komentar = zahtjev.Komentar,
                RazlogOdbijanja = zahtjev.RazlogOdbijanja,
                StatusNaziv = zahtjev.Status.Naziv,
                ReturnUrl = returnUrl,
            };

            return View(zahtjevDetailsViewModel);
        }

        public static string GetString(IHtmlContent content)
        {
            using (var writer = new System.IO.StringWriter())
            {
                content.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
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

        public List<SpGetOrganizacijskeJedinice> GetOrganizacijskeJedinice()
        {
            if (!cache.TryGetValue("OrganizacijskeJedinice", out List<SpGetOrganizacijskeJedinice> organizacijskeJednice))
            {
                var refreshPermissionsPeriodInSeconds = int.Parse(configuration.GetSection("RefreshCodeBooksPeriodInSeconds").Value);

                organizacijskeJednice = dbContext.SpGetOrganizacijskeJedinice();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(refreshPermissionsPeriodInSeconds));

                cache.Set("OrganizacijskeJedinice", organizacijskeJednice, cacheEntryOptions);
            }

            return organizacijskeJednice;
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
            catch(Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string returnUrl, int id)
        {
            Zahtjev zahtjev = dbContext.Zahtjev.Include(z => z.Status).FirstOrDefault(z => z.Id == id);

            if (zahtjev == null)
            {
                Response.StatusCode = 404;
                logger.LogWarning($"User: {User.Identity.Name}, Zahtjev nije nađen, Id = {id} ");
                return View("ZahtjevNotFound", id);
            }

            if (!(await authorizationService.AuthorizeAsync(User, zahtjev, "CanDeleteZahtjev")).Succeeded)
            {
                return new ForbidResult();
            }

            // ajmo brisati, mijenjam samo status u obrisan i mailam promjene
            // ako korisnik mijenja, to jest ako  podnositelj = User.FindFirstValue("Username"); onda ništa ne mjenjam
            // ako ne, onda kao kod edita pamtim starog odobravatelja itd.., logika kao i u edita !!!

            var stariOdobravateljEmail = zahtjev.OdobravateljEmail;
            var stariOdobravateljImePrezime = zahtjev.OdobravateljImePrezime;
            var noviOdobravateljImePrezime = User.FindFirstValue("ImePrezime");

            var stariStatus = zahtjev.Status.Naziv;

            zahtjev.StatusId = GetStatusi().FirstOrDefault(s => s.Naziv == "Obrisan").Id;
            zahtjev.ModifedBy = User.Identity.Name;
            zahtjev.ModifiedDate = DateTime.Now;

            // samo ako korisnik nije onaj koji briše, onda mijenjaj vrijednosti, inače ostavi isto
            if (zahtjev.Podnositelj != User.FindFirstValue("Username"))
            {
                zahtjev.OdobravateljADName = User.FindFirstValue("Username");
                zahtjev.OdobravateljImePrezime = User.FindFirstValue("ImePrezime");
                zahtjev.OdobravateljNazivOrganizacijskeJedinice = User.FindFirstValue("OdobravateljNazivOrganizacijskeJedinice");
                zahtjev.OdobravateljPutanja = User.FindFirstValue("Putanja");
                zahtjev.OdobravateljEmail = User.FindFirstValue("Email");
            }

            dbContext.Zahtjev.Update(zahtjev);
            dbContext.SaveChanges();

            // ako je promijenjen status ili razlog odbijanja ili odobravatelj, onda pošalji mail podnositelju i odobravatelju !!!

            string returnUrlZahtjev = this.Url.Action("Index", "Zahtjev");
            string relativeDetailUrl = this.Url.Action("Details", "Zahtjev", new { returnUrlZahtjev, id = zahtjev.Id });
            string fullDetailUrl = $"{this.Request.Scheme}://{this.Request.Host.Value}{relativeDetailUrl}";

            HtmlContentBuilder htmlBuilder = new HtmlContentBuilder();

            htmlBuilder.AppendFormat($"<h4>Vrijeme izmjene: { zahtjev.ModifiedDate:dd.MM.yyyy HH:mm} </h4>");

            htmlBuilder.AppendFormat($"<h4>Promijenjen je status iz <b>{stariStatus}</b> u <b>Obrisan</b> </h4>");

            // šalji mail samo ako odobravatelj nije stari i ako doboravatelj nije podnositelj
            if ((stariOdobravateljImePrezime != noviOdobravateljImePrezime) && (zahtjev.Podnositelj != User.FindFirstValue("Username")))
            {
                htmlBuilder.AppendFormat($"<h4>Promijenjen je odobravatelj iz <b>{stariOdobravateljImePrezime}</b> u <b>{noviOdobravateljImePrezime}</b> </h4>");
            }

            htmlBuilder.AppendFormat("<a href={0}>ID: {1} </a>", fullDetailUrl, zahtjev.Id);
            htmlBuilder.AppendFormat("<h3>Podnositelj: {0} </h3>", zahtjev.ImePrezimePodnositelj);
            htmlBuilder.AppendFormat("<h4>Organizacijska jedinica: {0} </h4>", zahtjev.OrganizacijskaJedinicaPodnositelj);
            htmlBuilder.AppendFormat("<h4>Odobravatelj: {0} </h4>", zahtjev.OdobravateljImePrezime);
            htmlBuilder.AppendFormat("<h4>Datum i vrijeme otvaranja: {0} </h4>", zahtjev.DatumOtvaranja.ToString("dd.MM.yyyy HH:mm"));
            htmlBuilder.AppendFormat("<h4>Flexi datum: {0} </h4>", zahtjev.DatumOdsustva.ToString("dd.MM.yyyy"));
            htmlBuilder.AppendFormat("<h4>Flexi vrijeme od: {0} </h4>", zahtjev.VrijemeOdsustvaOd.ToString("HH:mm"));
            htmlBuilder.AppendFormat("<h4>Flexi vrijeme do: {0} </h4>", zahtjev.VrijemeOdsustvaDo.ToString("HH:mm"));
            htmlBuilder.AppendFormat("<h4>Komentar: {0} </h4>", zahtjev.Komentar);
            htmlBuilder.AppendFormat("<h4>Status: {0} </h4>", "Obrisan");
            htmlBuilder.AppendFormat("<h4>Razlog odbijanja: {0} </h4>", zahtjev.RazlogOdbijanja);

            var outputHtml = GetString(htmlBuilder);


            if (zahtjev.Podnositelj == User.FindFirstValue("Username"))
            {
                await mailer.SendEmailAsync(zahtjev.PodnositeljEmail, $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);

                await mailer.SendEmailAsync(stariOdobravateljEmail, $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);

                return Redirect(returnUrl);
            }

            await mailer.SendEmailAsync(zahtjev.PodnositeljEmail, $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);
            await mailer.SendEmailAsync(User.FindFirstValue("Email"), $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);

            // šalji i originalnom odobravatelju ako nije isti kao trenutni user koji je editirao zahtjev
            if (User.FindFirstValue("Email") != stariOdobravateljEmail)
            {
                await mailer.SendEmailAsync(stariOdobravateljEmail, $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(string returnUrl, int id)
        {
            Zahtjev zahtjev = dbContext.Zahtjev.Include(z => z.Status).FirstOrDefault(z => z.Id == id);

            if (zahtjev == null)
            {
                Response.StatusCode = 404;
                logger.LogWarning($"User: {User.Identity.Name}, Zahtjev nije nađen, Id = {id} ");
                return View("ZahtjevNotFound", id);
            }

            if (!(await authorizationService.AuthorizeAsync(User, zahtjev, "CanCancelZahtjev")).Succeeded)
            {
                return new ForbidResult();
            }

            // ajmo brisati, mijenjam samo status u obrisan i mailam promjene
            // ako korisnik mijenja, to jest ako  podnositelj = User.FindFirstValue("Username"); onda ništa ne mjenjam
            // ako ne, onda kao kod edita pamtim starog odobravatelja itd.., logika kao i u edita !!!

            var stariOdobravateljEmail = zahtjev.OdobravateljEmail;
            var stariOdobravateljImePrezime = zahtjev.OdobravateljImePrezime;
            var noviOdobravateljImePrezime = User.FindFirstValue("ImePrezime");

            var stariStatus = zahtjev.Status.Naziv;

            zahtjev.StatusId = GetStatusi().FirstOrDefault(s => s.Naziv == "Otkazan").Id;
            zahtjev.ModifedBy = User.Identity.Name;
            zahtjev.ModifiedDate = DateTime.Now;

            // samo ako korisnik nije onaj koji briše, onda mijenjaj vrijednosti, inače ostavi isto
            if (zahtjev.Podnositelj != User.FindFirstValue("Username"))
            {
                zahtjev.OdobravateljADName = User.FindFirstValue("Username");
                zahtjev.OdobravateljImePrezime = User.FindFirstValue("ImePrezime");
                zahtjev.OdobravateljNazivOrganizacijskeJedinice = User.FindFirstValue("OdobravateljNazivOrganizacijskeJedinice");
                zahtjev.OdobravateljPutanja = User.FindFirstValue("Putanja");
                zahtjev.OdobravateljEmail = User.FindFirstValue("Email");
            }

            dbContext.Zahtjev.Update(zahtjev);
            dbContext.SaveChanges();

            // ako je promijenjen status ili razlog odbijanja ili odobravatelj, onda pošalji mail podnositelju i odobravatelju !!!

            string returnUrlZahtjev = this.Url.Action("Index", "Zahtjev");
            string relativeDetailUrl = this.Url.Action("Details", "Zahtjev", new { returnUrlZahtjev, id = zahtjev.Id });
            string fullDetailUrl = $"{this.Request.Scheme}://{this.Request.Host.Value}{relativeDetailUrl}";

            HtmlContentBuilder htmlBuilder = new HtmlContentBuilder();

            htmlBuilder.AppendFormat($"<h4>Vrijeme izmjene: { zahtjev.ModifiedDate:dd.MM.yyyy HH:mm} </h4>");

            htmlBuilder.AppendFormat($"<h4>Promijenjen je status iz <b>{stariStatus}</b> u <b>Otkazan</b> </h4>");

            // šalji mail samo ako odobravatelj nije stari i ako doboravatelj nije podnositelj
            if ((stariOdobravateljImePrezime != noviOdobravateljImePrezime) && (zahtjev.Podnositelj != User.FindFirstValue("Username")))
            {
                htmlBuilder.AppendFormat($"<h4>Promijenjen je odobravatelj iz <b>{stariOdobravateljImePrezime}</b> u <b>{noviOdobravateljImePrezime}</b> </h4>");
            }

            htmlBuilder.AppendFormat("<a href={0}>ID: {1} </a>", fullDetailUrl, zahtjev.Id);
            htmlBuilder.AppendFormat("<h3>Podnositelj: {0} </h3>", zahtjev.ImePrezimePodnositelj);
            htmlBuilder.AppendFormat("<h4>Organizacijska jedinica: {0} </h4>", zahtjev.OrganizacijskaJedinicaPodnositelj);
            htmlBuilder.AppendFormat("<h4>Odobravatelj: {0} </h4>", zahtjev.OdobravateljImePrezime);
            htmlBuilder.AppendFormat("<h4>Datum i vrijeme otvaranja: {0} </h4>", zahtjev.DatumOtvaranja.ToString("dd.MM.yyyy HH:mm"));
            htmlBuilder.AppendFormat("<h4>Flexi datum: {0} </h4>", zahtjev.DatumOdsustva.ToString("dd.MM.yyyy"));
            htmlBuilder.AppendFormat("<h4>Flexi vrijeme od: {0} </h4>", zahtjev.VrijemeOdsustvaOd.ToString("HH:mm"));
            htmlBuilder.AppendFormat("<h4>Flexi vrijeme do: {0} </h4>", zahtjev.VrijemeOdsustvaDo.ToString("HH:mm"));
            htmlBuilder.AppendFormat("<h4>Komentar: {0} </h4>", zahtjev.Komentar);
            htmlBuilder.AppendFormat("<h4>Status: {0} </h4>", "Otkazan");
            htmlBuilder.AppendFormat("<h4>Razlog odbijanja: {0} </h4>", zahtjev.RazlogOdbijanja);

            var outputHtml = GetString(htmlBuilder);


            if (zahtjev.Podnositelj == User.FindFirstValue("Username"))
            {
                await mailer.SendEmailAsync(zahtjev.PodnositeljEmail, $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);

                await mailer.SendEmailAsync(stariOdobravateljEmail, $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);

                return Redirect(returnUrl);
            }

            await mailer.SendEmailAsync(zahtjev.PodnositeljEmail, $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);
            await mailer.SendEmailAsync(User.FindFirstValue("Email"), $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);

            // šalji i originalnom odobravatelju ako nije isti kao trenutni user koji je editirao zahtjev
            if (User.FindFirstValue("Email") != stariOdobravateljEmail)
            {
                await mailer.SendEmailAsync(stariOdobravateljEmail, $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);
            }

            return Redirect(returnUrl);
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

            return Json(new { VrijemeOdsustvaDo = vrijemeOdsustvaOd.ToString("HH:mm") , Poruka = "" });
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

        public JsonResult IsFlexiDatumValid(string DatumOdsustva)
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

            var zahtjevPostojiZaTajDatum = dbContext.Zahtjev.Where(z => z.Podnositelj == User.FindFirstValue("Username"))
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

            var brojZahtjevaZaTjedanDatumOdsustva = dbContext.Zahtjev.Where(z => z.Podnositelj == User.FindFirstValue("Username"))
                     .Where(z => z.DatumOdsustva >= prviDanUTjednu && z.DatumOdsustva <= zadnjiDanUTjednu).Count();

            if (brojZahtjevaZaTjedanDatumOdsustva >= dozvoljeniBrojDanaTjedan)
            {
                return Json($"Za razdoblje {prviDanUTjednu:dd.MM.yyyy} - {zadnjiDanUTjednu:dd.MM.yyyy} već postoji jedan zahtjev", new Newtonsoft.Json.JsonSerializerSettings());
            }

            return Json(true, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}
