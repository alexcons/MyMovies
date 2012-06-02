using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konz.MyMovies.Core.Cinepolis
{
    public class SalaComplejo
    {
        public SalaComplejo()
        {
            Sonidos = new List<string>();
        }

        public string Code { get; set; }

        public string Orden { get; set; }

        public List<string> Sonidos { get; set; }
    }
}
