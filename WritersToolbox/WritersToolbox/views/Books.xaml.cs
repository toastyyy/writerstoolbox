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

namespace WritersToolbox.views
{
    public partial class Books : PhoneApplicationPage
    {
        public static BooksViewModel books_VM; // = null;
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


        /// <summary>
        /// Popup zum Buchtypändern wird geöffnet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeBookType(object sender, System.Windows.Input.GestureEventArgs e)
        {
            booktype_popup.IsOpen = true;
        }

        /// <summary>
        /// Popup zum Buchtypändern wird geschlossen und abgebrochen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bookTypeCancel(object sender, System.Windows.Input.GestureEventArgs e)
        {
            booktype_popup.IsOpen = false;
        }

        /// <summary>
        /// Hold-Event wird ausgelöst zum Löschen.
        /// Popup zum Buchlöschen wird geöffnet und eine Sicherheitsabfrage gestellt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TryDeleteBook(object sender, EventArgs e)
        {
            datawrapper.Book b = PivotMain.SelectedItem as datawrapper.Book;
            if (b == null)
                return;
            BookDeleteQuestion.Text = "Wollen Sie das Werk \"" + b.name.ToString() + "\" löschen?";
            deleteBookPopup.IsOpen = true;
        }


        /// <summary>
        /// Buch wird endgülitg gelöscht und Popup geschlossen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBook(object sender, EventArgs e)
        {
           
           datawrapper.Book b = PivotMain.SelectedItem as datawrapper.Book;
           books_VM.deleteBook(b);

            deleteBookPopup.IsOpen = false;
        }


        /// <summary>
        /// Buchlöschen wird abgebrochen und Popup geschlossen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoNotDeleteBook(object sender, EventArgs e)
        {
            deleteBookPopup.IsOpen = false;
        }

        /// <summary>
        /// Navigiert zu ChangeBook mit aktuellem Buch als Parameter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeBook(object sender, EventArgs e)
        {
            datawrapper.Book b = PivotMain.SelectedItem as datawrapper.Book;
            if (b == null)
                return;
            NavigationService.Navigate(new Uri("/views/ChangeBook.xaml?item=" + b.bookID, UriKind.Relative));
        }

        /// <summary>
        /// Die Methode passt beim ändern des Pivotitems die Icons und Handler der Appbar an
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                btn1.IconUri = new Uri("/icons/saveAs.png", UriKind.Relative);
                btn1.Text = "ändern";
                btn1.Click -= new EventHandler(SaveBook);
                btn1.Click += new EventHandler(ChangeBook);
                btn2.IconUri = new Uri("/icons/delete.png", UriKind.Relative);
                btn2.Text = "löschen";
                btn2.Click -= new EventHandler(CancelBook);
                btn2.Click += new EventHandler(TryDeleteBook);
            }
        }

        /// <summary>
        /// Ein neues Buch wird gespeichert und zu diesem navigiert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBook(object sender, EventArgs e)
        {
            //save Methode fehlt
            PivotMain.SelectedIndex = PivotMain.Items.Count - 2;
        }


        /// <summary>
        /// Neues Bucherstellen wird abgebrochen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBook(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// Die Methode navigiert zur Detailansicht eines Bandes. Es wird die TomeID übergeben
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tomeSelected(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            if (selector == null)
                return;
            datawrapper.Tome tome = selector.SelectedItem as datawrapper.Tome;
            if (tome == null)
                return;
            if (tome.tomeID != -1)
            {
                NavigationService.Navigate(new Uri("/views/BookDetails.xaml?tomeID=" + tome.tomeID, UriKind.Relative));
            }
            else
            {
                //neuer Band
            }
            selector.SelectedItem = null;
        }

    }
}