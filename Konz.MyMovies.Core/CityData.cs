using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Konz.MyMovies.Core
{
    public class CityData
    {
        public List<Showtime> Shows { get; set; }
        public List<Movie> Movies { get; set; }
        public List<Theater> Theaters { get; set; }

        public string CityCode { get; set; }

        public CityData()
        {
            Shows = new List<Showtime>();
            Movies = new List<Movie>();
            Theaters = new List<Theater>();
        }

        public List<Showtime> GetShows()
        {
            foreach (var show in Shows)
            {
                show.Movie = Movies.Where(x => x.Code == show.MovieCode).SingleOrDefault();
                show.Theater = Theaters.Where(x => x.Code == show.TheaterCode).SingleOrDefault();
                if (show.Time.Date != DateTime.Today)
                    show.Time = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, show.Time.Hour, show.Time.Minute, 0);
            }
            return Shows;
        }


        public static CityData GetCityDataFromShows(string cityCode, List<Showtime> result)
        {
            var data = new CityData();
            data.CityCode = cityCode;
            data.Movies = result.Select(x => x.Movie).Distinct().ToList();
            data.Shows = result;
            data.Theaters = result.Select(x => x.Theater).Distinct().ToList();
            return data;
        }
    }
}
