using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WritersToolbox.Resources;
using WritersToolbox.models;
using WritersToolbox.demo;
namespace WritersToolbox
{
    public partial class MainPage : PhoneApplicationPage
    {
        private WritersToolboxDatebase db;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            db = new WritersToolboxDatebase();
            try
            {
                db.DeleteDatabase(); // entfernen wenn demodaten fertig
                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                    Data.AddToDB(db);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/StartPage.xaml", UriKind.Relative));
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}