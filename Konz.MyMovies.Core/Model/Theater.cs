using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Konz.MyMovies.Core;

namespace Konz.MyMovies.Model
{
    public class Theater
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public List<Showtime> Showtimes { get; set; }
        [XmlIgnore]
        public ObservableCollection<Movie> Movies { get; set; }
        
        public List<TheaterRoom> TheaterRooms { get; set; }

        public string GetShowtimes(string movieCode)
        {
            var times = Showtimes.Where(x => x.MovieCode == movieCode).Select(x => Utils.GetTimeFormat(x.Date)).ToList();
            return string.Join(", ", times.ToArray());
        }

        public DateTime NextShow(string movieCode)
        {
            Showtime result = null;
            if (Showtimes.Count > 0)
                result = (from s in Showtimes.Where(x => x.MovieCode == movieCode) orderby s.Date where s.Date > DateTime.Now select s).Take(1).SingleOrDefault();
            return result == null ? DateTime.MaxValue : result.Date;
        }

        public string Order { get; set; }

        public Theater()
        {
            Showtimes = new List<Showtime>();
            Movies = new ObservableCollection<Movie>();
            TheaterRooms = new List<TheaterRoom>();
        }

    }
}
