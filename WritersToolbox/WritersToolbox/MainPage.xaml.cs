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
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Devices;
namespace WritersToolbox
{
    public partial class MainPage : PhoneApplicationPage
    {
        private WritersToolboxDatebase db;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            db = WritersToolboxDatebase.getInstance();
            //IsolatedStorageFile.GetUserStoreForApplication().Remove();
            try
            {
                //db.DeleteDatabase();
                if (!db.DatabaseExists())
                {
                    db.CreateDatabase();
                    Data.AddToDB(db);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            if (Microsoft.Devices.Environment.DeviceType == DeviceType.Emulator)
            {
                EmulatorHelper.AddDebugImages();
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

    public static class EmulatorHelper
    {
        const string flagName = "__emulatorTestImagesAdded";

        public static void AddDebugImages()
        {
            bool alreadyAdded = CheckAlreadyAdded();
            if (!alreadyAdded)
            {
                AddImages();
                SetAddedFlag();
            }
        }

        private static bool CheckAlreadyAdded()
        {
            IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;

            try
            {
                bool alreadyAdded = (bool)userSettings[flagName];
                return alreadyAdded;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static void SetAddedFlag()
        {
            IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;
            userSettings.Add(flagName, true);
            userSettings.Save();
        }

        private static void AddImages()
        {
            string[] fileNames = { "img10", "img11", "img12", "img7", "img8", "img9" };
            foreach (var fileName in fileNames)
            {
                MediaLibrary myMediaLibrary = new MediaLibrary();
                Uri myUri = new Uri(String.Format(@"tests/images/{0}.jpg", fileName), UriKind.Relative);

                System.IO.Stream photoStream = App.GetResourceStream(myUri).Stream;
                byte[] buffer = new byte[photoStream.Length];
                photoStream.Read(buffer, 0, Convert.ToInt32(photoStream.Length));
                myMediaLibrary.SavePicture(String.Format("{0}.jpg", fileName), buffer);
                photoStream.Close();
            }
        }
    }
}