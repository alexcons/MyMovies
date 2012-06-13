using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Konz.MyMovies.Core
{
    public class Showtime : EntityBase
    {
        public DateTime Time { get; set; }

        public string MovieCode { get; set; }

        public string TheaterCode { get; set; }
        
        private Movie _movie;
                
        [XmlIgnore]
        public Movie Movie
        {
            get
            {
                return _movie;
            }
            set
            {
                if (value != null)
                    MovieCode = value.Code;
                else
                    MovieCode = string.Empty;
                _movie = value;
            }
        }
        
        private Theater _theater;

        [XmlIgnore]
        public Theater Theater
        {
            get
            {
                return _theater;
            }
            set
            {
                if (value != null)
                    TheaterCode = value.Code;
                else
                    TheaterCode = string.Empty;
                _theater = value;
            }
        }

        public bool IsNew { get; set; }

        [XmlIgnore]
        public List<Room> Rooms { get; set; }

        public Showtime()
        {
            Rooms = new List<Room>();
        }
    }
}
