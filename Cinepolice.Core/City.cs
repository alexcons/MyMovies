using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konz.MyMovies.Core
{
    public class City : EntityBase
    {
        public List<Theater> Theaters {get; set;}

        /// <summary>
        /// Cinepolis Code
        /// </summary>
        public string Code {get; set;}

        public int Order {get; set;}

        public City()
        {
            Theaters = new List<Theater>();
        }

        public string CountryCode { get; set; }
    }
}
