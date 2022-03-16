using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.Models
{
    public class Parametar
    {
        public int Id { get; set; }

        [Required]
        public int DozvoljeniBrojDanaMjesec { get; set; }

        [Required]
        public int DozvoljeniBrojDanaTjedan { get; set; }

        [Required]
        public int DozvoljeniBrojaDanaOdobrenje { get; set; }
    }
}
