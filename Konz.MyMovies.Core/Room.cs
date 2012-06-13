using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konz.MyMovies.Core
{
    public class Room : EntityBase
    {
        /// <summary>
        /// Room Code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Sort Order
        /// </summary>
        public int Order{ get; set; }

        /// <summary>
        /// Room Features
        /// </summary>
        public List<RoomFeature> Features{ get; set; }

        public Room()
        {
            Features = new List<RoomFeature>();
        }

    }
}
