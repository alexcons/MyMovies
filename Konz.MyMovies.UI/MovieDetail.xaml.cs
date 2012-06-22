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
using System.Windows.Navigation;
using Konz.MyMovies.Model;
using System.Windows.Media.Imaging;

namespace Konz.MyMovies.UI
{
    public partial class MovieDetail : PhoneApplicationPage
    {
        public MovieDetail()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var movieCode = NavigationContext.QueryString["m"];
            var appState = (AppState)PhoneApplicationService.Current.State[SettingsConstants.AppState];
            var movie = appState.City.Movies.Where(x => x.Code == movieCode).SingleOrDefault();
            var theaterList = new List<TheaterInfo>();
            foreach (var theater in appState.City.Theaters)
            {
                theater.Movies.Clear();
                theater.Movies.Add(new MovieInfo() {
                    Showtimes = theater.Showtimes.Where(x => x.MovieCode == movieCode).ToList()
                });
                if (theater.Movies[0].Showtimes.Count > 0)
                    theaterList.Add(theater);
            }
            txtSinopsis.Text = movie.Sinopsis;
            panMovie.Title = movie.Title;
            txtMovieTitle.Header = movie.Title;
            imgPoster.Source = new BitmapImage(new Uri(movie.PosterURI));
            lstShowtimes.ItemsSource = theaterList;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.LandscapeRight || e.Orientation == PageOrientation.LandscapeLeft)
            {
                txtSinopsis.SetValue(Grid.ColumnProperty, 1);
                txtSinopsis.SetValue(Grid.RowProperty, 0);
            }
            else
            {
                txtSinopsis.SetValue(Grid.ColumnProperty, 0);
                txtSinopsis.SetValue(Grid.RowProperty, 1);
            }
        }

    }
}