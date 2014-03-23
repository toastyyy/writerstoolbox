using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;

namespace WritersToolbox.views
{
    public partial class DropboxAuthorize : PhoneApplicationPage
    {
        public DropboxAuthorize()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (PhoneApplicationService.Current.State.ContainsKey("authURL")) {
                var url = PhoneApplicationService.Current.State["authURL"];
                PhoneApplicationService.Current.State.Remove("authURL");
                webBrowser.IsScriptEnabled = true;
                webBrowser.Navigate(new Uri((String)url));
            }
        }

        private void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            
        }

        private void webBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            if(e.Uri.Host.Equals("dkdevelopment.net")) {
                PhoneApplicationService.Current.State["dropboxAuth"] = true;
                NavigationService.GoBack();
                webBrowser.Navigating -= webBrowser_Navigating;
            }
        }
    }
}