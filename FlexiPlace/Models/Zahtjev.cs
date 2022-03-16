using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.Models
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
        [Display(Name = "Datum odsustva")]
        public DateTime DatumOdsustva { get; set; }

        [Required]
        [Display(Name = "Vrijeme odsustva od")]
        public DateTime VrijemeOdsustvaOd { get; set; }

        [Required]
        [Display(Name = "Vrijeme odsustva do")]
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
