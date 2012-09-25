using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konz.MyMovies.Model
{
    public class FacebookResult
    {
        public Exception Error { get; set; }
        public  IDictionary<string,object> Result { get; set; }
    }
}
