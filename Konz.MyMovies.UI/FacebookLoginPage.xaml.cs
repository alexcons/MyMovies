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
using Facebook;
using Konz.MyMovies.Core;
using Konz.MyMovies.Model;

namespace Konz.MyMovies.UI
{
    public partial class FacebookLoginPage : PhoneApplicationPage
    {
        private FacebookManager _fb;

        public FacebookLoginPage()
        {
            InitializeComponent();
        }

        private void wBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            var loginUrl = FacebookManager.GetFacebookLoginUrl();
            wBrowser.Navigate(loginUrl);
        }

        private void wBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var result = FacebookManager.GetToken(e.Uri);
            if (e.Uri.LocalPath == @"/home.php")
                LoginDenied(result);
            if (result == null)
                return;
            if (result.IsSuccess && !string.IsNullOrWhiteSpace(result.AccessToken))
                LoginSucceded(result.AccessToken);
            else 
                LoginDenied(result);
        }

        private void LoginDenied(FacebookOAuthResult result)
        {
            MessageBox.Show("Lo siento, no pudimos registrarte");
            SettingsManager.FacebookActive = false;
            SettingsManager.FacebookToken = string.Empty;
            SettingsManager.DoNotSuggestFacebookIntegration = true;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void LoginSucceded(string accessToken)
        {
            var fb = new FacebookClient(accessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }
                
                var result = (IDictionary<string, object>)e.GetResultData();
                SettingsManager.FacebookActive = true;
                SettingsManager.FacebookToken = accessToken;
                SettingsManager.FacebookId = (string)result["id"];
                
                Dispatcher.BeginInvoke(() => {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                });
            };

            fb.GetAsync("me?fields=id");
        }
    }
}