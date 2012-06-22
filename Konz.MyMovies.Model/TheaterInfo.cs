using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace Konz.MyMovies.Model
{
    public class TheaterInfo
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public List<ShowtimeInfo> Showtimes { get; set; }
        [XmlIgnore]
        public ObservableCollection<MovieInfo> Movies { get; set; }

        public TheaterInfo()
        {
            Showtimes = new List<ShowtimeInfo>();
            Movies = new ObservableCollection<MovieInfo>();
        }

        public string GetShowtimes(string movieCode)
        {
            var times = Showtimes.Where(x => x.MovieCode == movieCode).Select(x => x.Date.ToString("HH:mmtt")).ToList();
            return string.Join(", ", times.ToArray());
        }

        public DateTime NextShow(string movieCode)
        {
            ShowtimeInfo result = null;
            if (Showtimes.Count > 0)
                result = (from s in Showtimes.Where(x => x.MovieCode == movieCode) orderby s.Date where s.Date > DateTime.Now select s).Take(1).SingleOrDefault();
            return result == null ? DateTime.MaxValue : result.Date;
        }

    }
}
