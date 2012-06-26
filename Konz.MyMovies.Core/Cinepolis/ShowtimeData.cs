using System;
using System.Collections.Generic;
using Konz.MyMovies.Model;

namespace Konz.MyMovies.Core.Cinepolis
{
    public class ShowtimeData
    {
        public ShowtimeData()
        {
            Expiration = new Expiration();
            Movies = new List<Movie>();
            Theaters = new List<Theater>();
            Carteleras = new List<ShowtimeScheadule>();
        }

        public City City { get; set; }

        public DateTime Date { get; set; }

        public Expiration Expiration { get; set; }

        public List<Movie> Movies { get; set; }

        public List<Theater> Theaters { get; set; }

        public List<ShowtimeScheadule> Carteleras { get; set; }
    }
}
