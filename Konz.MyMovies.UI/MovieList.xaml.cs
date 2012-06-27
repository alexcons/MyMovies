using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using Konz.MyMovies.Model;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Konz.MyMovies.Core;

namespace Konz.MyMovies.UI
{
    public partial class MovieList : PhoneApplicationPage
    {
        public MovieList()
        {
            InitializeComponent();
        }

        #region Form Events

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            DataContext = AppState.Current.City.Movies.OrderBy(x => x.Title).ToList();
            ApplicationTitle.Text = Utils.GetLongDate(AppState.Current.Date);
            PageTitle.Text = AppState.Current.City.Name;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var movie = (Movie)e.AddedItems[0];
                NavigationService.Navigate(new Uri("/MovieDetail.xaml?m=" + movie.Code, UriKind.Relative));
            }
        }

        #endregion
    }    
}