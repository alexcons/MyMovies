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
    public class City
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string CountryCode { get; set; }
        public List<Movie> Movies { get; set; }
        public List<Theater> Theaters { get; set; }

        public City()
        {
            Movies = new List<Movie>();
            Theaters = new List<Theater>();
        }
    }
}
