using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace.Models.DB
{
    public class SpGetUserDataBP2
    {
        public string UserName { get; set; }
        public string ImePrezime { get; set; }
        public string Email { get; set; }
        public string Uloga { get; set; }
        public string UlogaHanfa { get; set; }
        public string NazivOrganizacijskeJedinice { get; set; }
        public Int16 Lvl { get; set; }
        public string Putanja { get; set; }
        public string OdobravateljADName { get; set; }
        public string OdobravateljImePrezime { get; set; }
        public string OdobravateljNazivOrganizacijskeJedinice { get; set; }
        public string OdobravateljPutanja { get; set; }
        public string OdobravateljEmail { get; set; }
    }
}
