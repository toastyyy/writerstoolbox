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
using System.Windows.Media;
using Coding4Fun.Toolkit.Controls;


namespace WritersToolbox.views
{
    public partial class Types : PhoneApplicationPage
    {


        private TextBox newTypeTitle = new TextBox();
        private ListBox colorPicker;
        private datawrapper.TypeObject holdTypeobject;
        public static TypesViewModel types_VM = null;

        //bei selectionChanged wird Farbe hier zwischengespeichert
        private Color selectedColor;

        //Farben für Colorpicker
        static string[] colors =
        { 
	        "#FFFFE135","#FFFFFF66","#FF008A00","#FF32CD32","#FF00FF7F","#FF808000",
            "#FFFF0000","#FFFF4500","#FFFF8C00", "#FFFF7F50","#FFDC143C","#FFFF1493",
            "#FFB22222","#FFC71585","#FFDA70D6","#FF000080","#FF4B0082","#FF800080",
            "#FFADD8E6","#FF20B2AA","#FF008080"
        };

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
            if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                PhoneApplicationService.Current.State["typeObjectID"] = to.typeObjectID;
                NavigationService.Navigate(new Uri("/views/AddNote.xaml", UriKind.Relative));
                return;
            }
            // ein Objekt TypeObject hat TypID = -2
            else if (to.type.typeID == -2)
            {
                NavigationService.Navigate(new Uri("/views/AddTypeObject.xaml?typeID=" + (PivotMain.SelectedIndex + 1), UriKind.Relative));
            }
            // detailansicht fuer typobject
            else
            {
                NavigationService.Navigate(new Uri("/views/TypeObjectDetails2.xaml?typeObjectID=" + to.typeObjectID, UriKind.Relative));
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
            types_VM.LoadData();
            if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                ApplicationBar.Buttons.Clear();
                ApplicationBarIconButton cancel = new ApplicationBarIconButton(new Uri("/icons/cancel.png", UriKind.Relative));
                cancel.Text = "Abbrechen";
                ApplicationBar.Buttons.Add(cancel);
                this.PivotMain.Title = new TextBlock() { 
                    FontSize = 22,
                    Margin = new Thickness(9,0,0,0),
                    Text = "ZU TYPOBJEKT ZUORDNEN"

                };
                
            }
            else {
                ApplicationBar.Buttons.Clear();

                ApplicationBarIconButton edit = new ApplicationBarIconButton(new Uri("/icons/saveAs.png", UriKind.Relative));
                ApplicationBarIconButton delete = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));
                delete.Text = "Löschen";
                edit.Text = "Bearbeiten";
                ApplicationBar.Buttons.Add(edit);
                ApplicationBar.Buttons.Add(delete);
                this.PivotMain.Title = new TextBlock()
                {
                    FontSize = 22,
                    Margin = new Thickness(9, 0, 0, 0),
                    Text = "TYPEN"

                };
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
            String r = selectedColor.R.ToString("X2");
            String g = selectedColor.G.ToString("X2");
            String b = selectedColor.B.ToString("X2");

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
        /// Nach dem die Seite geladen ist, werden alle angegebenen Farben in die Listbox geladen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPickerPage_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox l = sender as ListBox;
            List<ColorItem> item = new List<ColorItem>();
            for (int i = 0; i < colors.Length; i++)
            {
                item.Add(new ColorItem() { Color = fromHexToColor(colors[i]) });
            };

            l.ItemsSource = item; //Fill ItemSource with all colors
        }

        /// <summary>
        /// Umwandlung von hex schreibweise in Color.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private Color fromHexToColor(String hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);
            Byte a = Convert.ToByte(hex.Substring(0, 2), 16);
            Byte colorR = Convert.ToByte(hex.Substring(2, 2), 16);
            Byte colorG = Convert.ToByte(hex.Substring(4, 2), 16);
            Byte colorB = Convert.ToByte(hex.Substring(6, 2), 16);

            return Color.FromArgb(a, colorR, colorG, colorB);
        }


        /// <summary>
        /// Anonyme Klasse in der die Farbe gespeichert wird.
        /// </summary>
        public class ColorItem
        {
            public Color Color { get; set; }
        }


        /// <summary>
        /// Wird eine Farbe in der Listbox ausgewählt, wird diese unter 
        /// selectedColor gespeichert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox l = sender as ListBox;
            ColorItem c = l.SelectedItem as ColorItem;
            selectedColor = c.Color;
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
        /// Beim Ändern des aktuellen Pivotitems wird die Appbar angepasst, 
        /// Icons sowie Eventhandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!PhoneApplicationService.Current.State.ContainsKey("assignNote"))
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
            
        }

        private void layoutContent_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {

        }

    }
}