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
using Konz.MyMovies.Core;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace Konz.MyMovies.UI
{
    public partial class MainPage : PhoneApplicationPage
    {
        City _currentCity;
        DateTime _currentDate;
        TimeSpan _startTime = TimeSpan.FromHours(9);

        List<Showtime> _allShows;
        Theater _currentTheater;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("SelectedCity"))
                _currentCity = (City)settings["SelectedCity"];
            else
                _currentCity = new City() { Code = "12", Name = "Culiacán" };

            if (settings.Contains("SelectedDate"))
                _currentDate = (DateTime)settings["SelectedDate"];
            else
                _currentDate = DateTime.Today;

            base.OnNavigatedTo(e);
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

            if (DateTime.Today == _currentDate)
                sldFromTime.Value = (DateTime.Now - _currentDate).TotalMinutes - _startTime.TotalMinutes;
            else
                sldFromTime.Value = 0;


            if (Utils.InternetIsAvailable())
            {
                var c = new DataExtractor();
                c.GetShows(_currentCity.Code, _currentDate, new Action<List<Showtime>>(LoadData));
            }
        }

        private void LoadData(List<Showtime> result)
        {
            _allShows = result;
            var theaters = _allShows.Select(x => x.Theater).Distinct().ToList();

            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("SelectedTheaterCode"))
                _currentTheater = theaters.Where(x => x.Code == (string)settings["SelectedTheaterCode"]).SingleOrDefault();
            
            _currentTheater = _currentTheater??theaters[0];
            
            DataContext = theaters;
            pvtTheaters.SelectedItem = _currentTheater;
        }

        private void RefreshShows(List<Showtime> shows)
        {
            if (_currentTheater == null)
                return;

            pvtTheaters.Title = _currentCity.Name;

            if (shows == null)
                shows = new List<Showtime>();

            var time = _startTime.Add(TimeSpan.FromMinutes(sldFromTime.Value));
            var fromTime = _currentDate.Add(time);
            txtTimeFrom.Text = fromTime.ToString("HH:mm");
            shows = shows.Where(x => x.Time > fromTime && x.Theater == _currentTheater).ToList();

            _currentTheater.Movies.Clear();
            foreach (var movie in shows.Select(x => x.Movie).Distinct())
            {
                movie.Shows.Clear();
                movie.Shows.AddRange(shows.Where(x => x.Movie == movie).ToList());
                _currentTheater.Movies.Add(movie);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshShows(_allShows);
        }

        private void pvtTheaters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentTheater = e.AddedItems[0] as Theater;
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("SelectedTheaterCode"))
                settings["SelectedTheaterCode"] = _currentTheater.Code;
            else
                settings.Add("SelectedTheaterCode", _currentTheater.Code);
            settings.Save();

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

            RefreshShows(_allShows);
        }

        private void MenuCityChange_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CitySelect.xaml", UriKind.Relative));
        }

        private void MenuComplexChange_Click(object sender, EventArgs e)
        {
            popSelectTheater.IsOpen = true;
            //lstTheaters.SelectedItem = _currentTheater;
        }

        private void Theater_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            popSelectTheater.IsOpen = false;
            pvtTheaters.SelectedItem = (Theater)e.AddedItems[0];
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (popSelectTheater.IsOpen)
            {
                popSelectTheater.IsOpen = false;
                e.Cancel = true;
            }
            else
                base.OnBackKeyPress(e);
        }

    }
}