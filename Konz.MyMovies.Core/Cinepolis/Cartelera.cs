using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konz.MyMovies.Core.Cinepolis
{
    public class Cartelera
    {
        public Cartelera()
        {
            Salas = new List<SalaCartelera>();
        }

        public string PeliculaCode { get; set; }

        public string ComplejoCode { get; set; }

        public string CiudadCode { get; set; }

        public List<SalaCartelera> Salas { get; set; }

        public string EsEstreno { get; set; }

        public string CiudadNombre { get; set; }
    }
}
