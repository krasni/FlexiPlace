using FlexiPlaceWinServiceTopShelf.Infrastructure;
using FlexiPlaceWinServiceTopShelf.Models;
using FlexiPlaceWinServiceTopShelf.Modles;
using log4net;
using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text.Encodings.Web;

namespace FlexiPlaceWinServiceTopShelf
{
    public class Service
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Start()
        {
            using (var dbContext = new FlexiPlaceDBContext())
            {
                // ovaj job bi trebao prebaciti stanje iz za odobravanje u neobrađeno
                // ako je prošlo dana kao u parametar tablici za kolonu DozvoljeniBrojaDanaOdobrenje
                // treba naravno poslati mail o promjeni statusa 

                Logger.Info($"Hangfire je pokreno job u {DateTime.Now}");

                try
                {
                    // učitaj broj dana za automatsko stavljanje iz u odobravanju u neobrađeno
                    Parametar parametar = dbContext.Parametar.FirstOrDefault();
                    int brojDana = parametar.DozvoljeniBrojaDanaOdobrenje;

                    // dohvati sve zahtjeve kojima (datum.now - datum otvaranja) > broj dana i status je za odobravanje
                    //var zahtjeviZaNeobradjeno = dbContext.Zahtjev.Include(z => z.Status).Where(z => (DateTime.Now - z.DatumOtvaranja).Days > brojDana && z.Status.Naziv == "Za odobravanje").ToList();

                    var zahtjeviZaNeobradjeno = dbContext.Zahtjev.Include(z => z.Status).Where(z => z.Status.Naziv == "Za odobravanje").ToList();

                    zahtjeviZaNeobradjeno = zahtjeviZaNeobradjeno.Where(z => (DateTime.Now - z.DatumOtvaranja).Days > brojDana).ToList();

                    // stavi status u neobrađeno za početak i snimi u bazu, poslje ide mailanje !!!!

                    // nađi id od neobrađen
                    var neobradjenId = dbContext.Status.FirstOrDefault(s => s.Naziv == "Neobrađen").Id;

                    foreach (var zahtjev in zahtjeviZaNeobradjeno)
                    {
                        zahtjev.StatusId = neobradjenId;
                        zahtjev.ModifiedDate = DateTime.Now;
                        zahtjev.ModifedBy = "HangFireJob";
                        var entity = dbContext.Zahtjev.Attach(zahtjev);
                        entity.State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    dbContext.SaveChanges();

                    // složi mailove
                    var rootPath = ConfigurationManager.AppSettings["RootPath"];

                    var smtpSettings = new SmtpSettings();
                    smtpSettings.Server = ConfigurationManager.AppSettings["Server"];
                    smtpSettings.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                    smtpSettings.SenderName = ConfigurationManager.AppSettings["SenderName"];
                    smtpSettings.SenderEmail = ConfigurationManager.AppSettings["SenderEmail"];

                    var mailer = new Mailer(smtpSettings);

                    foreach (var zahtjev in zahtjeviZaNeobradjeno)
                    {
                        HtmlContentBuilder htmlBuilder = new HtmlContentBuilder();

                        htmlBuilder.AppendFormat($"<h4>Vrijeme izmjene: { DateTime.Now:dd.MM.yyyy HH:mm} </h4>");
                        htmlBuilder.AppendFormat($"<h4>Promijenjen je status iz <b>Za odobravanje</b> u <b>Neobrađen</b> </h4>");

                        var fullDetailUrlZahtjev = rootPath + $"/Zahtjev/Details/{zahtjev.Id}";

                        htmlBuilder.AppendFormat("<a href={0}>ID: {1} </a>", fullDetailUrlZahtjev, zahtjev.Id);
                        htmlBuilder.AppendFormat("<h3>Podnositelj: {0} </h3>", zahtjev.ImePrezimePodnositelj);
                        htmlBuilder.AppendFormat("<h4>Organizacijska jedinica: {0} </h4>", zahtjev.OrganizacijskaJedinicaPodnositelj);
                        htmlBuilder.AppendFormat("<h4>Odobravatelj: {0} </h4>", zahtjev.OdobravateljImePrezime);
                        htmlBuilder.AppendFormat("<h4>Datum i vrijeme otvaranja: {0} </h4>", zahtjev.DatumOtvaranja.ToString("dd.MM.yyyy HH:mm"));
                        htmlBuilder.AppendFormat("<h4>Flexi datum: {0} </h4>", zahtjev.DatumOdsustva.ToString("dd.MM.yyyy"));
                        htmlBuilder.AppendFormat("<h4>Flexi vrijeme od: {0} </h4>", zahtjev.VrijemeOdsustvaOd.ToString("HH:mm"));
                        htmlBuilder.AppendFormat("<h4>Flexi vrijeme do: {0} </h4>", zahtjev.VrijemeOdsustvaDo.ToString("HH:mm"));
                        htmlBuilder.AppendFormat("<h4>Komentar: {0} </h4>", zahtjev.Komentar);
                        htmlBuilder.AppendFormat("<h4>Status: {0} </h4>", zahtjev.Status.Naziv);
                        htmlBuilder.AppendFormat("<h4>Razlog odbijanja: {0} </h4>", zahtjev.RazlogOdbijanja);

                        var outputHtml = GetString(htmlBuilder);

                        // pošalji mail podnositelju i odobravatelju, sa html-om cijelog zahjeva i linkom na details !!!
                        mailer.SendEmailAsync(zahtjev.PodnositeljEmail, $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);
                        mailer.SendEmailAsync(zahtjev.OdobravateljEmail, $"Flexiplace - Promijenjen je zahtjev - ID: {zahtjev.Id}", outputHtml);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Greška kod izvršavanja joba {ex.ToString()}");
                }
            }
        }

        public string GetString(IHtmlContent content)
        {
            using (var writer = new System.IO.StringWriter())
            {
                content.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }

        public void Stop() { }
    }
}
