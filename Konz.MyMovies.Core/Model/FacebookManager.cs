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
using Facebook;
using System.Windows.Threading;
using System.Collections.Generic;
using Konz.MyMovies.Core;
using System.Threading;

namespace Konz.MyMovies.Model
{
    public class FacebookManager
    {
        private const string facebookId = "139393152872238";
        private const string extendedPermissions = "publish_stream";
        private string facebookToken;
        private FacebookClient fb;
        private static FacebookClient _fb = new FacebookClient();

        public Action<FacebookResult> OnComplete;

        public FacebookManager(string facebookToken)
        {
            this.facebookToken = facebookToken;
            this.fb = new FacebookClient(facebookToken);
        }

        public void Share(Movie m, string message)
        {
            var accessToken = SettingsManager.FacebookToken;
            FacebookClient fb= new FacebookClient(accessToken);
            
            fb.PostCompleted += (o, args) =>
            {
                if (args.Error != null)
                {
                    OnComplete(new FacebookResult()
                    {
                        Error = args.Error
                    });
                }
                else
                {
                    OnComplete(new FacebookResult()
                    {
                        Result = (IDictionary<string, object>)args.GetResultData()
                    });
                    //var result = (IDictionary<string, object>)args.GetResultData();
                    //_lastMessageId = (string)result["id"];
                }
            };

            var parameters = new Dictionary<string, object>();
            parameters["caption"] = "Checa 'My Movies' en Windows Phone!";
            parameters["message"] = message;
            parameters["name"] = m.Title;
            parameters["description"] = m.Sinopsis;
            parameters["picture"] = m.PosterUri;
            parameters["link"] = m.MovieUri;

            fb.PostAsync("me/feed", parameters);
        }

        public static Uri GetFacebookLoginUrl()
        {
            var parameters = new Dictionary<string, object>();
            parameters["client_id"] = facebookId;
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
            parameters["display"] = "touch";

            // add the 'scope' only if we have extendedPermissions.
            if (!string.IsNullOrEmpty(extendedPermissions))
            {
                // A comma-delimited list of permissions
                parameters["scope"] = extendedPermissions;
            }
            return _fb.GetLoginUrl(parameters);
        }
        
        public static FacebookOAuthResult GetToken(Uri uri)
        {
            FacebookOAuthResult result;
            if (_fb.TryParseOAuthCallbackUrl(uri, out result))
                return result;
            return result;

        }
    }
}
