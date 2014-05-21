using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WritersToolbox.viewmodels;
using WritersToolbox.Resources;
using System.Collections;
using Coding4Fun.Toolkit.Controls;

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

        private LongListMultiSelector currentSelectList = null;

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
                        Books_VM.loadData();

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
            if (PhoneApplicationService.Current.State.ContainsKey("cancelAssignment"))
            {
                NavigationService.GoBack();
            }
            if (PhoneApplicationService.Current.State.ContainsKey("eventID"))
            {
                NavigationService.GoBack();
                return;
            }
            base.OnNavigatedTo(e);
            Books_VM.loadData();
            if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                //PivotMain.Title = new TextBlock() { Text = "Notiz zuweisen" };
                PivotMain.Title = AppResources.BooksHeadlineAssignNote;
                if (!ApplicationBar.Buttons.Contains(cancelBtn))
                {
                ApplicationBar.Buttons.Clear();
                }
                hasEventHandler = false;
                searchImage.Visibility = Visibility.Collapsed;
            }
            else
            {
                searchImage.Visibility = Visibility.Visible;
                if (PhoneApplicationService.Current.State.ContainsKey("NewTome"))
                {
                    PivotMain.SelectedIndex++;
                    PivotMain.SelectedIndex--;
                    PhoneApplicationService.Current.State.Remove("NewTome");
                }
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


        ///// <summary>
        ///// Hilfsmethode solange man Zoom nicht testen kann.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void navUeberblick(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/views/BooksOverview.xaml", UriKind.Relative));
        //}


        
       

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
                MessageBox.Show(AppResources.MessageDeleteBook, AppResources.MessageError, MessageBoxButton.OK);
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
                cancelBtn = new ApplicationBarIconButton();
                cancelBtn.IconUri = new Uri("/icons/cancel.png", UriKind.Relative);
                cancelBtn.Text = AppResources.AppBarCancel;
                cancelBtn.Click += new EventHandler(cancelAssignment);
                ApplicationBar.Buttons.Add(cancelBtn);
            }
        }
        ApplicationBarIconButton cancelBtn;
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
                btn1.IconUri = new Uri("/icons/edit.png", UriKind.Relative);
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
                cancelBtn = new ApplicationBarIconButton();
                cancelBtn.IconUri = new Uri("/icons/cancel.png", UriKind.Relative);
                cancelBtn.Text = AppResources.AppBarCancel;
                cancelBtn.Click += new EventHandler(cancelAssignment);
                ApplicationBar.Buttons.Add(cancelBtn);
            }
        }

        /// <summary>
        /// Ein neues Buch wird gespeichert und zu diesem navigiert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBook(object sender, EventArgs e)
        {
            try {
                Books_VM.addBook(bookname.Text, BT);
                PivotMain.SelectedIndex = PivotMain.Items.Count - 2; 
                //Workaround, damit der Content des Pivot aktualisiert wird.
                PivotMain.SelectedIndex++;
                PivotMain.SelectedIndex--;
                bookname.Text = "";
            }
            catch (ArgumentException ae) {
                MessageBox.Show(ae.Message, "Fehler", MessageBoxButton.OK);
            }
            
        }


        /// <summary>
        /// Neues Bucherstellen wird abgebrochen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBook(object sender, EventArgs e)
        {
            this.PivotMain.SelectedIndex = 0;
            bookname.Text = "";
        }

        /// <summary>
        /// Die Methode navigiert zur Detailansicht eines Bandes. Es wird die TomeID übergeben.
        /// Soll ein neuer Band erstellt haben, ist die TomeID -1:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tomeSelected(object sender, System.Windows.Input.GestureEventArgs e)
        {
            datawrapper.Book b = PivotMain.SelectedItem as datawrapper.Book;
            datawrapper.Tome tome = (sender as Grid).DataContext as datawrapper.Tome;
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
            ApplicationBar.Buttons.Remove(cancelBtn);
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

        /// <summary>
        /// Es wurde eine Auswahl der Bände getätigt oder geändert. Passend dazu wird "neuer Band hinzufügen"
        /// aus- oder eingeblendet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LongListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListMultiSelector selector = sender as LongListMultiSelector;
            Grid parent = selector.Parent as Grid;
            if (selector == null)
                return;
            //for (int i = 0; i < e.RemovedItems.Count; i++)
            //{
            //    selector.SelectedItems.Remove(e.RemovedItems[i]);
            //}

            if (selector.SelectedItems.Count == 0)
            {
                //Einblendworkaround
                selector.SelectionChanged -= LongListSelectionChanged;
                books_VM.addAddTome(PivotMain.SelectedItem as datawrapper.Book);
                selector.ItemsSource = null;
                selector.ItemsSource = (PivotMain.SelectedItem as datawrapper.Book).tomes;
                selector.EnforceIsSelectionEnabled = false;
                selector.SelectionChanged += LongListSelectionChanged;
                this.loadBookAppBar();
                this.PivotMain.IsLocked = false;
            }
            else
            {
                //Ausblenderworkaround
                LongListMultiSelector l = new LongListMultiSelector();
                IEnumerator enume = selector.SelectedItems.GetEnumerator();
                while (enume.MoveNext())
                {
                    l.SelectedItems.Add(enume.Current);
                }
                selector.SelectionChanged -= LongListSelectionChanged;
                books_VM.removeAddTome(PivotMain.SelectedItem as datawrapper.Book);
                selector.ItemsSource = null;
                selector.ItemsSource = (PivotMain.SelectedItem as datawrapper.Book).tomes;
                selector.EnforceIsSelectionEnabled = true;
                IEnumerator enumerator = l.SelectedItems.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    selector.SelectedItems.Add(enumerator.Current);
                }
                selector.SelectionChanged += LongListSelectionChanged;
                this.PivotMain.IsLocked = true;
                this.loadDeleteAppbar();
            }

            //"Alles auswählen" markieren oder nicht
            if (selector.SelectedItems.Count < selector.ItemsSource.Count)
            {
                ((CheckBox)parent.Children[0]).Unchecked -= selectAllCheckBox_Unchecked;
                ((CheckBox)parent.Children[0]).IsChecked = false;
                ((CheckBox)parent.Children[0]).Unchecked += selectAllCheckBox_Unchecked;
            }
            if (selector.SelectedItems.Count == selector.ItemsSource.Count)
            {
                ((CheckBox)parent.Children[0]).Unchecked -= selectAllCheckBox_Unchecked;
                ((CheckBox)parent.Children[0]).IsChecked = true;
                ((CheckBox)parent.Children[0]).Unchecked += selectAllCheckBox_Unchecked;
            }
            
        }

        /// <summary>
        /// Auswahl aller selektierten Bände soll aufgehoben werden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.PivotMain.IsLocked = false;
            FrameworkElement c = (FrameworkElement)((FrameworkElement)sender).Parent;

            IEnumerator enumeration = c.GetVisualChildren().GetEnumerator();
            while (enumeration.MoveNext())
            {
                if (enumeration.Current.IsTypeOf(new LongListMultiSelector()))
                {
                    LongListMultiSelector tmp = ((LongListMultiSelector)enumeration.Current);
                    tmp.SelectedItems.Clear();
                }
            }
            this.loadBookAppBar();
            books_VM.addAddTome(PivotMain.SelectedItem as datawrapper.Book);
        }


        /// <summary>
        /// Alle Bände sollen selektiert werden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Grid g = ((FrameworkElement)sender).Parent as Grid;

            LongListMultiSelector selector = g.Children[1] as LongListMultiSelector;
            selector.SelectionChanged -= LongListSelectionChanged;
            books_VM.removeAddTome(PivotMain.SelectedItem as datawrapper.Book);
            selector.ItemsSource = null;
            selector.ItemsSource = (PivotMain.SelectedItem as datawrapper.Book).tomes;
            selector.EnforceIsSelectionEnabled = true;
            selector.SelectionChanged += LongListSelectionChanged;
            this.PivotMain.IsLocked = true;

            LongListMultiSelector tmp = g.Children[1] as LongListMultiSelector;
            IEnumerator items = tmp.ItemsSource.GetEnumerator();
            this.currentSelectList = tmp;
            tmp.SelectionChanged -= LongListSelectionChanged;
            tmp.SelectedItems.Clear();
            while (items.MoveNext())
            {
                tmp.SelectedItems.Add(items.Current);
            }
            tmp.SelectionChanged += LongListSelectionChanged;
            this.loadDeleteAppbar();
        }

        /// <summary>
        /// Die passende Appbar zum Löschen von Bänden wird geladen.
        /// </summary>
        private void loadDeleteAppbar()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBarIconButton delete = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));
            ApplicationBarIconButton cancel = new ApplicationBarIconButton(new Uri("/icons/cancel.png", UriKind.Relative));
            delete.Text = AppResources.AppBarDelete;
            delete.Click += deleteSelection;
            cancel.Text = AppResources.AppBarCancel;
            cancel.Click += cancelSelection;
            ApplicationBar.Buttons.Add(delete);
            ApplicationBar.Buttons.Add(cancel);
        }

        /// <summary>
        /// Die Selektion zum Löschen von Bänden wird abgebrochen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelSelection(object sender, EventArgs e)
        {
            if (currentSelectList != null)
            {
                currentSelectList.SelectedItems.Clear();
                books_VM.addAddTome(PivotMain.SelectedItem as datawrapper.Book);
                this.loadBookAppBar();
            }
        }


        /// <summary>
        /// Alle ausgewählten Bände werden gelöscht.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteSelection(object sender, EventArgs e)
        {
            if (currentSelectList != null)
            {
                this.PivotMain.IsLocked = false;
                IEnumerator enumerator = currentSelectList.SelectedItems.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    books_VM.deleteTome(((datawrapper.Tome)enumerator.Current).tomeID);
                }
                //books_VM.addAddTome(PivotMain.SelectedItem as datawrapper.Book);
                this.loadBookAppBar();
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn auf einen Band länger gedrückt wird. Selektion wird
        /// gestartet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListMultiSelector llms = (LongListMultiSelector)sender;
            currentSelectList = llms;
            FrameworkElement c = (FrameworkElement)e.OriginalSource;

            llms.SelectedItems.Add(((datawrapper.Tome)c.DataContext));
        }


    }
}