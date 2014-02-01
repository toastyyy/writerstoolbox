﻿using System;
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
using System.Windows.Media;
using Coding4Fun.Toolkit.Controls;


namespace WritersToolbox.views
{
    public partial class Types : PhoneApplicationPage
    {


        private TextBox newTypeTitle = new TextBox();
        private ColorPicker picker = new ColorPicker();
        private datawrapper.TypeObject holdTypeobject;
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
                if (indexParsed == -1)
                {
                    //wurde ein neuer Typ erzeugt, wird dorthin navigiert
                    PivotMain.SelectedIndex = Types_VM.getTypeCount() - 1;
                } else 
                    //es wird zum ausgewählten Typ navigiert
                    PivotMain.SelectedIndex = indexParsed - 1;
            }
                
           
        }

        /// <summary>
        /// Die Methode wird bei einem Hold-Event auf ein TypObjekt aufgerufen, ermittelt die 
        /// jeweilige TypObjektID und übergibt diese dem ViewModel zum Löschen des TypObjekts.
        /// Vorher wird eine Abfrage erzeugt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TryDeleteTypeObject(object sender, System.Windows.Input.GestureEventArgs e)
        {
            holdTypeobject = (sender as Grid).DataContext as datawrapper.TypeObject;
            if (holdTypeobject == null)
                return;
            TypeObjectDeleteQuestion.Text = "Wollen Sie das Typobjekt \"" + holdTypeobject.name.ToString() + "\" löschen?";
            deleteTypeObjectPopup.IsOpen = true;
        }

        /// <summary>
        /// Typobjekt wird endgültig gelöscht und die Sicherheitsabfrage geschlossen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteTypeObject(object sender, EventArgs e)
        {
            Types.Types_VM.deleteTypeObject(holdTypeobject.typeObjectID);
            deleteTypeObjectPopup.IsOpen = false;
        }


        /// <summary>
        /// Der Löschvorgang wird abgebrochen und die Sicherheitsfrage geschlossen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoNotDeleteTypeObject(object sender, EventArgs e)
        {
            deleteTypeObjectPopup.IsOpen = false;
        }

        /// <summary>
        /// Ein neuer Typ wird erzeugt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveType(object sender, EventArgs e)
        {
            Color c = picker.Color;

            String r = c.R.ToString("X2");
            String g = c.G.ToString("X2");
            String b = c.B.ToString("X2");

            String color = "#" + r + g + b;
            String title = newTypeTitle.Text;
            try 
            { 
                Types.types_VM.createType(title, color, "");
                //zum gerade erzeugten Typ navigieren
                PivotMain.SelectedIndex = PivotMain.Items.Count - 2;
            }
            catch (ArgumentException ae) 
            {
                MessageBox.Show(ae.Message, "Fehler", MessageBoxButton.OK);
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// Cancelt die Erstellungen eines Typs und geht eine Seite zurück.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelType(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }


        /// <summary>
        /// Es wurde auf Typändern geklickt und zur entsprechenden View weitergeleitet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeType(object sender, EventArgs e)
        {
            datawrapper.Type t =  PivotMain.SelectedItem as datawrapper.Type;
            if (t == null)
                return;
            NavigationService.Navigate(new Uri("/views/ChangeType.xaml?item=" + t.typeID , UriKind.Relative));
            
        }

        /// <summary>
        /// In der Appbar wurde das Event Typlöschen ausgelöst und die Sicherheitsabfrage angezeigt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TryDeleteType(object sender, EventArgs e)
        {
            datawrapper.Type t = PivotMain.SelectedItem as datawrapper.Type;
            if (t == null)
                return;
            TypeDeleteQuestion.Text = "Wollen Sie den Typ \"" + t.title.ToString() +  "\" löschen?";
            deleteTypePopup.IsOpen = true;
        }


        /// <summary>
        /// Ein Typ wird endgültig gelöscht und die Sicherheitsabfrage geschlossen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteType(object sender, EventArgs e)
        {
            datawrapper.Type t = PivotMain.SelectedItem as datawrapper.Type;
            if (t == null)
                return;
            Types.Types_VM.deleteType(t.typeID);
            deleteTypePopup.IsOpen = false;
        }


        /// <summary>
        /// Der Löschvorgang für einen Typ wird abgebrochen und die Sicherheitsfrage geschlossen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoNotDeleteType(object sender, EventArgs e)
        {
            deleteTypePopup.IsOpen = false;
        }


        /// <summary>
        /// Bei der Erstellung eines neuen Typs wurde ein Titel eingegeben und somit
        /// die TextBox für die spätere Speicherung ermittelt.
        /// Grund dieses Vorgehens: TextBox ist in Template und kann nicht direkt angesprochen werden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleGotFocus(object sender, RoutedEventArgs e)
        {
            newTypeTitle = sender as TextBox;
            
        }


        /// <summary>
        /// Bei der Erstellung eines neuen Typs wurde eine Farbe ausgewählt und somit
        /// der ColorPicker für die spätere Speicherung ermittelt.
        /// Grund dieses Vorgehens: ColorPicker ist in Template und kann nicht direkt angesprochen werden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="color"></param>
        private void ColorChanged(object sender, Color color)
        {
            picker = sender as ColorPicker;
        }


        /// <summary>
        /// Beim Ändern des aktuellen Pivotitems wird die Appbar angepasst, 
        /// Icons sowie Eventhandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot p = sender as Pivot;
            if (p == null)
                return;
            datawrapper.Type t = p.SelectedItem as datawrapper.Type;
            if (t == null)
                return;
            ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            if (t.typeID == -1)
            {
                btn1.IconUri = new Uri("/icons/save.png", UriKind.Relative);
                btn1.Text = "speichern";
                btn1.Click -= new EventHandler(ChangeType);
                btn1.Click += new EventHandler(SaveType);
                btn2.IconUri = new Uri("/icons/cancel.png", UriKind.Relative);
                btn2.Text = "abbrechen";
                btn2.Click -= new EventHandler(TryDeleteType);
                btn2.Click += new EventHandler(CancelType);
            }
            else
            {
                btn1.IconUri = new Uri("/icons/speichernUnter.png", UriKind.Relative);
                btn1.Text = "ändern";
                btn1.Click -= new EventHandler(SaveType);
                btn1.Click += new EventHandler(ChangeType);
                btn2.IconUri = new Uri("/icons/delete.png", UriKind.Relative);
                btn2.Text = "löschen";
                btn2.Click -= new EventHandler(CancelType);
                btn2.Click += new EventHandler(TryDeleteType);
            }
        }

        private void layoutContent_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {

        }

    }
}