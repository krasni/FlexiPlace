using FlexiPlace.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.ViewModels
{
    public class ZahtjevEditViewModel
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

        [Required]
        [Display(Name = "Flexi datum")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime DatumOdsustva { get; set; }

        [Required(ErrorMessage = "Flexi vrijeme od obavezno polje")]
        [Display(Name = "Flexi vrijeme od")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime VrijemeOdsustvaOd { get; set; }

        [Required(ErrorMessage = "Flexi vrijeme do obavezno polje")]
        [Display(Name = "Flexi vrijeme do")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime VrijemeOdsustvaDo { get; set; }

        public string Komentar { get; set; }

        [Display(Name = "Razlog odbijanja")]
        [RequiredIfTrue(nameof(IsStatusOdbijen), ErrorMessage = "Razlog odbijanja je obavezan ako je status zahtjeva odbijen")]
        public string RazlogOdbijanja { get; set; }

        public bool IsStatusOdbijen
        {
            get
            {
                return StatusNaziv == "Odbijen";
            }
        }

        [Required]
        [Display(Name = "Status")]
        public int StatusID { get; set; }

        public string StatusNaziv { get; set; }

        public IEnumerable<SelectListItem> Statusi { get; set; }

        public string ReturnUrl { get; set; }
    }
}
