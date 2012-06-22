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
using System.Xml.Serialization;
using Microsoft.Phone.Shell;
using System.IO;
using Konz.MyMovies.Core;
using Konz.MyMovies.Model;

namespace Konz.MyMovies.UI
{
    public partial class MovieList : PhoneApplicationPage
    {
        const string citySetting = "city";

        public MovieList()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var appState = (AppState)PhoneApplicationService.Current.State[SettingsConstants.AppState];
            DataContext = appState.City.Movies.OrderBy(x=>x.Title).ToList();
            PageTitle.Text = appState.City.Name;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var movie = (MovieInfo)e.AddedItems[0];
                NavigationService.Navigate(new Uri("/MovieDetail.xaml?m=" + movie.Code, UriKind.Relative));
            }
        }
    }    
}