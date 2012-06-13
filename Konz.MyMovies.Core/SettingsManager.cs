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

namespace Konz.MyMovies.Core
{
    public class SettingsManager
    {
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
