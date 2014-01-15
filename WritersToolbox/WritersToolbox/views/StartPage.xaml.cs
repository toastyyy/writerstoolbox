using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using WritersToolbox.viewmodels;

namespace WritersToolbox.gui
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();

            //IsolatedStorageFile.GetUserStoreForApplication().Remove(); //Um Isolated Storge zu leeren.

            models.WritersToolboxDatebase db = models.WritersToolboxDatebase.getInstance();
            try
            {
                if (!db.DatabaseExists())
                {
                    db.CreateDatabase();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
           
        }
        
        //Fertig
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UnsortedNoteViewModel usnvm = new UnsortedNoteViewModel();
            NumberUN.Text = usnvm.getNumberOfUnsortedNote() + "";
            
            
        }
        private void newNote(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/AddNote.xaml", UriKind.Relative));
        }

        private void navigateToTypes(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Types.xaml", UriKind.Relative));
        }

        private void navigateToUnsortedNote(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/UnsortedNote.xaml", UriKind.Relative));
        }

        private void navigateToBooks(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Books.xaml", UriKind.Relative));
        }

        
    }
}