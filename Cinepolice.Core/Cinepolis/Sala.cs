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
            Horarios = new List<string>();
        }

        public string Code { get; set; }
        
        public List<string> Horarios { get; set; }
    }
}
