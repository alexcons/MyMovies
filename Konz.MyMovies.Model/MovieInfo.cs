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
using System.Xml.Serialization;
using Konz.MyMovies.Core;

namespace Konz.MyMovies.Model
{
    public class Movie
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string PosterURI { get; set; }
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

    }
}
