using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konz.MyMovies.Core
{
    public class City : EntityBase
    {
        public string Code {get; set;}

        public string CountryCode { get; set; }

        public int Order { get; set; }
    }
}
