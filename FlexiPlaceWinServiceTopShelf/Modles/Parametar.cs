using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlaceWinServiceTopShelf.Models
{
    public class Parametar
    {
        public int Id { get; set; }

        [Required]
        public int DozvoljeniBrojDanaMjesecu { get; set; }

        [Required]
        public int DozvoljeniBrojaDanaOdobrenje { get; set; }
    }
}
