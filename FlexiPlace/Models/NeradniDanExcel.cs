using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.Models
{
    public class NeradniDanExcel
    {
        [Display(Name = "Excel za neradne dane")]
        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Odaberite Excel datoteku sa neradnim danima")]
        public IFormFile ExcelNeradniDani { get; set; }
    }
}
