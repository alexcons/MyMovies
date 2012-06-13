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
        const string citiesFileName = "cities.xml";
        const string citySetting = "city";
        const string cityDirty = "cityDirty";
        const int expirationDays = 10;

        public CitySelect()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("cities"))
                ShowData((List<City>)PhoneApplicationService.Current.State["cities"]);
            else
                PersistableFile<List<City>>.Load(citiesFileName, CitiesLoadedFromFile);
        }

        private void CitiesLoadedFromFile(PersistableFile<List<City>> file, Exception error)
        {
            if (error == null && file != null && file.ExpirationDate > DateTime.Now)
            {
                ShowData(file.Data);
            }
            else
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
                    CreationDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddDays(expirationDays),
                    FileName = citiesFileName,
                    Data = result
                };

                file.Save(delegate(Exception ex)
                {
#if DEBUG
                    if (ex != null)
                        MessageBox.Show("El guardado del archivo local de ciudades falló.");
#endif
                });

                ShowData(result);
            }
        }

        private void ShowData(List<City> result)
        {
            PhoneApplicationService.Current.State["cities"] = result;
            DataContext = result.Where(x => x.CountryCode == "1").ToArray();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var newCity = e.AddedItems[0] as City;
                SettingsManager.SaveSetting(citySetting, newCity);
                SettingsManager.SaveSetting(cityDirty, true);
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}