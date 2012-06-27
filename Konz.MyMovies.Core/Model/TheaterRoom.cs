using System.Collections.Generic;

namespace Konz.MyMovies.Model
{
    public class TheaterRoom
    {
        public TheaterRoom()
        {
            Features = new List<string>();
        }

        public string Code { get; set; }

        public string Orden { get; set; }

        public List<string> Features { get; set; }
    }
}
