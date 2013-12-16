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
                db.DeleteDatabase();
                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                    BookType bt = new BookType();
                    bt.name = "Roman";
                    bt.updatedDate = DateTime.Now;
                    bt.addedDate = DateTime.Now;
                    bt.numberOfChapter = 3;

                    MemoryNote mn = new MemoryNote();
                    mn.addedDate = DateTime.Now;
                    mn.updatedDate = DateTime.Now;
                    mn.contentText = "lorem ipsum dolor sit amet";
                    mn.title = "testnotiz";
                    mn.associated = false;
                    mn.tags = "test1|test2";
                    db.GetTable<BookType>().InsertOnSubmit(bt);
                    db.GetTable<MemoryNote>().InsertOnSubmit(mn);
                    db.SubmitChanges();
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