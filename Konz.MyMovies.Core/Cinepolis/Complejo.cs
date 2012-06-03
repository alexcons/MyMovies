using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konz.MyMovies.Core.Cinepolis
{
    public class Complejo
    {
        public Complejo()
        {
            Salas = new List<SalaComplejo>();
        }

        public string Code { get; set; }

        public string CiudadCode { get; set; }

        public string Nombre { get; set; }

        public string Orden { get; set; }

        public List<SalaComplejo> Salas { get; set; }
    }
}
