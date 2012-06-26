using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Konz.MyMovies.Core;
using Konz.MyMovies.Core.Cinepolis;
using Konz.MyMovies.Model;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading;
using System.ComponentModel;

namespace Konz.MyMovies.UI
{
    public partial class MainPage : PhoneApplicationPage
    {
        TimeSpan _startTime = TimeSpan.FromHours(9);

        #region Constructor

        public MainPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Navigation Events

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
            
            if (SettingsManager.City == null)
            {
                NavigationService.Navigate(new Uri("/CitySelect.xaml", UriKind.Relative));
                return;
            }

            var dateList = new List<string>();
            var date = DateTime.Today;
            while (date < DateTime.Today.AddDays(5))
            {
                dateList.Add(Utils.GetLongDate(date));
                date = date.AddDays(1);
            }
            lstDates.ItemsSource = dateList;

            ShowLoading();

            GetAppState();

            base.OnNavigatedTo(e);
        }
        
        #endregion

        #region Private Methods

        private void GetAppState()
        {            
            var state = AppState.Current;

            if (state != null && state.City.Code == SettingsManager.City.Code && state.Date == SettingsManager.CurrentDate)
            {
                ShowData(state);
                return;
            }

            LoadFromInternet();
        }

        private void LoadFromInternet()
        {
            if (Utils.InternetIsAvailable())
                new DataExtractor().GetCityData(SettingsManager.City, SettingsManager.CurrentDate, new Action<City>(DataLoadedFromInternet));
            else
            {
                MessageBox.Show(Utils.GetMessage(Error.NoInternetConnection));
                ShowData(null);
            }
        }

        private void DataLoadedFromInternet(City result)
        {
            AppState state = null;
            if (result == null)
                MessageBox.Show(Utils.GetMessage(Error.NoServerAvailable));
            else
                state = new AppState() { City = result, Date = SettingsManager.CurrentDate, TheaterCode = SettingsManager.TheaterCode };
            
            ShowData(state);
        }

        private void ShowData(AppState state)
        {
            if (state == null)
            {
                NoDataLoaded();
                return;
            }

            AppState.Current = state;

            pvtTheaters.Title = string.Format("{0} ({1})", AppState.Current.City.Name, Utils.GetLongDate(AppState.Current.Date));

            //TODO: Check if this doe not trigger a selection change event
            //pvtTheaters.SelectedIndex = -1;
            DataContext = AppState.Current.City.Theaters;
            var theater = AppState.Current.City.Theaters.Where(x => x.Code == AppState.Current.TheaterCode).SingleOrDefault();
            if (theater != null) pvtTheaters.SelectedItem = theater;

            if (AppState.Current.Date == DateTime.Today)
                sldFromTime.Value = (DateTime.Now - AppState.Current.Date).TotalMinutes - _startTime.TotalMinutes;
            else
                sldFromTime.Value = 0;

            RefreshShows();
        }

        private void NoDataLoaded()
        {
            if (App.Current == null)
            {
                loadingPop.Message = "No hay datos, selecciona refrescar";
                loadingPop.IsLoading = false;
            }
            else
                HideLoading();
        }

        private void RefreshShows()
        {
            var state = AppState.Current;
            if (state == null)
                return;

            DateTime fromTime = sldFromTime.Value == 0 ? state.Date: state.Date.Add(_startTime.Add(TimeSpan.FromMinutes(sldFromTime.Value)));
            var theaterCode = state.TheaterCode;
            theaterCode = theaterCode ?? state.City.Theaters[0].Code;
            var theater = state.City.Theaters.Where(x => x.Code == theaterCode).SingleOrDefault();
            var showtimes = theater.Showtimes.Where(x => x.Date > fromTime).ToList();
            var movieCodes = showtimes.Select(x => x.MovieCode).Distinct().ToList();
            var movies = state.City.Movies.Where(x=>movieCodes.Contains(x.Code));

            foreach (var movie in movies)
                movie.Showtimes = showtimes.Where(x => x.MovieCode == movie.Code).ToList();

            theater.Movies.Clear();
            foreach (var item in movies.Where(x => x.Showtimes.Count > 0).OrderBy(x => x.NextShow))
                theater.Movies.Add(item);

            HideLoading();
        }

        private void ShowLoading()
        {
            loadingPop.Message = "Cargando...";
            loadingPop.IsLoading = true;
            loadingPop.Visibility = Visibility.Visible;
        }

        private void HideLoading()
        {
            loadingPop.Visibility = Visibility.Collapsed;
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
                popUp.Height = 290;
                popUp.Width = 556;
            }
        }

        #endregion

        #region UI Events

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AppState.Current == null)
                return;

            if (sldFromTime.Value == 0)
            {
                var fromTime = AppState.Current.Date;
                txtTimeFrom.Text = Utils.GetMessage(Info.AnyTime);
            }
            else
            {
                var time = _startTime.Add(TimeSpan.FromMinutes(sldFromTime.Value));
                var fromTime = AppState.Current.Date.Add(time);
                txtTimeFrom.Text = Utils.GetTimeFormat(fromTime);
            }
        }

        private void sldFromTime_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            RefreshShows();
        }

        private void pvtTheaters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppState.Current == null)
                return;
            if (e.AddedItems.Count > 0)
            {
                ShowLoading();
                var code = (e.AddedItems[0] as Theater).Code;
                lstTheaters.SelectedIndex = -1;
                AppState.Current.TheaterCode = code;
                //SettingsManager.TheaterCode = code;
                RefreshShows();
            }
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            var orientation = PageOrientation.Portrait;
            if (e.Orientation == PageOrientation.LandscapeRight || e.Orientation == PageOrientation.LandscapeLeft)
                orientation = PageOrientation.Landscape;

            SetPopUpSize(popSelectDate, orientation);
            SetPopUpSize(popSelectTheater, orientation);

        }

        #region Pop Up Selections

        private void Movies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var movie = (Movie)e.AddedItems[0];
                NavigationService.Navigate(new Uri("/MovieDetail.xaml?m=" + movie.Code, UriKind.Relative));
            }
        }

        private void Theaters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                pvtTheaters.SelectedItem = (Theater)e.AddedItems[0];
                popSelectTheater.IsOpen = false;
            }
        }

        private void Dates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ShowLoading();
                SettingsManager.CurrentDate = Utils.ParseLongDate((string)e.AddedItems[0]);
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
            if (Utils.InternetIsAvailable())
                NavigationService.Navigate(new Uri("/CitySelect.xaml", UriKind.Relative));
            else
                MessageBox.Show(Utils.GetMessage(Error.NoInternetConnection));
        }

        private void MenuDateChange_Click(object sender, EventArgs e)
        {
            if (Utils.InternetIsAvailable())
                popSelectDate.IsOpen = true;
            else
                MessageBox.Show(Utils.GetMessage(Error.NoInternetConnection));            
        }

        private void MenuTheaterChange_Click(object sender, EventArgs e)
        {
            if (AppState.Current != null && AppState.Current.City.Theaters.Count > 1)
                popSelectTheater.IsOpen = true;
        }

        private void MenuRefresh_Click(object sender, EventArgs e)
        {
            ShowLoading();
            LoadFromInternet();
        }

        private void MenuMovies_Click(object sender, EventArgs e)
        {
            if (AppState.Current != null && AppState.Current.City.Movies.Count > 0)
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

        private void MenuToggleInternet_Click(object sender, EventArgs e)
        {
            SettingsManager.Internet = !SettingsManager.Internet;
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