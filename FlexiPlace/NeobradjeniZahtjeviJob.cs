using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlexiPlace
{
    public interface INeobradjeniZahtjeviJob
    {
        void PromjeniStatusUOdobravanjuUNeobradjeno();
    }

    public class NeobradjeniZahtjeviJob : INeobradjeniZahtjeviJob
    {
        public void PromjeniStatusUOdobravanjuUNeobradjeno()
        {
            Console.WriteLine("Pozdrav iz neobradjeni zahtjevi!!!!");
        }
    }
}
