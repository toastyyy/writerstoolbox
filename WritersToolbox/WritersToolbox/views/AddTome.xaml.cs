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
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Input;
using WritersToolbox.models;

namespace WritersToolbox.views
{
    public partial class AddTome : PhoneApplicationPage
    {
        //Verbindung zwischen View und Model des Bandes.
        private AddTomeViewModel atvm;
        //Primäreschlüssel des bandes.
        private int tomeID;
        //Primärschlüssel des Werkes.
        private int bookID;
        //Textbox Chapter
        TextBox b = new TextBox();
        private datawrapper.BookType BookType;

        public AddTome()
        {
            InitializeComponent();
            atvm = new AddTomeViewModel();
        }

        /// <summary>
        /// Beim Navigieren zu dieser Seite wird das ausgewählte Objekt aus
        /// dem Navigationskontext herausgefiltert und die Details dazu mit dem
        /// Viewmodel geladen.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
   //         base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("bookID"))
            {
              //  bookType = int.Parse(NavigationContext.QueryString["bookType"]);
                bookID = int.Parse(NavigationContext.QueryString["bookID"]);
              //  atvm = new AddTomeViewModel();
              //  DataContext = atvm;

            }
        }

        /// <summary>
        /// Wird unmittelbar aufgerufen, nachdem die Page entladen und nicht mehr 
        /// die aktuelle Quelle eines übergeordneten Frame ist.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            //Hilfsvariable für die Kontrolle der Änderungen.
            bool isChanged = false;
            //Änderungen kontrollieren.
            if (!titleTextBox.Text.Trim().Equals("") || !titleTextBox.Text.Trim().ToUpper().Equals("TITLE"))
            {
                isChanged = true;
            }

            //Wenn nichts geändert wurde, dann wird eine Message für Benutzer gezeigt,
            //sonst die Notiz speichern, die Hilfsvariable in ApplicationService löschen
            //und zurück zu dem vorherigen Screen.
            if (!isChanged)
            {
                MessageBox.Show("Sie müssen mindestens Title der für ein Buch eingeben!!");
            }
            else
            {
                //Wenn in Title nicht geändert wurde, dann wird automatisch der aktuelle Datum für Title gegeben.
                string title = (titleTextBox.Text.Trim().Equals("") || titleTextBox.Text.Trim().ToUpper().Equals("TITLE"))
                    ? DateTime.Now.ToString("F")
                    : titleTextBox.Text.Trim();      
                    //Notiz spiechern.
                    //atvm.save(tomeID, DateTime.Now, title, DateTime.Now);
                

                //Hilfsvariable in ApplicationService löschen.

                //PhoneApplicationService.Current.State.Remove("tomeID");
                BooksViewModel bvm = new BooksViewModel();
                Book b = bvm.getBookByID(this.bookID);
                bvm.addTome(title, this.bookID, b.obj_bookType.bookTypeID);
                NavigationService.GoBack();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// Wenn Title der Notiz Fokus bekommt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void titleTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //Hintergrund dynamisch ändern.
            titleTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xCC, 0x63, 0x61, 0x61));
            SolidColorBrush _s = new SolidColorBrush(Colors.Transparent);
            titleTextBox.Background = _s;
            if (titleTextBox.Text.ToString().Trim().ToUpper().Equals("TITLE"))
            {
                titleTextBox.Text = "";
            }
        }

        /// <summary>
        /// Wenn Title der Notiz FoKus verliert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void titleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //Border des Textbox dynamisch ändern.
            titleTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0x33, 0x63, 0x61, 0x61));
            if (titleTextBox.Text.Trim().Length == 0)
            {
                titleTextBox.Text = "Title";
            }
        }   
    }
}
