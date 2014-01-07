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
using System.Diagnostics;
using WritersToolbox.models;


namespace WritersToolbox.views
{
    public partial class Types : PhoneApplicationPage
    {

        public static TypesViewModel types_VM = null;
        /// <summary>
        /// ViewModel für Types und TypesOverview wird erstellt.
        /// </summary>
        public static TypesViewModel Types_VM
        {
            get
            {
                if (types_VM == null)
                {
                    types_VM = new TypesViewModel();
                    if (!types_VM.IsDataLoaded)
                        types_VM.LoadData();

                }
                return types_VM;
            }
        }


        public Types()
        {
            InitializeComponent();
            // ViewModel wird der View als DataContext zugewiesen
            DataContext = Types_VM;
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
                    NavigationService.Navigate(new Uri("/views/TypesOverview.xaml", UriKind.Relative));
                }
                else
                    System.Diagnostics.Debug.WriteLine("Zoomin");

            }


        }
        /// <summary>
        /// Hilfsmethode solange man Zoom nicht testen kann.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navUeberblick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/TypesOverview.xaml", UriKind.Relative));
        }



        /// <summary>
        /// Die Methode wird aufgerufen, wenn ein Item im LongListSelector angeklickt wurde.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            if (selector == null)
                return;
            datawrapper.TypeObject to = selector.SelectedItem as datawrapper.TypeObject;
            if (to == null)
                return;

            // ein Objekt Type hat TypID = -1
            if (to.type.typeID == -1)
            {
                NavigationService.Navigate(new Uri("/views/AddType.xaml", UriKind.Relative));
            }
            // ein Objekt TypeObject hat TypID = -2
            else if (to.type.typeID == -2)
            {
                NavigationService.Navigate(new Uri("/views/AddTypeObject.xaml?typeID=" + (PivotMain.SelectedIndex + 1), UriKind.Relative));
            }
            // detailansicht fuer typobject
            else 
            {
                NavigationService.Navigate(new Uri("/views/TypeObjectDetails.xaml?item=" + to.typeObjectID, UriKind.Relative));
            }
            selector.SelectedItem = null;
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
                PivotMain.SelectedIndex = indexParsed - 1;
            }
            else if (PhoneApplicationService.Current.State.ContainsKey("NewType"))
            {
                PivotMain.SelectedIndex = PivotMain.Items.Count - 2;
            }
                
           
            
        
        }

        /// <summary>
        /// Die Methode wird bei einem Hold-Event auf ein TypObjekt aufgerufen, ermittelt die 
        /// jeweilige TypObjektID und übergibt diese dem ViewModel zum Löschen des TypObjekts.
        /// Vorher wird eine Abfrage erzeugt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteTypeObject(object sender, System.Windows.Input.GestureEventArgs e)
        {
            datawrapper.TypeObject to = (sender as Grid).DataContext as datawrapper.TypeObject;
            if (to == null)
                return;
            MessageBoxResult result = MessageBox.Show("Wollen Sie das Typobjekt wirklich löschen?",
            "Typobjekt löschen", MessageBoxButton.OKCancel);
            if(result == MessageBoxResult.OK)
                Types_VM.deleteTypeObject(to.typeObjectID);
        }

    }
}