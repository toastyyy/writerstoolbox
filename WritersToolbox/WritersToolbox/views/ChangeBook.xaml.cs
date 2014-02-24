using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media;
using WritersToolbox.models;

namespace WritersToolbox.views
{
    public partial class ChangeBook : PhoneApplicationPage
    {
        private Book book;

        private datawrapper.BookType tmpBooktype;

        private Grid lastSelectedGrid;

        public ChangeBook()
        {
            DataContext = Books.Books_VM;
            InitializeComponent();
        }
        /// <summary>
        /// Ein Buch wird geändert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateBook(object sender, EventArgs e)
        {
            String name = bName.Text;

            try
            {
                Books.Books_VM.updateBook(book.bookID, name, book.obj_bookType.bookTypeID);
                NavigationService.GoBack();
            }
            catch (ArgumentException ae)
            {
                MessageBox.Show(ae.Message, "Fehler", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Cancelt die Erstellungen eines Buchs und geht eine Seite zurück.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBook(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// Wird auf diese Page naviert, überprüft die Methode, welches Buch geladen
        /// werden soll.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("item"))
            {
                var item = NavigationContext.QueryString["item"];
                var indexParsed = int.Parse(item);
                book = Books.Books_VM.getBookByID(indexParsed);
                bName.Text = book.name;
                booktype.Text = book.obj_bookType.name;
            }


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
            tmpBooktype = null;
            if (lastSelectedGrid != null)
            {
                lastSelectedGrid.Background = new SolidColorBrush(Colors.Transparent);
                lastSelectedGrid = null;
            }
            
        }

        /// <summary>
        /// In der Buchtypauswahl wird eine auswahl getroffen und temporär gespeichert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BookTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tmpBooktype = (sender as LongListSelector).SelectedItem as datawrapper.BookType;
        }

        /// <summary>
        /// Die Buchtypauswahl wird gespeichert. Aus der temporären Auswahl werden die benötigten Werte
        /// gespeichert. Die Visuelle Auswahl wird augehoben und das Popup geschlossen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveNewBookType(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (tmpBooktype != null)
            {
                book.obj_bookType.bookTypeID = tmpBooktype.bookTypeID;
                booktype.Text = tmpBooktype.name;
            }
            if (lastSelectedGrid != null)
            {
                lastSelectedGrid.Background = new SolidColorBrush(Colors.Transparent);
                lastSelectedGrid = null;
            }
            booktype_popup.IsOpen = false;
        }

        /// <summary>
        /// In der Buchtypauswahl ist eine visuelle Auswahl möglich.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void highlightSelection(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (lastSelectedGrid != null)
            {
                lastSelectedGrid.Background = new SolidColorBrush(Colors.Transparent);
            }
            Grid g = sender as Grid;
            g.Background = new SolidColorBrush(Colors.Brown);
            lastSelectedGrid = g;
        }
    }
}