using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlaceWinServiceTopShelf.Models
{
    public class Zahtjev
    {
        public int Id { get; set; }

        [Required]
        public string Podnositelj  { get;set;}

        [Display(Name = "Podnositelj")]
        public string ImePrezimePodnositelj { get; set; }

        [Display(Name = "Organizacijska jedinica")]
        public string OrganizacijskaJedinicaPodnositelj { get; set; }

        public string OrganizacijskaJedinicaPutanjaPodnositelj { get; set; }

        [Display(Name = "Odobravatelj")]
        public string OdobravateljImePrezime { get; set; }

        public string OdobravateljADName { get; set; }

        public string OdobravateljNazivOrganizacijskeJedinice { get; set; }

        public string OdobravateljPutanja { get; set; }

        public string Komentar { get; set; }

        [Required]
        public int StatusId { get; set; }
        public Status Status { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        [Display(Name = "Datum i vrijeme otvaranja")]
        public DateTime DatumOtvaranja { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Display(Name = "Datum odstustva")]
        public DateTime DatumOdsustva { get; set; }

        [Required(ErrorMessage = "Flexi vrijeme od je obavezno polje")]
        [Display(Name = "Flexi vrijeme od")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime VrijemeOdsustvaOd { get; set; }

        [Required(ErrorMessage = "Flexi vrijeme do obavezno polje")]
        [Display(Name = "Flexi vrijeme do")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime VrijemeOdsustvaDo { get; set; }

        public string RazlogOdbijanja { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public string ModifedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public string PodnositeljEmail { get; set; }

        public string OdobravateljEmail { get; set; }
    }
}
