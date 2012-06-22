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
        public string _theaterCode;
        public string TheaterCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_theaterCode))
                {
                    if ((City != null) && City.Theaters.Count > 0)
                        _theaterCode = City.Theaters[0].Code;
                }
                return _theaterCode;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if ((City != null) && City.Theaters.Count > 0 && City.Theaters.Where(x => x.Code == value).Any())
                        _theaterCode = value;
                }
            }
        }
        public string MovieCode { get; set; }
        public CityInfo City { get; set; }
    }
}
