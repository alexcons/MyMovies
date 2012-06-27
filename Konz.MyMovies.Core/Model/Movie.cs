using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Konz.MyMovies.Core;
using System.Windows.Media.Imaging;

namespace Konz.MyMovies.Model
{
    public class Movie
    {
        private string _posterURI;
        public string PosterURI
        {
            get 
            {
                return Utils.InternetIsAvailable() ? _posterURI : "/Images/Background.png"; 
            }
            set { _posterURI = value; }
        }

        private BitmapImage _poster;        
        [XmlIgnore]
        public BitmapImage Poster
        {
            get
            {
                if (_poster == null && PosterURI != null)
                    _poster = new BitmapImage(new Uri(PosterURI, UriKind.RelativeOrAbsolute));
                return _poster;
            }
        }

        public string Code { get; set; }
        public string Title { get; set; }
        public string Sinopsis { get; set; }
        
        [XmlIgnore]
        public List<Showtime> Showtimes { get; set; }

        public Movie()
        {
            Showtimes = new List<Showtime>();
        }
        
        public string ShowtimesHours
        {
            get
            {
                var times = Showtimes.Select(x => Utils.GetTimeFormat(x.Date)).ToList();
                return string.Join(", ", times.ToArray());
            }
        }

        public string NextShowLegend
        {
            get
            {
                if (NextShow == DateTime.MaxValue)
                    return "Hoy no hay mas funciones";
                var mins = Math.Round(NextShow.Subtract(DateTime.Now).TotalMinutes);
                return "Empieza en " + mins.ToString() + " minutos";
            }
        }
        
        public DateTime NextShow
        {
            get
            {
                Showtime result = null;
                if (Showtimes.Count > 0)
                    result = (from s in Showtimes orderby s.Date where s.Date > DateTime.Now select s).Take(1).SingleOrDefault();
                return result == null ? DateTime.MaxValue : result.Date;
            }
        }

        public string Classification { get; set; }

        public string Actors { get; set; }

        public string Rating { get; set; }
    }
}
