using System.Collections.Generic;

namespace Konz.MyMovies.Model
{
    public class ShowtimeRoom
    {
        public ShowtimeRoom()
        {
            Showtimes = new List<Showtime>();
        }

        public string Code { get; set; }

        public List<Showtime> Showtimes { get; set; }
    }
}
