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

        public DateTime Fecha { get; set; }

        public Vigencia Vigencia { get; set; }

        public List<Pelicula> Peliculas { get; set; }

        public List<Complejo> Complejos { get; set; }

        public List<Cartelera> Carteleras { get; set; }

        public List<Showtime> GetShowTimes(CinepolisData result)
        {
            var movies = new List<Movie>();
            var city = new City()
            {
                Code = result.Carteleras[0].CiudadCode,
                Name = result.Carteleras[0].CiudadNombre
            };
            
            foreach (var p in result.Peliculas)
            {
                var m = new Movie()
                {
                    Code = p.Code,
                    Name = p.Nombre,
                    PictureURI = string.Format("http://www.cinepolis.com.mx/imagenes/peliculas/{0}", p.Cartel),
                    Sinopsis = p.Sinopsis
                };

                foreach (var a in p.Actores.Split(",".ToArray()))
                {
                    m.Cast.Add(new Artist() {
                        Name = a
                    });
                }
            }
            foreach (var c in result.Complejos)
            {
                var t = new Theater()
                {
                    Code = c.Code,
                    Name = c.Nombre,
                    Order = int.Parse(c.Orden)
                };

                foreach (var s in c.Salas)
                {
                    t.Rooms.Add( new Room()
                    {
                        Code = int.Parse(s.Code),
                        Name = s.Code,
                        Order = int.Parse(s.Orden)
                    });
                }
                city.Theaters.Add(t);
            }

            var shows = new List<Showtime>();
            foreach (var c in result.Carteleras)
            {
                foreach (var s in c.Salas)
	            {
                    foreach (var h in s.Horarios)
                    {
                        var st = new Showtime();
                        st.Theater = city.Theaters.Where(x => x.Code == c.ComplejoCode).SingleOrDefault();
                        st.Movie = movies.Where(x => x.Code == c.PeliculaCode).SingleOrDefault();
                        st.Room = st.Theater.Rooms.Where(x => x.Code == int.Parse(s.Code)).SingleOrDefault();
                        var timeParts = h.Value.Split(":".ToArray());
                        var hr = int.Parse(timeParts[0]);
                        var mn = int.Parse(timeParts[1]);
                        st.Code = h.Key;
                        st.Time = new DateTime(Fecha.Year, Fecha.Month, Fecha.Day, hr, mn, 00);
                        shows.Add(st);
                    }
                }
            }

            return shows;
        }
    }
}
