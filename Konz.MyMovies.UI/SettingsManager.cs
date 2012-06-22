using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using Konz.MyMovies.Core;

namespace Konz.MyMovies.UI
{
    public class SettingsManager
    {
        private static IsolatedStorageSettings _settings = IsolatedStorageSettings.ApplicationSettings;

        public static City City
        {
            get
            {
                var settingName = "city";
                if (ExistsSetting(settingName))
                    return (City)LoadSetting(settingName);
                else
                {
                    var city = new City()
                    {
                        Code = "12",
                        Name = "Culiacán"
                    };
                    SaveSetting(settingName, city);
                    return city;
                }
            }
            set
            {
                var settingName = "city";
                SaveSetting(settingName, value);
            }
        }

        public static string TheaterCode
        {
            get
            {
                var settingName = "theater";
                if (ExistsSetting(settingName))
                    return (String)LoadSetting(settingName);
                else
                    return null;
            }
            set
            {
                var settingName = "theater";
                SaveSetting(settingName, value);
            }
        }
        
        public static DateTime CurrentDate
        {
            get
            {
                var settingName = "date";
                if (ExistsSetting(settingName))
                    return (DateTime)LoadSetting(settingName);
                else
                    return DateTime.Today;
            }
            set
            {
                var settingName = "date";
                SaveSetting(settingName, value);
            }
        }

        public static bool ExistsSetting(string name)
        {
            return _settings.Contains(name);
        }

        public static void SaveSetting(string name, object value)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings[name] = value;
            settings.Save();
        }

        public static object LoadSetting(string name)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(name))
                return settings[name];
            return null;
        }

    }
}
