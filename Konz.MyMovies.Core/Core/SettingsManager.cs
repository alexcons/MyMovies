﻿using System;
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
                return (City)GetSetting(settingName);
            }
            set
            {
                var settingName = "city";
                _settings[settingName] = value;
            }
        }

        public static string TheaterCode
        {
            get
            {
                var settingName = "theater";
                return (String)GetSetting(settingName);
            }
            set
            {
                var settingName = "theater";
                _settings[settingName] = value;
            }
        }
        
        public static DateTime CurrentDate
        {
            get
            {
                var settingName = "date";
                var value = (DateTime?)GetSetting(settingName);
                return value??DateTime.Today;
            }
            set
            {
                var settingName = "date";
                _settings[settingName] = value;
            }
        }

        public static bool Internet
        {
            get
            {
                var settingName = "internet";
                var value = (bool?)GetSetting(settingName);
                return value ?? true;
            }
            set
            {
                var settingName = "internet";
                _settings[settingName] = value;
            }
        }

        public static void SaveSettings()
        {
            _settings.Save();
        }

        public static object GetSetting(string name)
        {
            if (_settings.Contains(name))
                return _settings[name];
            return null;
        }
        
        public static bool DoNotSuggestFacebookIntegration
        {
            get
            {
                var settingName = "nofacebooksuggestion";
                var result = (bool?)GetSetting(settingName);
                return result ?? false;
            }
            set
            {
                var settingName = "nofacebooksuggestion";
                _settings[settingName] = value;
            }
        }
        public static bool FacebookActive
        {
            get
            {
                var settingName = "facebookactive";
                var result = (bool?)GetSetting(settingName);
                return result ?? false;
            }
            set
            {
                var settingName = "facebookactive";
                _settings[settingName] = value;
            }
        }

        public static string FacebookToken
        {
            get
            {
                var settingName = "facebooktoken";
                return (String)GetSetting(settingName);
            }
            set
            {
                var settingName = "facebooktoken";
                _settings[settingName] = value;
            }
        }

        public static string FacebookId
        {
            get
            {
                var settingName = "facebookid";
                return (String)GetSetting(settingName);
            }
            set
            {
                var settingName = "facebookid";
                _settings[settingName] = value;
            }
        }
    }
}
