using System;
using System.IO.IsolatedStorage;
using Konz.MyMovies.Core;
using Konz.MyMovies.Model;

namespace Konz.MyMovies.Core
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
                    return null;
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
                {
                    var date = (DateTime)LoadSetting(settingName);
                    date = date < DateTime.Today ? DateTime.Today : date;
                    return date;
                }
                else
                    return DateTime.Today;
            }
            set
            {
                var settingName = "date";
                SaveSetting(settingName, value);
            }
        }

        public static bool Internet
        {
            get
            {
                var settingName = "internet";
                if (ExistsSetting(settingName))
                    return (bool)LoadSetting(settingName);
                else
                    return true;
            }
            set
            {
                var settingName = "internet";
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
