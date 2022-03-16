using FlexiPlace.Models.DB;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.ViewModels
{
    public class ZahtjevCreateAdminViewModel
    {
        [Display(Name = "Djelatnik")]
        [Required(ErrorMessage = "Odaberite djelatnika")]
        public string UserName { get; set; }

        public List<SpGetDjelatnici> Djelatnici { get; set; }

        [Required(ErrorMessage = "Flexi datum je obavezno polje")]
        [Display(Name = "Flexi datum")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Remote("IsFlexiDatumValid", "Administracija", AdditionalFields = "UserName", ErrorMessage = "Flexi datum je neispravan.")]

        public DateTime DatumOdsustva { get; set; }

        [Required(ErrorMessage = "Flexi vrijeme od je obavezno polje")]
        [Display(Name = "Flexi vrijeme od")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [Remote("IsVrijemeOdsustvaOdValid", "Administracija", ErrorMessage = "Flexi datum je neispravan.")]
        public DateTime VrijemeOdsustvaOd { get; set; }

        [Required(ErrorMessage = "Flexi vrijeme do obavezno polje")]
        [Display(Name = "Flexi vrijeme do")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime VrijemeOdsustvaDo { get; set; }

        public string Komentar { get; set; }
    }
}
