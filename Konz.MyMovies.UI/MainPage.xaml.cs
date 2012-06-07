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
        List<Showtime> _allShows;

        City _currentCity;
        Theater _currentTheater;
        DateTime _currentDate;
        
        string _currentTheaterCode;
        TimeSpan _startTime = TimeSpan.FromHours(9);
        

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("city"))
                _currentCity = (City)settings["city"];
            else
                _currentCity = new City() { Code = "12", Name = "Culiacán" };

            if (settings.Contains("date"))
                _currentDate = (DateTime)settings["date"];
            else
                _currentDate = DateTime.Today;
            
            if (settings.Contains("theater"))
                _currentTheaterCode = (string)settings["theater"];
            else
                _currentTheaterCode = null;

            if (PhoneApplicationService.Current.State.ContainsKey("allshows"))
                _allShows = (List<Showtime>)PhoneApplicationService.Current.State["allshows"];

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            PhoneApplicationService.Current.State["allshows"] = _allShows;
            base.OnNavigatedFrom(e);
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

            if (DateTime.Today == _currentDate)
                sldFromTime.Value = (DateTime.Now - _currentDate).TotalMinutes - _startTime.TotalMinutes;
            else
                sldFromTime.Value = 0;

            if (_allShows != null)
            {
                LoadData(_allShows);
                return;
            }

            if (Utils.InternetIsAvailable())
            {
                var c = new DataExtractor();
                c.GetShows(_currentCity.Code, _currentDate, new Action<List<Showtime>>(LoadData));
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        private void LoadData(List<Showtime> result)
        {
            _allShows = result;
            var allTheaters = _allShows.Select(x => x.Theater).Distinct().ToList();
            if (_currentTheaterCode != null)
                _currentTheater = allTheaters.Where(x => x.Code == _currentTheaterCode).SingleOrDefault() ?? allTheaters[0];
            else
                _currentTheater = allTheaters[0];

            DataContext = allTheaters;
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

            shows = shows.Where(x => x.Time > fromTime && x.Theater == _currentTheater).ToList();

            var theaterMovies = new List<Movie>();
            foreach (var movie in shows.Select(x => x.Movie).Distinct())
            {
                movie.Shows.Clear();
                movie.Shows.AddRange(shows.Where(x => x.Movie == movie).ToList());
                theaterMovies.Add(movie);
            }
            
            _currentTheater.Movies.Clear();
            foreach (var movie in theaterMovies.OrderBy(x=>x.NextShow.Time))
                _currentTheater.Movies.Add(movie);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var time = _startTime.Add(TimeSpan.FromMinutes(sldFromTime.Value));
            var fromTime = _currentDate.Add(time);
            txtTimeFrom.Text = fromTime.ToString("HH:mm");
        }

        private void pvtTheaters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _currentTheater = e.AddedItems[0] as Theater;
                
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (!settings.Contains("theater"))
                    settings.Add("theater", _currentTheater.Code);
                else
                    settings["theater"] = _currentTheater.Code;
                settings.Save();

                RefreshShows(_allShows);
            }
        }

        private void MenuCityChange_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CitySelect.xaml", UriKind.Relative));
        }

        private void MenuComplexChange_Click(object sender, EventArgs e)
        {
            popSelectTheater.IsOpen = true;
        }

        private void Theater_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                pvtTheaters.SelectedItem = (Theater)e.AddedItems[0];
                popSelectTheater.IsOpen = false;
            }
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

        private void sldFromTime_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            RefreshShows(_allShows);
        }
    }
}