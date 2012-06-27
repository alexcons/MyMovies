using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Konz.MyMovies.Core;
using Konz.MyMovies.Core.Cinepolis;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Konz.MyMovies.Model;

namespace Konz.MyMovies.UI
{
    public partial class CitySelect : PhoneApplicationPage
    {
        #region Constructor

        public CitySelect()
        {
            InitializeComponent();
            //ApplicationTitle.Text = Utils.GetMessage(Info.ChooseCityTitle);
            PageTitle.Text = Utils.GetMessage(Info.ChooseCity);
        }

        #endregion

        #region Form Events

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (SettingsManager.City == null)
            {
                while (NavigationService.CanGoBack)
                    NavigationService.RemoveBackEntry();                
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(Constants.CitiesState))
                ShowData((List<City>)PhoneApplicationService.Current.State[Constants.CitiesState]);
            else
                PersistableFile<List<City>>.Load(Constants.CitiesStateFileName, CitiesLoadedFromFile);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SettingsManager.City = e.AddedItems[0] as City;
                NavigationService.GoBack();
            }
        }

        #endregion

        #region Private Methods

        private void CitiesLoadedFromFile(PersistableFile<List<City>> file, Exception error)
        {
            if (error == null)
                ShowData(file.Data);
            else
                LoadCitiesFromInternet();
        }

        private void LoadCitiesFromInternet()
        {
            if (Utils.InternetIsAvailable())
            {
                var c = new DataExtractor();
                c.GetCities(new Action<List<City>>(CitiesLoadedFromInternet));
            }
            else
            {                
                MessageBox.Show(Utils.GetMessage(Error.NoInternetConnection));
                NavigationService.GoBack();
            }
        }

        private void CitiesLoadedFromInternet(List<City> result)
        {
            if (result == null || result.Count == 0)
                MessageBox.Show(Utils.GetMessage(Error.NoServerAvailable));
            else
            {
                result = result.Where(x => x.CountryCode == "1").ToList();
                
                var file = new PersistableFile<List<City>>()
                {
                    FileName = Constants.CitiesStateFileName,
                    Data = result
                };

                file.Save(delegate(Exception ex)
                {
#if DEBUG
                    if (ex != null)
                        MessageBox.Show(Utils.GetMessage(Error.CitiesFileNotSaved) + " : " + ex.Message);
#endif
                });
                
                PhoneApplicationService.Current.State[Constants.CitiesState] = file.Data;
                ShowData(file.Data);
            }
        }

        private void ShowData(List<City> result)
        {
            DataContext = result;
        }
 
        #endregion
    }
}