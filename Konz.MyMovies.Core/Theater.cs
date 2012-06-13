using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Konz.MyMovies.Core
{
    public class Theater : EntityBase
    {
        public string Code{ get; set; }

        public int Order { get; set; }

        [XmlIgnore]
        public List<Room> Rooms { get; set; }
        
        [XmlIgnore]
        public ObservableCollection<Movie> Movies { get; set; }

        public Theater()
        {
            Rooms = new List<Room>();
            Movies = new ObservableCollection<Movie>();
        }

    }
}
