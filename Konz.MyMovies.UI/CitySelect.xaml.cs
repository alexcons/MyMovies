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
    public partial class CitySelect : PhoneApplicationPage
    {
        public CitySelect()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("cities"))
            {
                var cities = (List<City>)PhoneApplicationService.Current.State["cities"];
                ShowData(cities);
            }
            else
            {
                if (Utils.InternetIsAvailable())
                {
                    var c = new DataExtractor();
                    c.GetCities(new Action<List<City>>(ShowData));
                }
            }                
        }

        private void ShowData(List<City> result)
        {
            PhoneApplicationService.Current.State["cities"] = result;
            DataContext = result.Where(x=>x.CountryCode=="1").ToArray();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var city = e.AddedItems[0] as City;
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains("SelectedCity"))
                    settings["SelectedCity"] = city;
                else
                    settings.Add("SelectedCity", city);
                settings.Save();
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }                
        }
    }
}