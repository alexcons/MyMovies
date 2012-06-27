using System;
using System.Linq;

namespace Konz.MyMovies.Model
{
    public class Showtime
    {
        public DateTime Date { get; set; }
        public string MovieCode { get; set; }

        public Showtime()
        {
        }

        public Showtime(string movieCode, DateTime date, string hr)
        {
            var timeParts = hr.Split(":".ToArray());
            var hour = int.Parse(timeParts[0]);
            var mins = int.Parse(timeParts[1]);
            
            Date = new DateTime(date.Year, date.Month, date.Day, hour, mins, 0);
            MovieCode = movieCode;
        }
    }
}
