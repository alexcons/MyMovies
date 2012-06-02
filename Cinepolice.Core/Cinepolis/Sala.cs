using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konz.MyMovies.Core.Cinepolis
{
    public class SalaCartelera
    {
        public SalaCartelera()
        {
            Horarios = new Dictionary<string, string>();
        }

        public string Code { get; set; }
        
        public Dictionary<string, string> Horarios { get; set; }
    }
}
