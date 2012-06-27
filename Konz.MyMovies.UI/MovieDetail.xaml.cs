using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Konz.MyMovies.Model;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Konz.MyMovies.Core;

namespace Konz.MyMovies.UI
{
    public partial class MovieDetail : PhoneApplicationPage
    {
        #region Constructor

        public MovieDetail()
        {
            InitializeComponent();
        }

        #endregion

        #region Form Events

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var movieCode = NavigationContext.QueryString["m"];
            var movie = AppState.Current.City.Movies.Where(x => x.Code == movieCode).SingleOrDefault();
            
            var theaterList = new List<Theater>();
            foreach (var theater in AppState.Current.City.Theaters)
            {
                theater.Movies.Clear();
                theater.Movies.Add(new Movie() {
                    Showtimes = theater.Showtimes.Where(x => x.MovieCode == movieCode).ToList()
                });
                if (theater.Movies[0].Showtimes.Count > 0)
                    theaterList.Add(theater);
            }

            ShowData(movie, theaterList);

            base.OnNavigatedTo(e);
        }

        private void ShowData(Movie movie, List<Theater> theaterList)
        {
            txtScheadulesTitle.Header = Utils.GetMessage(Info.Sheadules);
            txtMovieTitle.Header = movie.Title;
            
            txtSinopsis.Text = movie.Sinopsis;
            panMovie.Title = movie.Title;
            imgPoster.Source = movie.Poster;
            lstShowtimes.ItemsSource = theaterList;
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

        #endregion

        private void lstShowtimes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var theater = (Theater)e.AddedItems[0];
                AppState.Current.TheaterCode = theater.Code;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

    }
}