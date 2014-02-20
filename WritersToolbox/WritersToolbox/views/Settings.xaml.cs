using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using WritersToolbox.viewmodels;
using System.Threading;

namespace WritersToolbox.views
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void langDE(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!Thread.CurrentThread.CurrentCulture.Name.Equals("de-de"))
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-de");
            }
        }

        private void langEngl(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!Thread.CurrentThread.CurrentCulture.Name.Equals("en"))
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            }
        }

        
    }
}