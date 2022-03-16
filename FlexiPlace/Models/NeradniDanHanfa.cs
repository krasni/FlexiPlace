using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.Models
{
    public class NeradniDanHanfa
    {
        public int Id { get; set; }

        [Required]
        public DateTime NeradniDan { get; set; }
    }
}
