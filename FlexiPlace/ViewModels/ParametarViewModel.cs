using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.ViewModels
{
    public class ParametarViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Administracija dozvoljenog broja dana u mjesecu")]
        [Required]
        public int DozvoljeniBrojDanaMjesec { get; set; }

        [Display(Name = "Administracija dozvoljenog broja dana u tjednu")]
        [Required]
        public int DozvoljeniBrojDanaTjedan { get; set; }

        [Required]
        [Display(Name = "Administracija dozvoljenog broja dana za odobrenje")]
        public int DozvoljeniBrojaDanaOdobrenje { get; set; }
    }
}
