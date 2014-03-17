﻿using System;
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
            UnsortedNoteViewModel usnvm = new UnsortedNoteViewModel();
            TrashbinViewModel tbvm = new TrashbinViewModel();
            NumberUN.Text = usnvm.getNumberOfUnsortedNote() + "";
            //TrashCounter.Text = "Papierkorb (" + tbvm.getNumberOfTrash() + ")";
            if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                // GUI überarbeiten
             //   this.btnNewNote.Background = new SolidColorBrush(Color.FromArgb(155, 155, 155, 155));
                this.btnNewNote.Click -= newNote;
             //   this.btnSettings.Background = new SolidColorBrush(Color.FromArgb(155, 155, 155, 155));
                this.btnSettings.Click -= navigateToSettings;
                //   this.btnTrash.Background = new SolidColorBrush(Color.FromArgb(155, 155, 155, 155));
                this.btnTrash.Click -= navigateToTrash;
                //   this.btnUnsortedNotes.Background = new SolidColorBrush(Color.FromArgb(155, 155, 155, 155));
                this.btnUnsortedNotes.Click -= navigateToUnsortedNote;
           //     this.tTitle.Text = "Zuordnen";
                ApplicationBar.IsVisible = true;
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
            }
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
            if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                var lastPage = NavigationService.BackStack.FirstOrDefault();
                if (lastPage != null && lastPage.Source.ToString() == "/views/StartPage.xaml")
                {
                    NavigationService.RemoveBackEntry();
                }
            }
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
    }
}