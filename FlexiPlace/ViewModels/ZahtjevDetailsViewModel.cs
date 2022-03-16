using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.ViewModels
{
    public class ZahtjevDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Podnositelj")]
        public string ImePrezimePodnositelj { get; set; }

        [Display(Name = "Organizacijska jedinica")]
        public string OrganizacijskaJedinicaPodnositelj { get; set; }

        [Display(Name = "Odobravatelj")]
        public string OdobravateljImePrezime { get; set; }

        [Display(Name = "Datum i vrijeme otvaranja")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime DatumOtvaranja { get; set; }

        [Display(Name = "Flexi datum")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DatumOdsustva { get; set; }

        [Display(Name = "Flexi vrijeme od")]
        public DateTime VrijemeOdsustvaOd { get; set; }

        [Display(Name = "Flexi vrijeme do")]
        public DateTime VrijemeOdsustvaDo { get; set; }

        public string Komentar { get; set; }

        [Display(Name = "Razlog odbijanja")]
        public string RazlogOdbijanja { get; set; }

        [Display(Name = "Status")]
        public string StatusNaziv { get; set; }
        public string ReturnUrl { get; set; }
    }
}
