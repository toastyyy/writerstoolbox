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
using WritersToolbox.Resources;
using System.Windows.Media;

namespace WritersToolbox.views
{
    public partial class Books : PhoneApplicationPage
    {
        public static BooksViewModel books_VM; // = null;

        private TextBox bookname;

        private datawrapper.BookType BookType;

        private TextBlock BookTypeInfo;
        private bool hasEventHandler = false;

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
            BookDeleteQuestion.Text = AppResources.BookDeleteQuestion1 + b.name.ToString() + AppResources.BookDeleteQuestion2;
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
           books_VM.deleteBook(b, keepTomes.IsChecked.Value);
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
            datawrapper.Book b = PivotMain.SelectedItem as datawrapper.Book;
            if (b == null)
                return;
            
            if (b.bookID == -1)
            {
                if (hasEventHandler)
                {
                    loadNewBookAppBar();
                    hasEventHandler = false;
                }
            }
            else
            {
                if (!hasEventHandler)
                {
                    loadBookAppBar();
                    hasEventHandler = true;
                }
            }
        }

        private void loadNewBookAppBar()
        {
            ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            btn1.IconUri = new Uri("/icons/save.png", UriKind.Relative);
            btn1.Text = AppResources.AppBarSave;
            btn1.Click -= new EventHandler(ChangeBook);
            btn1.Click += new EventHandler(SaveBook);
            btn2.IconUri = new Uri("/icons/cancel.png", UriKind.Relative);
            btn2.Text = AppResources.AppBarCancel;
            btn2.Click -= new EventHandler(TryDeleteBook);
            btn2.Click += new EventHandler(CancelBook);
        }

        private void loadBookAppBar()
        {
            ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            btn1.IconUri = new Uri("/icons/saveAs.png", UriKind.Relative);
            btn1.Text = AppResources.AppBarEdit;
            btn1.Click -= new EventHandler(SaveBook);
            btn1.Click += new EventHandler(ChangeBook);
            btn2.IconUri = new Uri("/icons/delete.png", UriKind.Relative);
            btn2.Text = AppResources.AppBarDelete;
            btn2.Click -= new EventHandler(CancelBook);
            btn2.Click += new EventHandler(TryDeleteBook);
        }

        /// <summary>
        /// Ein neues Buch wird gespeichert und zu diesem navigiert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBook(object sender, EventArgs e)
        {
            Books_VM.addBook(bookname.Text, BookType);
            PivotMain.SelectedIndex = PivotMain.Items.Count - 1; // wird auf das neue Werk navigiert, wird ein falsches Werk als selectedItem angegeben
            bookname.Text = "";
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
                NavigationService.Navigate(new Uri("/views/TomeDetails.xaml?tomeID=" + tome.tomeID, UriKind.Relative));
            }
            else
            {
                //neuer Band
            }
            selector.SelectedItem = null;
        }

        /// <summary>
        /// Textbox für Werkname wird gespeichert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BooknameGotFocus(object sender, RoutedEventArgs e)
        {
            bookname = sender as TextBox;
        }

        

       

        private void BookTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker lp = sender as ListPicker;
            BookType = lp.SelectedItem as datawrapper.BookType;
            if (BookTypeInfo != null)
            {
                BookTypeInfo.Text = "Vorangelegte Kapitel: " + BookType.numberOfChapter.ToString();
            }
        }

        private void BookTypeInfoLoaded(object sender, RoutedEventArgs e)
        {
            BookTypeInfo = sender as TextBlock;
            BookTypeInfo.Text = "Vorangelegte Kapitel: " + BookType.numberOfChapter.ToString();
        }

    }
}