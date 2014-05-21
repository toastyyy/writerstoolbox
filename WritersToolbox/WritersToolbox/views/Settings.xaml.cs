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
        //Flag ob Seite geladen
        private bool loaded = false;

        //Index der ausgewählten Sprache
        private int langIndex;

        public Settings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Es wird überprüft, welche Sprache aktuell im Phone angestellt ist und der Index der Sprachbox angepasst.
        /// </summary>
        /// <param name="e"></param>
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

        /// <summary>
        /// Die Sprache wurde verändert und die Seite wird mit neuer Spracheinstellung neu geladen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Nach dem Laden der Seite wird der Flag verändert und die Index angepasst.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
            langPicker.SelectedIndex = langIndex;
        }

        /// <summary>
        /// Navi zur Text-Exportseite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/ExportText.xaml", UriKind.RelativeOrAbsolute));
        }        

        /// <summary>
        /// Navi zur Import-Exportseite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/ExportImportBackup.xaml", UriKind.RelativeOrAbsolute));
        }        
    }
}