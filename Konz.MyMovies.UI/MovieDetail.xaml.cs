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
using System.Windows;

namespace Konz.MyMovies.UI
{
    public partial class MovieDetail : PhoneApplicationPage
    {

        private Movie _movie;
        private string _defaultMessage = Utils.GetMessage(Info.GetDefaultShareMessage);

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
            _movie = AppState.Current.City.Movies.Where(x => x.Code == movieCode).SingleOrDefault();
            
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

            ShowData(_movie, theaterList);

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
            var orientation = PageOrientation.Portrait;
            if (e.Orientation == PageOrientation.LandscapeRight || e.Orientation == PageOrientation.LandscapeLeft)
                orientation = PageOrientation.Landscape;

            if (orientation == PageOrientation.Landscape)
            {
                txtSinopsis.SetValue(Grid.ColumnProperty, 1);
                txtSinopsis.SetValue(Grid.RowProperty, 0);
            }
            else
            {
                txtSinopsis.SetValue(Grid.ColumnProperty, 0);
                txtSinopsis.SetValue(Grid.RowProperty, 1);
            }

            SetPopUpSize(popFacebookShare, orientation);
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

        private void btnShare_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SettingsManager.FacebookActive)
            {
                txtMessage.Text = _defaultMessage;
                popFacebookShare.IsOpen = true;
            }
            else
                if (MessageBox.Show(Utils.GetMessage(Info.FacebookIntegration), Utils.GetMessage(Info.FacebookIntegrationTitle), MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    NavigationService.Navigate(new Uri("/FacebookLoginPage.xaml", UriKind.Relative));
        }

        private void PostToSocial(string message)
        {
            if (SettingsManager.FacebookActive)
            {
                var fb = new FacebookManager(SettingsManager.FacebookToken);
                fb.OnComplete = (args =>
                {
                    if (args.Error == null)
                    {
                        Dispatcher.BeginInvoke(() => MessageBox.Show("Ya esta en tu muro!"));
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                        SettingsManager.FacebookActive = false;
                    }
                });
                fb.Share(_movie, message);
            }
        }

        private void btnPostToSocial_Click(object sender, RoutedEventArgs e)
        {
            popFacebookShare.IsOpen = false;
            PostToSocial(txtMessage.Text);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (popFacebookShare.IsOpen)
            {
                popFacebookShare.IsOpen = false;
                e.Cancel = true;
            }
            else
                base.OnBackKeyPress(e);
        }

        private void SetPopUpSize(System.Windows.Controls.Primitives.Popup popUp, PageOrientation orientation)
        {
            if (orientation == PageOrientation.Portrait)
            {
                popUp.Height = 506;
                popUp.Width = 380;
            }
            else
            {
                popUp.Height = 320;
                popUp.Width = 700;
            }
        }

        private void txtMessage_GotFocus(object sender, RoutedEventArgs e)
        {
            txtMessage.SelectAll();
        }

        private void btnTrailer_Click(object sender, RoutedEventArgs e)
        {
            YouTubeManager.Search(_movie.CleanTitle);
        }
    }
}