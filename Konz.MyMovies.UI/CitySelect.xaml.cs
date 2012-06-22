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
using System.Xml;
using System.IO.IsolatedStorage;

namespace Konz.MyMovies.UI
{
    public partial class CitySelect : PhoneApplicationPage
    {
        public CitySelect()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(SettingsConstants.CitiesState))
                ShowCities((List<City>)PhoneApplicationService.Current.State[SettingsConstants.CitiesState]);
            else
                PersistableFile<List<City>>.Load(SettingsConstants.CitiesStateFileName, CitiesLoadedFromFile);
        }

        private void CitiesLoadedFromFile(PersistableFile<List<City>> file, Exception error)
        {
            if (error == null)
                ShowCities(file.Data);
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
                MessageBox.Show("No hay connexion a internet. Por favor intente mas tarde.");
                NavigationService.GoBack();
            }
        }

        private void CitiesLoadedFromInternet(List<City> result)
        {
            if (result == null)
            {
                MessageBox.Show("No pudimos conectarnos al servidor de cinépolis. Por favor intente mas tarde.");
            }
            else
            {
                var file = new PersistableFile<List<City>>()
                {
                    FileName = SettingsConstants.CitiesStateFileName,
                    Data = result
                };

                file.Save(delegate(Exception ex)
                {
#if DEBUG
                    if (ex != null)
                        MessageBox.Show("El guardado del archivo local de ciudades falló.");
#endif
                });
                result = result.Where(x => x.CountryCode == "1").ToList();
                
                PhoneApplicationService.Current.State[SettingsConstants.CitiesState] = result;
                ShowCities(result);
            }
        }

        private void ShowCities(List<City> result)
        {
            DataContext = result;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                SettingsManager.City = e.AddedItems[0] as City;
                NavigationService.GoBack();
            }
        }
    }
}