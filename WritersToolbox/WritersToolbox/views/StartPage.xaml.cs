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
using System.Windows.Media;
using WritersToolbox.Resources;
using System.Windows.Threading;

namespace WritersToolbox.gui
{
    public partial class Page1 : PhoneApplicationPage
    {
        private DispatcherTimer newTimer;
        public Page1()
        {
            InitializeComponent();

            //IsolatedStorageFile.GetUserStoreForApplication().Remove(); //Um Isolated Storge zu leeren.

            models.WritersToolboxDatebase db = models.WritersToolboxDatebase.getInstance();

            if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                // GUI überarbeiten
                this.btnNewNote.Background = new SolidColorBrush(Color.FromArgb(155, 155, 155, 155));
                this.btnNewNote.Click -= newNote;
                this.btnSettings.Background = new SolidColorBrush(Color.FromArgb(155, 155, 155, 155));
                this.btnSettings.Click -= navigateToSettings;
                this.btnTrash.Background = new SolidColorBrush(Color.FromArgb(155, 155, 155, 155));
                this.btnTrash.Click -= navigateToTrash;
                this.btnUnsortedNotes.Background = new SolidColorBrush(Color.FromArgb(155, 155, 155, 155));
                this.btnUnsortedNotes.Click -= navigateToUnsortedNote;
                this.Title.Visibility = Visibility.Visible;
                ApplicationBar.IsVisible = true;
                AppTitle.Visibility = Visibility.Collapsed;
                this.btnBooks.Margin = new Thickness(this.btnBooks.Margin.Left, this.btnBooks.Margin.Top - 60.0, this.btnBooks.Margin.Right, this.btnBooks.Margin.Bottom);
                this.btnNewNote.Margin = new Thickness(this.btnNewNote.Margin.Left, this.btnNewNote.Margin.Top - 60.0, this.btnNewNote.Margin.Right, this.btnNewNote.Margin.Bottom);
                this.btnSettings.Margin = new Thickness(this.btnSettings.Margin.Left, this.btnSettings.Margin.Top - 60.0, this.btnSettings.Margin.Right, this.btnSettings.Margin.Bottom);
                this.btnTrash.Margin = new Thickness(this.btnTrash.Margin.Left, this.btnTrash.Margin.Top - 60.0, this.btnTrash.Margin.Right, this.btnTrash.Margin.Bottom);
                this.btnTypes.Margin = new Thickness(this.btnTypes.Margin.Left, this.btnTypes.Margin.Top - 60.0, this.btnTypes.Margin.Right, this.btnTypes.Margin.Bottom);
                this.btnUnsortedNotes.Margin = new Thickness(this.btnUnsortedNotes.Margin.Left, this.btnUnsortedNotes.Margin.Top - 60.0, this.btnUnsortedNotes.Margin.Right, this.btnUnsortedNotes.Margin.Bottom);
                BackgroundImage.Margin = new Thickness(this.BackgroundImage.Margin.Left, this.BackgroundImage.Margin.Top - 60.0, this.BackgroundImage.Margin.Right, this.BackgroundImage.Margin.Bottom);
                SearchImage.Visibility = Visibility.Collapsed;
            }
            else
            {
                ApplicationBar.IsVisible = false;
                // this.tTitle.Text = "Writer's Toolbox";
                this.btnNewNote.Click += newNote;
                //    this.btnNewNote.Background = new SolidColorBrush(Color.FromArgb(255, 114, 169, 28));
                this.btnSettings.Click += navigateToSettings;
                //    this.btnSettings.Background = new SolidColorBrush(Color.FromArgb(255, 205, 155, 5));
                this.btnTrash.Click += navigateToTrash;
                //     this.btnTrash.Background = new SolidColorBrush(Color.FromArgb(255, 205, 155, 5));
                this.btnUnsortedNotes.Click += navigateToUnsortedNote;
                //    this.btnUnsortedNotes.Background = new SolidColorBrush(Color.FromArgb(255, 145, 17, 36));
                AppTitle.Visibility = Visibility.Visible;
            }
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

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (!PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                Application.Current.Terminate();
                e.Cancel = true;
            }
        }

