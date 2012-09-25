using System;
using System.Globalization;
using System.Net.NetworkInformation;

namespace Konz.MyMovies.Core
{
    public class Utils
    {
        static CultureInfo culture = new CultureInfo("es-MX");

        public static bool InternetIsAvailable()
        {            
            return NetworkInterface.GetIsNetworkAvailable() && SettingsManager.Internet;
        }

        public static string GetLongDate(DateTime date)
        {
            return date.ToString(@"d \de MMMM - dddd", culture);
        }

        public static DateTime ParseLongDate(string date)
        {
            return DateTime.ParseExact(date, @"d \de MMMM - dddd", culture);
        }

        public static string GetTimeFormat(DateTime time)
        {
            return time.ToString(@"h:mmt\m").ToLower();
        }

        public static string GetMessage(Error error)
        {
            switch (error)
            {
                case Error.NoInternetConnection:
                    return "No hay connexion a internet por favor intente mas tarde";
                case Error.NoServerAvailable:
                    return "No pudimos conectarnos al servidor de cinépolis por favor intente mas tarde";
                case Error.CitiesFileNotSaved:
                    return "El guardado del archivo local de ciudades no funcionó";
                case Error.AppStateFileNotSaved:
                    return "El guardado del archivo local de estado no funcionó";
                default:
                    return "Error no identificado.";
            }
        }

        public static string GetMessage(Info info)
        {
            switch (info)
            {
                case Info.ChooseCity:
                    return "Elige tu ciudad";
                case Info.AnyTime:
                    return "Todas";
                case Info.Sheadules:
                    return "Horarios";
                case Info.Loading:
                    return "Cargando...";
                case Info.NoData:
                    return "No hay datos";
                case Info.NoMoreShows:
                    return "En {0} no hay mas funciones :("; 
                case Info.FacebookIntegration:
                    return "¿Quisieras integrar tu experiencia con Facebook?";
                case Info.FacebookIntegrationTitle:
                    return "Integración con Facebook";
                case Info.GetDefaultShareMessage:
                    return "¡Quiero ver esta película hoy!";
                default:
                    return "Info No identificado";
            }
        }
    }
}
