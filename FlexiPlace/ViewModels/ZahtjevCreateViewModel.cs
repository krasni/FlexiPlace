using FlexiPlace.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.ViewModels
{
    public class ZahtjevCreateViewModel
    {
        [Display(Name = "Podnositelj")]
        public string ImePrezimePodnositelj { get; set; }

        [Display(Name = "Organizacijska jedinica")]
        public string OrganizacijskaJedinicaPodnositelj { get; set; }

        [Display(Name = "Odobravatelj")]
        public string OdobravateljImePrezime { get; set; }

        [Required(ErrorMessage = "Flexi datum je obavezno polje")]
        [Display(Name = "Flexi datum")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Remote("IsFlexiDatumValid", "Zahtjev", ErrorMessage = "Flexi datum je neispravan.")]

        public DateTime DatumOdsustva { get; set; }

        [Required(ErrorMessage = "Flexi vrijeme od je obavezno polje")]
        [Display(Name = "Flexi vrijeme od")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [Remote("IsVrijemeOdsustvaOdValid", "Zahtjev", ErrorMessage = "Flexi datum je neispravan.")]
        public DateTime VrijemeOdsustvaOd { get; set; }

        [Required(ErrorMessage = "Flexi vrijeme do obavezno polje")]
        [Display(Name = "Flexi vrijeme do")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime VrijemeOdsustvaDo { get; set; }

        public string Komentar { get; set; }

        public string ReturnUrl { get; set; }
    }
}
