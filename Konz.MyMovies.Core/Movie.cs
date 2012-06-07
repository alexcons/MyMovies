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
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace Konz.MyMovies.Core
{
    public class Movie : EntityBase
    {

        /// <summary>
        /// Movie Directors
        /// </summary>
        public List<Artist> Directors {get; set;}

        /// <summary>
        /// Movie Writers
        /// </summary>
        public List<Artist> Writers {get; set;}

        /// <summary>
        /// Cinepolis Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Poster Image URI
        /// </summary>
        public string PictureURI { get; set; }

        /// <summary>
        /// Movie Plot
        /// </summary>
        public string Sinopsis {get; set;}

        /// <summary>
        /// Main Cast Members
        /// </summary>
        public List<Artist> Cast {get; set;}

        /// <summary>
        /// Movie Rating
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Movie Trailer
        /// </summary>
        public string TrailerURI { get; set; }

        /// <summary>
        /// Other Information
        /// </summary>
        public IDictionary<string, string> PropertyBag { get; set; }

        public Movie()
        {
            Directors = new List<Artist>();
            Writers = new List<Artist>();
            Cast = new List<Artist>();
            PropertyBag = new Dictionary<string, string>();
            Shows = new List<Showtime>();
        }

        public List<Showtime> Shows { get; set; }

        public string Showtimes
        {
            get
            {
                var result = (from s in Shows select s.Time.ToString("HH:mm")).ToArray();
                return string.Join(", ", result);
            }
        }

        BitmapImage _picture;
        public BitmapImage PictureImg
        {
            get
            {
                if (_picture == null)
                    _picture = new BitmapImage(new Uri(PictureURI));
                return _picture;
            }
        }

        public Showtime NextShow
        {
            get
            {
                Showtime result = null;
                if (Shows.Count > 0)
                    result = (from s in Shows orderby s.Time where s.Time > DateTime.Now select s).Take(1).SingleOrDefault();
                return result??new Showtime() { Time = DateTime.MaxValue};
            }
        }

        public string NextShowLegend
        {
            get 
            {
                if (NextShow.Time == DateTime.MaxValue)
                    return "Hoy no hay mas funciones";
                var mins = Math.Round(NextShow.Time.Subtract(DateTime.Now).TotalMinutes);
                return "Empieza en " + mins.ToString() + " minutos";
            }
        }
    }
}
