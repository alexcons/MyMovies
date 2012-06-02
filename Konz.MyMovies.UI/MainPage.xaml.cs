using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Konz.MyMovies.Core.Cinepolis;

namespace Konz.MyMovies.UI
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            var c = new DataExtractor();
            c.GetData("12", DateTime.Today, new Action<CinepolisData>(ShowData));
        }

        private void ShowData(CinepolisData result)
        {
            var shows = result.GetShowTimes(result);
            foreach (var show in shows)
            {
                var tb = new TextBlock();
                tb.Text = string.Format("{1} - {2}", show.Movie.Name, show.Time.ToString());
                stkConsole.Children.Add(tb);                
            }
        }

    }
}