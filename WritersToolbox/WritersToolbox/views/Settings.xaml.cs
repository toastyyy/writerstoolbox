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
        private bool loaded = false;

        private int langIndex;

        public Settings()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (Thread.CurrentThread.CurrentCulture.Name.Equals("de-DE"))
            {
                langIndex = 0;
            }
            else if (Thread.CurrentThread.CurrentCulture.Name.Equals("en-US"))
            {
                langIndex = 1;
            }
        }


        private void LanguageChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker lp = sender as ListPicker;
            if (loaded)
            {
                if (lp.SelectedIndex == 0)
                {
                    if (!Thread.CurrentThread.CurrentCulture.Name.Equals("de-DE"))
                    {
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
                        NavigationService.Navigate(new Uri("/views/Settings.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
                        NavigationService.RemoveBackEntry();
                    }
                }
                else if (lp.SelectedIndex == 1)
                {
                    if (!Thread.CurrentThread.CurrentCulture.Name.Equals("en-US"))
                    {
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                        NavigationService.Navigate(new Uri("/views/Settings.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
                        NavigationService.RemoveBackEntry();
                    }
                }
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
            langPicker.SelectedIndex = langIndex;
        }

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/ExportText.xaml", UriKind.RelativeOrAbsolute));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {

            NavigationService.Navigate(new Uri("/views/StartPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}