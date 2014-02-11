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

namespace WritersToolbox.views
{
    public partial class BookDetails : PhoneApplicationPage
    {

        private TomeDetailViewModel tome_VM;

        public BookDetails()
        {
            InitializeComponent();
            
        }

        private void charsAndPlaces(object sender, RoutedEventArgs e)
        {
            //Chars and Places view
        }

        private void bookSettings(object sender, RoutedEventArgs e)
        {
            //settings view
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
                int tomeID = int.Parse(NavigationContext.QueryString["tomeID"]);
                tome_VM = new TomeDetailViewModel(tomeID);
                DataContext = tome_VM;
                bookTitle.Text = tome_VM.getTitle();
           //     Console.WriteLine(tome_VM.ToString());
            //    Console.WriteLine(tome_VM.getTitle());
            }
        }

        private void editBookDetailsClick(object sender, EventArgs e)
        {
         //   NavigationService.Navigate(new Uri("/views/TypeObjectEdit.xaml?typeObjectID=" + this.tdvm.TypeObject.typeObjectID, UriKind.Relative));
        }

        private void TryDeleteBook(object sender, EventArgs e)
        {
         //   TypeObjectDeleteQuestion.Text = "Wollen Sie das Typobjekt \"" + tdvm.TypeObject.name.ToString() + "\" löschen?";
          //  deleteTypeObjectPopup.IsOpen = true;
        }
    }
}
