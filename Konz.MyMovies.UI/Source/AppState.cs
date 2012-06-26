using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Konz.MyMovies.Model
{
    public class AppState
    {
        public DateTime Date { get; set; }
        public string TheaterCode { get; set; }
        public City City { get; set; }

        public static AppState Current { get; set; }

    }
}