        //Fertig
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("cancelAssignment")) {
                PhoneApplicationService.Current.State.Remove("cancelAssignment");
                PhoneApplicationService.Current.State.Remove("assignNote");
                NavigationService.GoBack();
            }
            if (PhoneApplicationService.Current.State.ContainsKey("typeObjectID"))
            {
                NavigationService.GoBack();
                return;
            }
            if (PhoneApplicationService.Current.State.ContainsKey("eventID"))
            {
                NavigationService.GoBack();
                return;
            }
            UnsortedNoteViewModel usnvm = new UnsortedNoteViewModel();
            TrashbinViewModel tbvm = new TrashbinViewModel();
            NumberUN.Text = usnvm.getNumberOfUnsortedNote() + "";
            //TrashCounter.Text = "Papierkorb (" + tbvm.getNumberOfTrash() + ")";
            if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                // GUI überarbeiten
                this.btnNewNote.Background = new SolidColorBrush(Color.FromArgb(155, 155, 155, 155));
                this.btnNewNote.Click -= newNote;
                this.btnSettings.Background = new SolidColorBrush(Color.FromArgb(155, 155, 155, 155));
                this.btnSettings.Click -= navigateToSettings;
                this.btnTrash.Background = new SolidColorBrush(Color.FromArgb(155, 155, 155, 155));
                this.btnTrash.Click -= navigateToTrash;
                this.btnUnsortedNotes.Background = new SolidColorBrush(Color.FromArgb(155, 155, 155, 155));
                this.btnUnsortedNotes.Click -= navigateToUnsortedNote;
                this.Title.Visibility = Visibility.Visible;
                ApplicationBar.IsVisible = true;
                AppTitle.Visibility = Visibility.Collapsed;
                //this.btnBooks.Margin = new Thickness(this.btnBooks.Margin.Left, this.btnBooks.Margin.Top - 60.0, this.btnBooks.Margin.Right, this.btnBooks.Margin.Bottom);
                //this.btnNewNote.Margin = new Thickness(this.btnNewNote.Margin.Left, this.btnNewNote.Margin.Top - 60.0, this.btnNewNote.Margin.Right, this.btnNewNote.Margin.Bottom);
                //this.btnSettings.Margin = new Thickness(this.btnSettings.Margin.Left, this.btnSettings.Margin.Top - 60.0, this.btnSettings.Margin.Right, this.btnSettings.Margin.Bottom);
                //this.btnTrash.Margin = new Thickness(this.btnTrash.Margin.Left, this.btnTrash.Margin.Top - 60.0, this.btnTrash.Margin.Right, this.btnTrash.Margin.Bottom);
                //this.btnTypes.Margin = new Thickness(this.btnTypes.Margin.Left, this.btnTypes.Margin.Top - 60.0, this.btnTypes.Margin.Right, this.btnTypes.Margin.Bottom);
                //this.btnUnsortedNotes.Margin = new Thickness(this.btnUnsortedNotes.Margin.Left, this.btnUnsortedNotes.Margin.Top - 60.0, this.btnUnsortedNotes.Margin.Right, this.btnUnsortedNotes.Margin.Bottom);
                //BackgroundImage.Margin = new Thickness(this.BackgroundImage.Margin.Left, this.BackgroundImage.Margin.Top - 60.0, this.BackgroundImage.Margin.Right, this.BackgroundImage.Margin.Bottom);
            }
            else 
            {
                ApplicationBar.IsVisible = false;
               // this.tTitle.Text = "Writer's Toolbox";
                this.btnNewNote.Click += newNote;
                //    this.btnNewNote.Background = new SolidColorBrush(Color.FromArgb(255, 114, 169, 28));
                this.btnSettings.Click += navigateToSettings;
                //    this.btnSettings.Background = new SolidColorBrush(Color.FromArgb(255, 205, 155, 5));
                this.btnTrash.Click += navigateToTrash;
                //     this.btnTrash.Background = new SolidColorBrush(Color.FromArgb(255, 205, 155, 5));
                this.btnUnsortedNotes.Click += navigateToUnsortedNote;
                //    this.btnUnsortedNotes.Background = new SolidColorBrush(Color.FromArgb(255, 145, 17, 36));
                AppTitle.Visibility = Visibility.Visible;
            }

            if (views.addNote.meldung != null)
            {
                //MessageBoxResult result = MessageBox.Show(views.addNote.meldung,
                //        AppResources.AppBarClose, MessageBoxButton.OK);
                //views.addNote.meldung = null;
                newTimer = new DispatcherTimer();
                // timer interval specified as 1 second
                if (views.addNote.meldung.Contains("Event"))
                {
                    newTimer.Interval = TimeSpan.FromMilliseconds(520);
                }
                else
                {
                    newTimer.Interval = TimeSpan.FromMilliseconds(500);
                }                
                // Sub-routine OnTimerTick will be called at every 1 second
                newTimer.Tick += OnTimerTick;
                // starting the timer
                newTimer.Start();
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            newTimer.Stop();
            newTimer = null;
            MessageBoxResult result = MessageBox.Show(views.addNote.meldung,
                    AppResources.AppBarSuccessful, MessageBoxButton.OK);
            views.addNote.meldung = null;

        }
        
        private void newNote(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/AddNote.xaml", UriKind.Relative));
        }

        private void navigateToTypes(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Types.xaml", UriKind.Relative));
            //Um GoBack zu beschrenken, dass App nicht wieder zu StartPage navigiert,
            //wenn die Notiz zugeordnet ist.
            //if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            //{
            //    var lastPage = NavigationService.BackStack.FirstOrDefault();
            //    if (lastPage != null && lastPage.Source.ToString() == "/views/StartPage.xaml")
            //    {
            //        NavigationService.RemoveBackEntry();
            //    }
            //}
        }

        private void navigateToUnsortedNote(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/UnsortedNote.xaml", UriKind.Relative));
        }

        private void navigateToBooks(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Books.xaml", UriKind.Relative));
        }

        private void navigateToTrash(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Trashbin.xaml", UriKind.Relative));
        }

        private void navigateToSettings(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Settings.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State.Remove("assignNote");
            NavigationService.GoBack();
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Search.xaml", UriKind.RelativeOrAbsolute));
        }      
    }
}