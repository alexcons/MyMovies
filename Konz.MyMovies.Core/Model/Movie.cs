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
        private string _posterUri;
        public string PosterUri
        {
            get
            {
                return Utils.InternetIsAvailable() ? _posterUri : "/Images/Background.png";
            }
            set { _posterUri = value; }
        }

        private BitmapImage _poster;
        [XmlIgnore]
        public BitmapImage Poster
        {
            get
            {
                if (_poster == null && PosterUri != null)
                    _poster = new BitmapImage(new Uri(PosterUri, UriKind.RelativeOrAbsolute));
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


                var mins = NextShow.Subtract(DateTime.Now).Minutes;
                var hours = (int)NextShow.Subtract(DateTime.Now).TotalHours;
                string hoursLegend = string.Empty;
                string minsLegend = string.Empty;
                string message = string.Empty;

                if (hours >= 1)
                    hoursLegend = " {0} " + (hours >= 2 ? "horas" : "hora");

                if (mins >= 1)
                    minsLegend = " {1} " + (mins >= 2 ? "minutos" : "minuto");

                if (hours == 0 && mins > 0 && mins < 5)
                    message = "Empieza ahora!";
                else if (hours == 0 && mins <= 0 && mins > -10)
                    message = "Acaba de empezar todavia alcanzas!";
                else
                    message = "Empieza en";

                return string.Format(message + hoursLegend + minsLegend, hours.ToString(), mins.ToString());
            }
        }

        public DateTime NextShow
        {
            get
            {
                Showtime result = null;
                if (Showtimes.Count > 0)
                    result = (from s in Showtimes orderby s.Date where s.Date > DateTime.Now.Subtract(TimeSpan.FromMinutes(10)) select s).Take(1).SingleOrDefault();
                return result == null ? DateTime.MaxValue : result.Date;
            }
        }

        public string Classification { get; set; }

        public string Actors { get; set; }

        public string Rating { get; set; }
    }
}

