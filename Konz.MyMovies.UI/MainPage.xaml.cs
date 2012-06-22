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
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using Konz.MyMovies.Model;

namespace Konz.MyMovies.UI
{
    public partial class MainPage : PhoneApplicationPage
    {
        AppState _appState;
        TimeSpan _startTime = TimeSpan.FromHours(9);

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        #region Navigation Events

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

            var dateList = new List<string>();
            var date = DateTime.Today;
            while (date < DateTime.Today.AddDays(7))
            {
                dateList.Add(date.ToString("dddd dd MMMM", new CultureInfo("es-MX")));
                date = date.AddDays(1);
            }
            lstDates.ItemsSource = dateList;

            GetAppState(new Action<AppState>(RefreshState));

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            PhoneApplicationService.Current.State[SettingsConstants.AppState] = _appState;
        }
        
        #endregion

        #region Private Methods

        private void GetAppState(Action<AppState> action)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(SettingsConstants.AppState))
            {
                var state = (AppState)PhoneApplicationService.Current.State[SettingsConstants.AppState];
                if (state.City.Code == SettingsManager.City.Code && state.Date == SettingsManager.CurrentDate)
                {
                    action(state);
                    return;
                }
            }

            PersistableFile<AppState>.Load(SettingsConstants.AppStateFileName, new Action<PersistableFile<AppState>, Exception>(StateLoadedFromFile));
        }

        private void StateLoadedFromFile(PersistableFile<AppState> file, Exception error)
        {
            if (error == null && file.Data.Date == SettingsManager.CurrentDate && file.Data.City.Code == SettingsManager.City.Code)
                RefreshState(file.Data);
            else
                LoadFromInternet();
        }

        private void LoadFromInternet()
        {
            if (Utils.InternetIsAvailable())
                new DataExtractor().GetShows(SettingsManager.City, SettingsManager.CurrentDate, new Action<CinepolisData>(StateLoadedFromInternet));
            else
            {
                MessageBox.Show("No hay connexion a internet. Por favor intente mas tarde.");
                RefreshState(null);
            }
        }

        private void StateLoadedFromInternet(CinepolisData result)
        {
            if (result == null)
                MessageBox.Show("No pudimos conectarnos al servidor de cinépolis. Por favor intente mas tarde.");
            else
            {
                var state = GetAppState(result);

                var file = new PersistableFile<AppState>()
                {
                    FileName = SettingsConstants.AppStateFileName,
                    Data = state
                };

                file.Save(delegate(Exception ex)
                {
#if DEBUG
                    if (ex != null)
                        MessageBox.Show("El guardado del archivo local de funciones falló: " + ex.Message);
#endif
                });

                RefreshState(state);
            }
        }

        private AppState GetAppState(CinepolisData result)
        {
            var state = new AppState();
            state.City = new CityInfo()
            {
                Code = result.Ciudad.Code,
                Name = result.Ciudad.Name
            };

            state.Date = result.Vigencia.de;

            state.City.Movies = (from d in result.Peliculas
                                 select new MovieInfo()
                                 {
                                     Code = d.Code,
                                     PosterURI = string.Format("http://www.cinepolis.com.mx/imagenes/peliculas/{0}", d.Cartel),
                                     Sinopsis = d.Sinopsis,
                                     Title = d.Nombre
                                 }).ToList();
            state.City.Theaters = (from d in result.Complejos
                                   select new TheaterInfo()
                                   {
                                       Code = d.Code,
                                       Name = d.Nombre
                                   }).ToList();

            var date = state.Date;
            foreach (var cartelera in result.Carteleras)
            {
                var theater = state.City.Theaters.Where(x => x.Code == cartelera.ComplejoCode).SingleOrDefault();
                
                //cinepolis bug
                if (theater == null)
                    continue;

                foreach (var sala in cartelera.Salas)
                {
                    foreach (var h in sala.Horarios)
                    {
                        var st = new ShowtimeInfo();
                        st.MovieCode = cartelera.PeliculaCode;
                        var timeParts = h.Split(":".ToArray());
                        var hr = int.Parse(timeParts[0]);
                        var mn = int.Parse(timeParts[1]);
                        st.Date = new DateTime(date.Year, date.Month, date.Day, hr, mn, 00);
                        theater.Showtimes.Add(st);
                    }
                }
            }
            return state;
        }

        private void RefreshState(AppState state)
        {
            _appState = state;
            if (state == null)
                state = GetDefaultState();
            
            state.TheaterCode = SettingsManager.TheaterCode;

            pvtTheaters.Title = string.Format("{0} ({1})", state.City.Name, state.Date.ToString("dddd dd MMMM", new CultureInfo("es-MX")));
            DataContext = state.City.Theaters;
            var theater = state.City.Theaters.Where(x => x.Code == state.TheaterCode).SingleOrDefault();
            pvtTheaters.SelectedItem = theater;

            if (DateTime.Today == state.Date)
                sldFromTime.Value = (DateTime.Now - state.Date).TotalMinutes - _startTime.TotalMinutes;
            else
                sldFromTime.Value = 0;

            RefreshShows(state);
        }

        private void RefreshShows(AppState state)
        {
            DateTime fromTime;
            if (sldFromTime.Value == 0)
                fromTime = state.Date;
            else
                fromTime = state.Date.Add(_startTime.Add(TimeSpan.FromMinutes(sldFromTime.Value)));

            var theater = state.City.Theaters.Where(x => x.Code == state.TheaterCode).SingleOrDefault();
            var showtimes = theater.Showtimes.Where(x => x.Date > fromTime).ToList();
            var movieCodes = showtimes.Select(y => y.MovieCode).Distinct().ToList();

            //var movies = state.City.Movies.Where(x=>movieCodes.Contains(x.Code));
            var movies = new List<MovieInfo>();
            foreach (var movieCode in movieCodes)
            {
                var movie = state.City.Movies.Where(x => x.Code == movieCode).SingleOrDefault();
                if (movie != null)
                    movies.Add(movie);
            }
            
            foreach (var movie in movies)
            {
                movie.Showtimes.Clear();
                var movieShowtimes = showtimes.Where(x => x.MovieCode == movie.Code).ToList();
                movie.Showtimes.AddRange(movieShowtimes);
            }

            theater.Movies.Clear();
            foreach (var m in movies.OrderBy(x => x.NextShow))
                theater.Movies.Add(m);
        }

        private AppState GetDefaultState()
        {
            var state = new AppState()
            {
                City = new CityInfo
                {
                    Name = "No City Loaded",
                    Theaters = new List<TheaterInfo>()
                },
                Date = DateTime.Today,
                MovieCode = null,
                TheaterCode = null
            };

            return state;
        }

        #endregion

        #region UI Events

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sldFromTime.Value == 0)
            {
                var fromTime = _appState.Date;
                txtTimeFrom.Text = "Anytime";
            }
            else
            {
                var time = _startTime.Add(TimeSpan.FromMinutes(sldFromTime.Value));
                var fromTime = _appState.Date.Add(time);
                txtTimeFrom.Text = fromTime.ToString("h:mmtt");
            }
        }

        private void sldFromTime_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            RefreshShows(_appState);
        }

        private void pvtTheaters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var code = (e.AddedItems[0] as TheaterInfo).Code;
                _appState.TheaterCode = code;
                SettingsManager.TheaterCode = code;
                RefreshShows(_appState);
            }
        }

        #region Pop Up Selections
        
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var movie = (MovieInfo)e.AddedItems[0];
                NavigationService.Navigate(new Uri("/MovieDetail.xaml?m=" + movie.Code, UriKind.Relative));
            }
        }

        private void Theater_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                pvtTheaters.SelectedItem = (TheaterInfo)e.AddedItems[0];
                popSelectTheater.IsOpen = false;
            }
        }

        private void Dates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SettingsManager.CurrentDate = DateTime.ParseExact((string)e.AddedItems[0], "dddd dd MMMM", new CultureInfo("es-MX"));
                sldFromTime.Value = 0;
                popSelectDate.IsOpen = false;
                LoadFromInternet();
            }
        }
        
        #endregion

        #endregion

        #region AppBar Menu Events

        private void MenuCityChange_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CitySelect.xaml", UriKind.Relative));
        }

        private void MenuDateChange_Click(object sender, EventArgs e)
        {
            popSelectDate.IsOpen = true;
        }

        private void MenuTheaterChange_Click(object sender, EventArgs e)
        {
            popSelectTheater.IsOpen = true;
        }

        private void MenuRefresh_Click(object sender, EventArgs e)
        {
            LoadFromInternet();
        }

        private void MenuMovies_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MovieList.xaml", UriKind.Relative));
        }

        private void MenuClearData_Click(object sender, EventArgs e)
        {
            using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                foreach (var item in myIsolatedStorage.GetFileNames())
                    myIsolatedStorage.DeleteFile(item);
            }
        }
        
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (popSelectTheater.IsOpen || popSelectDate.IsOpen)
            {
                popSelectTheater.IsOpen = false;
                popSelectDate.IsOpen = false;
                e.Cancel = true;
            }
            else
                base.OnBackKeyPress(e);
        }

        #endregion
    }
}