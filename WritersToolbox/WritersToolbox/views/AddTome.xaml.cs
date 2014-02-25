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

namespace WritersToolbox.views
{
    public partial class AddTome : PhoneApplicationPage
    {
        //Verbindung zwischen View und Model des Bandes.
        private AddTomeViewModel Addtome_VM;
        //Primäreschlüßel des bandes.
        private int tomeID;
        //Textbox Chapter
        TextBox b = new TextBox();

        public AddTome()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Beim Navigieren zu dieser Seite wird das ausgewählte Objekt aus
        /// dem Navigationskontext herausgefiltert und die Details dazu mit dem
        /// Viewmodel geladen.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("tomeID"))
            {
                tomeID = int.Parse(NavigationContext.QueryString["tomeID"]);
                Addtome_VM = new AddTomeViewModel();
                DataContext = Addtome_VM;
                //     int code = Addtome_VM.getInformation();

            }
            //llstructure.ItemsSource = tome_VM.getStructure();
        }

        /// <summary>
        /// Wird unmittelbar aufgerufen, nachdem die Page entladen und nicht mehr 
        /// die aktuelle Quelle eines übergeordneten Frame ist.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //Ausgewählte Information in der Datenbank aktualisieren.
            //tome_VM.updateInformation(informationCode);
        }

    
        /// <summary>
        /// Ereignislist aufklappen bzw. zusammenklappen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image img = sender as Image;
            Grid parent = (img.Parent as Grid).Parent as Grid;
            LongListMultiSelector llms = ((parent.Children[1]) as LongListMultiSelector);
            if (llms.Visibility == Visibility.Collapsed)    //Ist die Ereignislist zusammengeklappt, 
            {                                               //dann wird sie aufgeklappt.
                img.Source = new BitmapImage(new Uri("/icons/on.png", UriKind.RelativeOrAbsolute));
                llms.Visibility = Visibility.Visible;
            }
            else      //Ist die Ereignislist aufgeklappt,
            {         //dann wird sie zusammenfeklappt.
                img.Source = new BitmapImage(new Uri("/icons/off.png", UriKind.RelativeOrAbsolute));
                llms.Visibility = Visibility.Collapsed;
            }
        }






        private void saveButton_Click(object sender, EventArgs e)
        {
            //save
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            //cancel
        }
    }
}
