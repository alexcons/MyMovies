using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konz.MyMovies.Core
{
    public class Showtime : EntityBase
    {
        public int ShowtimeId { get; set; }

        public DateTime Time { get; set; }

        public Movie Movie { get; set; }

        public Theater Theater { get; set; }

        public List<Room> Rooms { get; set; }

        public bool IsNew { get; set; }

        public Showtime()
        {
            Rooms = new List<Room>();
        }
    }
}
