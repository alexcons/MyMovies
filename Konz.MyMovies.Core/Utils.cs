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
using System.Net.NetworkInformation;

namespace Konz.MyMovies.Core
{
    public class Utils
    {
        public static bool InternetIsAvailable()
        {
            var available = NetworkInterface.GetIsNetworkAvailable();
//#if DEBUG
//            available = false;
//#endif
            if (!available)
            {
                MessageBox.Show("No hay connexion a internet. Por favor intente mas tarde.");
                return false;
            }
            return true;
        }

    }
}
