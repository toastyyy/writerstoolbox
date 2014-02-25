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
using System.Collections;


namespace WritersToolbox.views
{
    public partial class Types : PhoneApplicationPage
    {
        private TextBox newTypeTitle = new TextBox();
        private ListBox colorPicker;
        private datawrapper.TypeObject holdTypeobject;
        public static TypesViewModel types_VM = null;

        private LongListMultiSelector currentSelectList = null;
        private datawrapper.Type currentType = null;
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
            if (!this.PivotMain.IsLocked) {
                NavigationService.Navigate(new Uri("/views/TypesOverview.xaml", UriKind.Relative));            
            }
        }



        /// <summary>
        /// Die Methode wird aufgerufen, wenn ein Item im LongListSelector angeklickt wurde.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListMultiSelector selector = sender as LongListMultiSelector;
            Grid parent = selector.Parent as Grid;
            if (selector == null)
                return;
            for (int i = 0; i < e.RemovedItems.Count; i++)
            {
                selector.SelectedItems.Remove(e.RemovedItems[i]);
            }

            if (selector.SelectedItems.Count == 0)
            {
                types_VM.addAddTypeObject(this.currentType);
                this.restoreApplicationBar();
                this.PivotMain.IsLocked = false;
            }
            else
            {
                this.PivotMain.IsLocked = true;
            }

