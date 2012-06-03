using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Konz.MyMovies.Core
{
    public class Theater : EntityBase
    {
        /// <summary>
        /// Cinepolis Theater Code
        /// </summary>
        public string Code{ get; set; }

        /// <summary>
        /// Sort Order
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Theater Rooms
        /// </summary>
        public List<Room> Rooms { get; set; }

        public Theater()
        {
            Rooms = new List<Room>();
            Movies = new ObservableCollection<Movie>();
        }

        public ObservableCollection<Movie> Movies { get; set; }
    }
}
