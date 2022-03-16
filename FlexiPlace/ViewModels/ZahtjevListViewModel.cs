using FlexiPlace.Models;
using FlexiPlace.Models.DB;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.ViewModels
{
    public class ZahtjevListViewModel
    {
        public IEnumerable<Zahtjev> Zahtjevi { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string StatusNaziv { get; set; }

        public string PodnositeljOrganizacijskaJedinicaNaziv { get; set; }
        public string OdobravateljOrganizacijskaJedinicaNaziv { get; set; }

        public string Podnositelj { get; set; }
        public string Odobravatelj { get; set; }

        public List<Status> Statusi { get; set; }

        public List<SpGetOrganizacijskeJedinice> PodnositeljOrganizacijskeJedinice { get; set; }
        public List<SpGetOrganizacijskeJedinice> OdobravateljOrganizacijskeJedinice { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? DatumOtvaranjaOd { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? DatumOtvaranjaDo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? DatumOdsustvaOd { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? DatumOdsustvaDo { get; set; }
    }
}
