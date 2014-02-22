using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WritersToolbox.viewmodels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WritersToolbox.models;
using System.Diagnostics;
using WritersToolbox.Resources;
namespace WritersToolbox.views
{
    public partial class BooksOverview : PhoneApplicationPage
    {
        private datawrapper.Book selectedBook;

        public BooksOverview()
        {
            InitializeComponent();
            //der Seite wird der DataContext von Types zugewiesen
            DataContext = Books.Books_VM;
        }


        /// <summary>
        /// Die Methode erkennt die Zoomout-Geste und navigiert zu Types
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pinch_out(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                if (e.PinchManipulation.CumulativeScale < 1d)
                {
                    System.Diagnostics.Debug.WriteLine("Zoomin");
                    NavigationService.Navigate(new Uri("/views/Books.xaml", UriKind.Relative));
                }
                else
                    System.Diagnostics.Debug.WriteLine("Zoomout");

            }
        }

        /// <summary>
        /// Die Methode wird aufgerufen, wenn ein Item im LongListSelector angeklickt wurde.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            if (selector == null)
                return;
            datawrapper.Book b = selector.SelectedItem as datawrapper.Book;
            if (b == null)
                return;
             // dem Navigationspfad wird angehängt, welches item geklickt wurde und zu welchem Pivotitem naviert werden soll
            NavigationService.Navigate(new Uri("/views/Books.xaml?item=" + b.bookID, UriKind.Relative));
            selector.SelectedItem = null;
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
        }


        /// <summary>
        /// Die Methode wird bei einem Hold-Event auf einen Typ aufgerufen, ermittelt die 
        /// jeweilige TypID und übergibt diese dem ViewModel zum Löschen des Typs.
        /// Vorher wird eine Abfrage erzeugt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TryDeleteBook(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedBook = (sender as Grid).DataContext as datawrapper.Book;
            if (selectedBook == null)
                return;
            if (selectedBook.bookID == -1)
                return;
            BookDeleteQuestion.Text = AppResources.BookDeleteQuestion1 + selectedBook.name.ToString() + AppResources.BookDeleteQuestion2;
            deleteBookPopup.IsOpen = true;
        }

        private void DeleteBook(object sender, EventArgs e)
        {
            deleteBookPopup.IsOpen = false;
        }

        private void DoNotDeleteBook(object sender, EventArgs e)
        {
            deleteBookPopup.IsOpen = false;
        }


    }
}