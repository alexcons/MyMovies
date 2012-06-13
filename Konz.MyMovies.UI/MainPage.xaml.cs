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

namespace Konz.MyMovies.UI
{
    public partial class MainPage : PhoneApplicationPage
    {
        const string showsFileName = "shows.xml";
        const string citySetting = "city";
        const string cityDirty = "cityDirty";
        const int expirationDays = 1;


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
            _currentCity = (City) SettingsManager.LoadSetting(citySetting) ?? new City() { Code = "12", Name = "Culiacán" };
            _currentTheaterCode = (string)SettingsManager.LoadSetting("theater");
            _currentDate = DateTime.Today;
            var isCityDirty = (bool?)SettingsManager.LoadSetting(cityDirty) ?? true;


            if (PhoneApplicationService.Current.State.ContainsKey("allshows") && !isCityDirty)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CityData));
                var showsString = (string)PhoneApplicationService.Current.State["allshows"];
                var sr = new StringReader(showsString);
                _allShows = ((CityData)serializer.Deserialize(sr)).GetShows();
            }

            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

            if (DateTime.Today == _currentDate)
                sldFromTime.Value = (DateTime.Now - _currentDate).TotalMinutes - _startTime.TotalMinutes;
            else
                sldFromTime.Value = 0;

            if (_allShows != null)
                ShowData(_allShows);
            else
                PersistableFile<CityData>.Load(showsFileName, new Action<PersistableFile<CityData>, Exception>(ShowsLoadedFromFile));
            
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            StringWriter sr = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(CityData));
            serializer.Serialize(sr, CityData.GetCityDataFromShows(_currentCity.Code, _allShows));
            PhoneApplicationService.Current.State["allshows"] = sr.ToString();
            sr.Close();
            SettingsManager.SaveSetting(cityDirty, false);
            base.OnNavigatedFrom(e);
        }

        private void ShowsLoadedFromFile(PersistableFile<CityData> file, Exception error)
        {
            if (error == null && file != null && file.ExpirationDate > DateTime.Now && file.Data.CityCode == _currentCity.Code)
                ShowData(file.Data.GetShows());
            else
            {
                LoadFromInternet();
            }
        }

        private void LoadFromInternet()
        {
            if (Utils.InternetIsAvailable())
            {
                var c = new DataExtractor();
                c.GetShows(_currentCity.Code, _currentDate, new Action<DateTime, List<Showtime>>(ShowsLoadedFromInternet));
            }
            else
            {
                MessageBox.Show("No hay connexion a internet. Por favor intente mas tarde.");
                //NavigationService.GoBack();
            }
        }

        private void ShowsLoadedFromInternet(DateTime expiresOn, List<Showtime> result)
        {
            if (result == null)
                MessageBox.Show("No pudimos conectarnos al servidor de cinépolis. Por favor intente mas tarde.");
            else
            {
                var file = new PersistableFile<CityData>()
                {
                    CreationDate = DateTime.Now,
                    ExpirationDate = expiresOn,
                    FileName = showsFileName,
                    Data = CityData.GetCityDataFromShows(_currentCity.Code, result)
                };

                file.Save(delegate(Exception ex)
                {
#if DEBUG
                    if (ex != null)
                        MessageBox.Show("El guardado del archivo local de funciones falló: " + ex.Message);
#endif
                });
                ShowData(result);
            }
        }

        private void ShowData(List<Showtime> result)
        {
            _allShows = result;

            var theaters = _allShows.Select(x => x.Theater).Distinct().ToList();
            
            if (_currentTheaterCode != null)
                _currentTheater = theaters.Where(x => x.Code == _currentTheaterCode).SingleOrDefault() ?? theaters[0];
            else
                _currentTheater = theaters[0];

            DataContext = theaters;
            pvtTheaters.SelectedItem = _currentTheater;
            
            RefreshShows(result);
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
            txtTimeFrom.Text = fromTime.ToString("h:mmtt");
        }

        private void pvtTheaters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _currentTheater = e.AddedItems[0] as Theater;
                SettingsManager.SaveSetting("theater", _currentTheater.Code);
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

        private void MenuRefresh_Click(object sender, EventArgs e)
        {
            LoadFromInternet();
        }
    }
}