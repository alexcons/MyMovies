using System.Collections.Generic;

namespace Konz.MyMovies.Model
{
    public class ShowtimeScheadule
    {
        public ShowtimeScheadule()
        {
            ShowtimeRooms = new List<ShowtimeRoom>();
        }

        public string MovieCode { get; set; }

        public string TheaterCode { get; set; }

        public string CityCode { get; set; }

        public string IsNew { get; set; }

        public string CityName { get; set; }

        public List<ShowtimeRoom> ShowtimeRooms { get; set; }
    }
}
