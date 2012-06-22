using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Konz.MyMovies.Model
{
    public class CityInfo
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public List<MovieInfo> Movies { get; set; }
        public List<TheaterInfo> Theaters { get; set; }

        public CityInfo()
        {
            Movies = new List<MovieInfo>();
            Theaters = new List<TheaterInfo>();
        }
    }
}
