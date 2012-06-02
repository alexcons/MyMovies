using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konz.MyMovies.Core
{
    public class Room : EntityBase
    {
        /// <summary>
        /// Cinepolis Room Number
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Sort Order
        /// </summary>
        public int Order
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Room Features
        /// </summary>
        public List<RoomFeature> Features
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}
