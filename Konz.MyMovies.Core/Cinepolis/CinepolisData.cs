using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konz.MyMovies.Core.Cinepolis
{
    public class CinepolisData
    {
        public CinepolisData()
        {
            Vigencia = new Vigencia();
            Peliculas = new List<Pelicula>();
            Complejos = new List<Complejo>();
            Carteleras = new List<Cartelera>();
        }

        public City Ciudad { get; set; }

        public DateTime Fecha { get; set; }

        public Vigencia Vigencia { get; set; }

        public List<Pelicula> Peliculas { get; set; }

        public List<Complejo> Complejos { get; set; }

        public List<Cartelera> Carteleras { get; set; }
    }
}