            if (selector.SelectedItems.Count < selector.ItemsSource.Count) {
                ((CheckBox)parent.Children[0]).Unchecked -= selectAllCheckBox_Unchecked;
                ((CheckBox)parent.Children[0]).IsChecked = false;
                ((CheckBox)parent.Children[0]).Unchecked += selectAllCheckBox_Unchecked;
            }
            if (selector.SelectedItems.Count == selector.ItemsSource.Count) {
                ((CheckBox)parent.Children[0]).Unchecked -= selectAllCheckBox_Unchecked;
                ((CheckBox)parent.Children[0]).IsChecked = true;
                ((CheckBox)parent.Children[0]).Unchecked += selectAllCheckBox_Unchecked;
            }
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
                    Text = "ZU TYPOBJEKT ZUORDNEN",
                    FontFamily = new FontFamily("Segoe UI Light")
                };
                
            }
            else {
                ApplicationBar.Buttons.Clear();

                ApplicationBarIconButton edit = new ApplicationBarIconButton(new Uri("/icons/edit.png", UriKind.Relative));
                ApplicationBarIconButton delete = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));
                delete.Text = "Löschen";
                edit.Text = "Bearbeiten";
                edit.Click += ChangeType;
                delete.Click += TryDeleteType;
                ApplicationBar.Buttons.Add(edit);
                ApplicationBar.Buttons.Add(delete);
                this.PivotMain.Title = new TextBlock()
                {
                    FontSize = 22,
                    Margin = new Thickness(9, 0, 0, 0),
                    Text = "TYPEN",
                    FontFamily = new FontFamily("Segoe UI Light")
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
        private void OpenTypeObjectDetail(object sender, System.Windows.Input.GestureEventArgs e)
        {
            holdTypeobject = (sender as Grid).DataContext as datawrapper.TypeObject;
            if (holdTypeobject == null)
                return;

            if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                PhoneApplicationService.Current.State["typeObjectID"] = holdTypeobject.typeObjectID;
                NavigationService.GoBack();
                return;
            }
            // ein Objekt TypeObject hat TypID = -2
            else if (holdTypeobject.type.typeID == -2)
            {
                NavigationService.Navigate(new Uri("/views/TypeObjectAdd.xaml?typeID=" + (this.currentType.typeID), UriKind.Relative));
            }
            // detailansicht fuer typobject
            else
            {
                NavigationService.Navigate(new Uri("/views/TypeObjectDetails2.xaml?typeObjectID=" + holdTypeobject.typeObjectID, UriKind.Relative));
            }
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
                Types.types_VM.createType(title, color, "/icons/character.png");
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
            this.currentType = t;
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
                this.currentSelectList = null;
            }
            else
            {
                btn1.IconUri = new Uri("/icons/edit.png", UriKind.Relative);
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

        private void LongList_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListMultiSelector llms = (LongListMultiSelector)sender;
            currentSelectList = llms;
            FrameworkElement c = (FrameworkElement) e.OriginalSource;
            while (!llms.Parent.IsTypeOf(new Grid())) { };
            Grid g = (Grid)c.Parent;
            Types_VM.removeAddTypeObject(this.currentType);

            llms.SelectedItems.Add(((datawrapper.TypeObject)g.DataContext));

            ApplicationBar.Buttons.Clear();

            ApplicationBarIconButton delete = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));
            ApplicationBarIconButton cancel = new ApplicationBarIconButton(new Uri("/icons/cancel.png", UriKind.Relative));
            delete.Text = "löschen";
            delete.Click += deleteSelection;
            cancel.Text = "abbrechen";
            cancel.Click += cancelSelection;
            ApplicationBar.Buttons.Add(delete);
            ApplicationBar.Buttons.Add(cancel);
        }

        private void cancelSelection(object sender, EventArgs e) {
            if (currentSelectList != null) { 
                currentSelectList.SelectedItems.Clear();
                types_VM.addAddTypeObject(this.currentType);
                this.restoreApplicationBar();
            } 
        }

        private void deleteSelection(object sender, EventArgs e) {
            if (currentSelectList != null) {
                IEnumerator enumerator = currentSelectList.SelectedItems.GetEnumerator();
                while (enumerator.MoveNext()) {
                    types_VM.deleteTypeObject(((datawrapper.TypeObject)enumerator.Current).typeObjectID);
                }
                types_VM.addAddTypeObject(this.currentType);
                this.restoreApplicationBar();
            }
        }

        private void restoreApplicationBar() {
            ApplicationBarIconButton btn1 = new ApplicationBarIconButton();
            ApplicationBarIconButton btn2 = new ApplicationBarIconButton();

            btn1.IconUri = new Uri("/icons/edit.png", UriKind.Relative);
            btn1.Text = "ändern";
            btn1.Click -= new EventHandler(SaveType);
            btn1.Click += new EventHandler(ChangeType);
            btn2.IconUri = new Uri("/icons/delete.png", UriKind.Relative);
            btn2.Text = "löschen";
            btn2.Click -= new EventHandler(CancelType);
            btn2.Click += new EventHandler(TryDeleteType);

            ApplicationBar.Buttons.Clear();
            ApplicationBar.Buttons.Add(btn1);
            ApplicationBar.Buttons.Add(btn2);
        }

        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.PivotMain.IsLocked = true;
            types_VM.removeAddTypeObject(this.currentType);

            Grid g = ((FrameworkElement) sender).Parent as Grid;

            LongListMultiSelector tmp = g.Children[1] as LongListMultiSelector;
            IEnumerator items = tmp.ItemsSource.GetEnumerator();
            this.currentSelectList = tmp;
            tmp.SelectionChanged -= LongList_SelectionChanged;
            tmp.SelectedItems.Clear();
            while (items.MoveNext()) {
                tmp.SelectedItems.Add(items.Current);
            }
            tmp.SelectionChanged += LongList_SelectionChanged;
            ApplicationBar.Buttons.Clear();

            ApplicationBarIconButton delete = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));
            ApplicationBarIconButton cancel = new ApplicationBarIconButton(new Uri("/icons/cancel.png", UriKind.Relative));
            delete.Text = "löschen";
            delete.Click += deleteSelection;
            cancel.Text = "abbrechen";
            cancel.Click += cancelSelection;
            ApplicationBar.Buttons.Add(delete);
            ApplicationBar.Buttons.Add(cancel);
        }

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
            this.restoreApplicationBar();
            types_VM.addAddTypeObject(this.currentType);
        }
    }
}