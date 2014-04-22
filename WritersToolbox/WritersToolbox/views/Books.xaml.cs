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
using System.Diagnostics;
using System.Windows.Input;

namespace WritersToolbox.views
{
    public partial class Books : PhoneApplicationPage
    {
        public static BooksViewModel books_VM; // = null;


        //Name für ein neues Werk
        private TextBox bookname;


        //Werk, welches gerade im Pivot angezeigt wird
        private datawrapper.Book b;

        //Buchtyp für neues Werk
        private datawrapper.BookType BT;

        //Listpicker für den Buchtyp
        private ListPicker picker;

        //Textblock mit Infos über den ausgewählten Buchtyp
        private TextBlock BookTypeInfo;

        //Flag für ändern der BUttons in der Appbar
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
                        Books_VM.loadBookTypes();
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

            //if (e.PinchManipulation != null)
            //{
            //    if (e.PinchManipulation.CumulativeScale > 1d)
            //    {
            //        System.Diagnostics.Debug.WriteLine("Zoomout");
            //        NavigationService.Navigate(new Uri("/views/BooksOverview.xaml", UriKind.Relative));
            //    }
            //    else
            //        System.Diagnostics.Debug.WriteLine("Zoomin");

            //}

 

        }

        /// <summary>
        /// In diesem Fall wird, falls die App unterbrochen wird und gerade ein neues Werk angelegt wurde,
        /// die bisherigen Eingaben zwischengespeichert.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (PivotMain.SelectedIndex == PivotMain.Items.Count - 1)
            {
                PhoneApplicationService.Current.State["RestoreData"] =
                new datawrapper.Book()
                {
                    name = bookname.Text,
                    bookType = BT
                };
            }
            

        }

        /// <summary>
        /// Wird auf diese Page naviert, überprüft die Methode folgendes: ob gerade eine Notiz zugewiesen wird,
        /// ob zu einem gewünschten Pivotitem navigiert werden soll oder ob die App von tombstoned wieder erwacht.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Books_VM.loadData();
            if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                PivotMain.Title = new TextBlock() { Text = "Notiz zuweisen" };
                ApplicationBar.Buttons.Clear();
                hasEventHandler = false;
            }

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
            //schon eingegebene Daten werden wiederhergestellt
            if (PhoneApplicationService.Current.State.ContainsKey("tombstoned"))
            {
                if (PhoneApplicationService.Current.State.ContainsKey("RestoreData"))
                {
                    b = (datawrapper.Book)PhoneApplicationService.Current.State["RestoreData"];

                    PivotMain.SelectedIndex = books_VM.getBookCount() -1;
                    loadNewBookAppBar();
                    PhoneApplicationService.Current.State.Remove("RestoreData");
                }
                PhoneApplicationService.Current.State.Remove("tombstoned");
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
            if (b.tomes.Count == 1)
            {
                BookDeleteQuestion.Text = AppResources.BookDeleteQuestion1 + b.name.ToString() + AppResources.BookDeleteQuestion2;
                deleteBookPopup.IsOpen = true;
            }
            else
            {
                MessageBox.Show("Es ist nur möglich ein Werk zu löschen, wenn keine Bände mehr existieren.", "Fehler", MessageBoxButton.OK);
            }
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
           this.PivotMain.SelectedIndex = (this.PivotMain.SelectedIndex) % this.PivotMain.Items.Count;
           this.PivotMain.SelectedIndex = (this.PivotMain.SelectedIndex == 0) ?
               this.PivotMain.Items.Count - 1 :
               this.PivotMain.SelectedIndex--;
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

        /// <summary>
        /// Die Appbar mit den passenden Buttons für ein neues Werk wird geladen.
        /// Wenn gerade eine Notiz zugewiesen wird, wird diese Appbar geladen.
        /// </summary>
        private void loadNewBookAppBar()
        {
            if (!PhoneApplicationService.Current.State.ContainsKey("assignNote"))
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
            else {
                ApplicationBar.Buttons.Clear();
                ApplicationBarIconButton btn = new ApplicationBarIconButton();
                btn.IconUri = new Uri("/icons/cancel.png", UriKind.Relative);
                btn.Text = AppResources.AppBarCancel;
                btn.Click += new EventHandler(cancelAssignment);
                ApplicationBar.Buttons.Add(btn);
            }
        }

        /// <summary>
        /// Die Appbar für ein existierendes Werk wird geladen.
        /// Wenn gerade eine Notiz zugewiesen wird, wird diese Appbar geladen.
        /// </summary>
        private void loadBookAppBar()
        {
            if (!PhoneApplicationService.Current.State.ContainsKey("assignNote"))
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
            else
            {
                ApplicationBar.Buttons.Clear();
                ApplicationBarIconButton btn = new ApplicationBarIconButton();
                btn.IconUri = new Uri("/icons/cancel.png", UriKind.Relative);
                btn.Text = AppResources.AppBarCancel;
                btn.Click += new EventHandler(cancelAssignment);
                ApplicationBar.Buttons.Add(btn);
            }
        }

        /// <summary>
        /// Ein neues Buch wird gespeichert und zu diesem navigiert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBook(object sender, EventArgs e)
        {
            Books_VM.addBook(bookname.Text, BT);
            PivotMain.SelectedIndex = PivotMain.Items.Count - 2; 
            //Workaround, damit der Content des Pivot aktualisiert wird.
            PivotMain.SelectedIndex++;
            PivotMain.SelectedIndex--;
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
        /// Die Methode navigiert zur Detailansicht eines Bandes. Es wird die TomeID übergeben.
        /// Soll ein neuer Band erstellt haben, ist die TomeID -1:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tomeSelected(object sender, SelectionChangedEventArgs e)
        {
            datawrapper.Book b = PivotMain.SelectedItem as datawrapper.Book;
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
                NavigationService.Navigate(new Uri("/views/AddTome.xaml?bookID=" + b.bookID, UriKind.Relative));
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

        

       
        /// <summary>
        /// Beim Ändern der Auswahl im Listpicker für den Buchtyp, wird der Infotext angepasst.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BookTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker lp = sender as ListPicker;
            picker = lp;
            BT = lp.SelectedItem as datawrapper.BookType;
            if (BookTypeInfo != null)
            {
                BookTypeInfo.Text = AppResources.BooksBookTypeInfoText + BT.numberOfChapter.ToString();
            }
        }

        /// <summary>
        /// Wenn die Textbox für den Infotext geladen hat, wird der passende Infotext übergeben.
        /// Dieses Vorgehen ist notwendig, weil es sich um ein Template handelt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BookTypeInfoLoaded(object sender, RoutedEventArgs e)
        {
            BookTypeInfo = sender as TextBlock;
            BookTypeInfo.Text = AppResources.BooksBookTypeInfoText + BT.numberOfChapter.ToString();
        }

        /// <summary>
        /// Das Zuordnen einer Notiz wird beendet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelAssignment(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State["cancelAssignment"] = true;
            NavigationService.GoBack();
        }

        /// <summary>
        /// Der Werkname wird in der aktuellen Werkvariable gespeichert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BooknameLoaded(object sender, RoutedEventArgs e)
        {
            bookname = sender as TextBox;
            if (b != null)
            {
                bookname.Text = b.name;
            }
        }

        /// <summary>
        /// Der passende Buchtyp der Werkvariable wird dem Picker übergeben.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PickerLoaded(object sender, RoutedEventArgs e)
        {
            ListPicker lp = sender as ListPicker;
            if (b != null)
            {
                lp.SelectedIndex = b.bookType.bookTypeID -1;
            }
        }


        /// <summary>
        /// Die Searchview wird aufgerufen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Search.xaml", UriKind.RelativeOrAbsolute));
        }

        private void LayoutRoot_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
                NavigationService.Navigate(new Uri("/views/BooksOverview.xaml", UriKind.Relative));
                e.Handled = true;
                e.Complete();
        }

    }
}