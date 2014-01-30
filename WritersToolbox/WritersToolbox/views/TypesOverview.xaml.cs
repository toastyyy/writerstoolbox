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
using System.Collections.ObjectModel;
using System.ComponentModel;
using WritersToolbox.models;
using System.Diagnostics;
namespace WritersToolbox.views
{
    public partial class TypesOverview : PhoneApplicationPage
    {
        private datawrapper.Type selectedType;

        public TypesOverview()
        {
            InitializeComponent();
            //der Seite wird der DataContext von Types zugewiesen
            DataContext = Types.Types_VM;
        }


        /// <summary>
        /// Die Methode erkennt die Zoomout-Geste und navigiert zu Types
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pinch_out(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                if (e.PinchManipulation.CumulativeScale < 1d)
                {
                    System.Diagnostics.Debug.WriteLine("Zoomin");
                    NavigationService.Navigate(new Uri("/views/Types.xaml", UriKind.Relative));
                }
                else
                    System.Diagnostics.Debug.WriteLine("Zoomout");

            }
        }

        /// <summary>
        /// Die Methode wird aufgerufen, wenn ein Item im LongListSelector angeklickt wurde.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            if (selector == null)
                return;
            datawrapper.Type t = selector.SelectedItem as datawrapper.Type;
            if (t == null)
                return;
            // dem Navigationspfad wird angehängt, welches item geklickt wurde und zu welchem Pivotitem naviert werden soll
            NavigationService.Navigate(new Uri("/views/Types.xaml?item=" + t.typeID, UriKind.Relative));
            selector.SelectedItem = null;
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Die Methode wird bei einem Hold-Event auf einen Typ aufgerufen, ermittelt die 
        /// jeweilige TypID und übergibt diese dem ViewModel zum Löschen des Typs.
        /// Vorher wird eine Abfrage erzeugt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TryDeleteType(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedType = (sender as Grid).DataContext as datawrapper.Type;
            if (selectedType == null)
                return;
            if (selectedType.typeID == -1)
                return;
            TypeDeleteQuestion.Text = "Wollen Sie den Typ \"" + selectedType.title.ToString() + "\" löschen?";
            deleteTypePopup.IsOpen = true;
        }

        private void DeleteType(object sender, EventArgs e)
        {
            Types.Types_VM.deleteType(selectedType.typeID);
            deleteTypePopup.IsOpen = false;
        }

        private void DoNotDeleteType(object sender, EventArgs e)
        {
            deleteTypePopup.IsOpen = false;
        }


    }
}