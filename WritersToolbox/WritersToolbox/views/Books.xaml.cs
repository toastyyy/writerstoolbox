﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WritersToolbox.viewmodels;

namespace WritersToolbox.views
{
    public partial class Books : PhoneApplicationPage
    {
        public static BooksViewModel books_VM = null;
        private datawrapper.Book selectedBook;
        /// <summary>
        /// ViewModel für Types und TypesOverview wird erstellt.
        /// </summary>
        public static BooksViewModel Books_VM
        {
            get
            {
                if (books_VM == null)
                {
                    books_VM = new BooksViewModel();
                    if (!books_VM.isDataLoaded())
                        books_VM.loadData();

                }
                return books_VM;
            }
        }

        public Books()
        {
            DataContext = Books_VM;

            InitializeComponent();
        }

        //private void navigateToBooksDetails(object sender, RoutedEventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/views/BooksDetails.xaml", UriKind.Relative));
        //}


        /// <summary>
        /// Die Methode erkennt die Zoomout-Geste und navigiert zu TypesOverview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pinch_out(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {

            if (e.PinchManipulation != null)
            {
                if (e.PinchManipulation.CumulativeScale > 1d)
                {
                    System.Diagnostics.Debug.WriteLine("Zoomout");
                    NavigationService.Navigate(new Uri("/views/BooksOverview.xaml", UriKind.Relative));
                }
                else
                    System.Diagnostics.Debug.WriteLine("Zoomin");

            }


        }

        /// <summary>
        /// Wird auf diese Page naviert, überprüft die Methode, ob zu einem gewünschten Pivotitem
        /// navigiert werden soll.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("item"))
            {
                var item = NavigationContext.QueryString["item"];
                var indexParsed = int.Parse(item);
                if (indexParsed == -1)
                {
                    PivotMain.SelectedIndex = Books_VM.getBookCount() - 1;
                } else 
                    PivotMain.SelectedIndex = indexParsed - 1;
            }


        }


        /// <summary>
        /// Hilfsmethode solange man Zoom nicht testen kann.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navUeberblick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/BooksOverview.xaml", UriKind.Relative));
        }

        private void changeBookType(object sender, System.Windows.Input.GestureEventArgs e)
        {
            booktype_popup.IsOpen = true;
        }

        private void bookTypeCancel(object sender, System.Windows.Input.GestureEventArgs e)
        {
            booktype_popup.IsOpen = false;
        }

        
        private void TryDeleteBook(object sender, EventArgs e)
        {
            datawrapper.Book b = PivotMain.SelectedItem as datawrapper.Book;
            if (b == null)
                return;
            BookDeleteQuestion.Text = "Wollen Sie das Werk \"" + b.name.ToString() + "\" löschen?";
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


        private void ChangeBook(object sender, EventArgs e)
        {
            datawrapper.Book b = PivotMain.SelectedItem as datawrapper.Book;
            if (b == null)
                return;
            NavigationService.Navigate(new Uri("/views/ChangeBook.xaml?item=" + b.bookID, UriKind.Relative));
        }


        private void PivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot p = sender as Pivot;
            if (p == null)
                return;
            datawrapper.Book b = p.SelectedItem as datawrapper.Book;
            if (b == null)
                return;
            ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            if (b.bookID == -1)
            {
                btn1.IconUri = new Uri("/icons/save.png", UriKind.Relative);
                btn1.Text = "speichern";
                btn1.Click -= new EventHandler(ChangeBook);
                btn1.Click += new EventHandler(SaveBook);
                btn2.IconUri = new Uri("/icons/cancel.png", UriKind.Relative);
                btn2.Text = "abbrechen";
                btn2.Click -= new EventHandler(TryDeleteBook);
                btn2.Click += new EventHandler(CancelBook);
            }
            else
            {
                btn1.IconUri = new Uri("/icons/speichernUnter.png", UriKind.Relative);
                btn1.Text = "ändern";
                btn1.Click -= new EventHandler(SaveBook);
                btn1.Click += new EventHandler(ChangeBook);
                btn2.IconUri = new Uri("/icons/delete.png", UriKind.Relative);
                btn2.Text = "löschen";
                btn2.Click -= new EventHandler(CancelBook);
                btn2.Click += new EventHandler(TryDeleteBook);
            }
        }

        private void SaveBook(object sender, EventArgs e)
        {
            PivotMain.SelectedIndex = PivotMain.Items.Count - 2;
        }

        private void CancelBook(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}