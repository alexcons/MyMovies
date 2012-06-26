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

namespace Konz.MyMovies.UI
{
    public partial class LoadingControl : UserControl
    {
        public LoadingControl()
        {
            InitializeComponent();
        }

        #region Properties
        
        public bool IsLoading
        {
            get 
            { 
                return pgbLoading.Visibility == Visibility.Visible;
            }
            set
            {
                pgbLoading.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public string Message
        {
            get
            {
                return txtMessage.Text;
            }
            set
            {
                txtMessage.Text = value;
            }
        }
        
        #endregion
    }
}
