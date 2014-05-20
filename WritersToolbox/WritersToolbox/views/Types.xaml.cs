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
using WritersToolbox.Resources;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;


namespace WritersToolbox.views
{
    public partial class Types : PhoneApplicationPage
    {
        private TextBox newTypeTitle = new TextBox();
        private ListBox iconPicker;
        private datawrapper.TypeObject holdTypeobject;
        public static TypesViewModel types_VM = null;

        private datawrapper.Type restoreType;

        private LongListMultiSelector currentSelectList = null;
        private datawrapper.Type currentType = null;
        //bei selectionChanged wird Farbe hier zwischengespeichert
        private String selectedImage;

        private PhotoChooserTask photoChooserTask;

        //Farben für Colorpicker
        static List<String> icons;

        private int currentApplicationbarId = 1; // Applicationbar Id; 1 = normal, 2 = selection

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
            // PhotochooserTask initialisieren (für neuen Typ)
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            icons = this.loadIconSettings();
        }

        public void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                if (icons.IndexOf(e.OriginalFileName) < 0) {
                    this.selectedImage = e.OriginalFileName;
                    icons.Add(icons.Last<String>());
                    icons[icons.Count() - 2] = e.OriginalFileName;
                    List<IconItem> iconList = (List<IconItem>)iconPicker.ItemsSource;
                    IconItem newIcon = new IconItem() { imagePath = e.OriginalFileName };
                    IconItem addIcon = iconList.Last();
                    iconPicker.SelectionChanged -= IconPicker_SelectionChanged;
                    iconPicker.ItemsSource = null;
                    iconList.Remove(addIcon);
                    iconList.Add(newIcon);
                    iconList.Add(addIcon);
                    iconPicker.ItemsSource = iconList;
                    iconPicker.SelectedItem = newIcon;
                    iconPicker.SelectionChanged += IconPicker_SelectionChanged;
                    this.saveIconSettings();
                }
            }
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
        /// Die Methode wird aufgerufen, wenn ein Item im LongListSelector angeklickt wurde.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListMultiSelector selector = sender as LongListMultiSelector;
            this.currentSelectList = selector;
            Grid parent = selector.Parent as Grid;
            if (selector == null)
                return;
            for (int i = 0; i < e.RemovedItems.Count; i++)
            {
                selector.SelectedItems.Remove(e.RemovedItems[i]);
            }

            // wenn kein Item selektiert wurde erstelle "normale" Application Bar und entsperre Pivot.
            // ansonsten sperre pivot und erstelle die entsprechende ApplicationBar.
            if (selector.SelectedItems.Count == 0)
            {
                types_VM.addAddTypeObject(this.currentType);
                this.setNormalApplicationBar();
                this.PivotMain.IsLocked = false;
            }
            else
            {
                this.setSelectionApplicationBar();
                this.PivotMain.IsLocked = true;
            }

            // setze wert für "alle auswählen" checkbox
            if (selector.SelectedItems.Count < selector.ItemsSource.Count) {
                ((CheckBox)parent.Children[0]).Unchecked -= selectAllCheckBox_Unchecked;
                ((CheckBox)parent.Children[0]).IsChecked = false;
                ((CheckBox)parent.Children[0]).Unchecked += selectAllCheckBox_Unchecked;
            } else {
                ((CheckBox)parent.Children[0]).Unchecked -= selectAllCheckBox_Unchecked;
                ((CheckBox)parent.Children[0]).IsChecked = true;
                ((CheckBox)parent.Children[0]).Unchecked += selectAllCheckBox_Unchecked;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            
            PhoneApplicationService.Current.State["RestoreData"] =
                new datawrapper.Type()
                {
                    title = newTypeTitle.Text,
                    imageString = selectedImage,
                };
            
        }

        /// <summary>
        /// Wird auf diese Page naviert, überprüft die Methode, ob zu einem gewünschten Pivotitem
        /// navigiert werden soll.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!PhoneApplicationService.Current.State.ContainsKey("chooseIcon"))
            {
                types_VM.LoadData();
                if (PhoneApplicationService.Current.State.ContainsKey("assignNote") ||
                    PhoneApplicationService.Current.State.ContainsKey("attachEvent"))
                {
                    ApplicationBar.Buttons.Clear();
                    ApplicationBarIconButton cancel = new ApplicationBarIconButton(new Uri("/icons/cancel.png", UriKind.Relative));
                    cancel.Text = AppResources.AppBarCancel;
                    cancel.Click += CancelAssignment;
                    ApplicationBar.Buttons.Add(cancel);
                    searchImage.Visibility = Visibility.Collapsed;
                    this.PivotMain.Title = new TextBlock()
                    {
                        FontSize = 22,
                        Margin = new Thickness(9, 0, 0, 0),
                        Text = AppResources.TypeObjectAssign,
                        FontFamily = new FontFamily("Segoe UI Light")
                    };

                }
                else
                {
                    this.setNormalApplicationBar();
                    this.PivotMain.Title = new TextBlock()
                    {
                        FontSize = 22,
                        Margin = new Thickness(9, 0, 0, 0),
                        Text = AppResources.TypesPageTitle,
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
                        }
                        else
                            //es wird zum ausgewählten Typ navigiert
                            PivotMain.SelectedIndex = indexParsed - 1;
                    }
                }
            }
            else {
                PhoneApplicationService.Current.State.Remove("chooseIcon");
            }
            

            if (PhoneApplicationService.Current.State.ContainsKey("tombstoned"))
            {
                if (PhoneApplicationService.Current.State.ContainsKey("RestoreData"))
                {
                    restoreType = (datawrapper.Type)PhoneApplicationService.Current.State["RestoreData"];
                    
                    
                    PhoneApplicationService.Current.State.Remove("RestoreData");
                }
                PhoneApplicationService.Current.State.Remove("tombstoned");
            }  
           
        }

        /// <summary>
        /// Die Methode wird bei einem Tap-Event auf ein TypObjekt aufgerufen, ermittelt die 
        /// jeweilige TypObjektID und navigiert zum TypObjekt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTypeObjectDetail(object sender, System.Windows.Input.GestureEventArgs e)
        {
            holdTypeobject = (sender as Grid).DataContext as datawrapper.TypeObject;
            if (holdTypeobject == null)
                return;

            if (PhoneApplicationService.Current.State.ContainsKey("assignNote") || PhoneApplicationService.Current.State.ContainsKey("attachEvent"))
            {
                if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
                {
                    if (types_VM.isExistNoteInTypeobject(holdTypeobject.typeObjectID, (PhoneApplicationService.Current.State["memoryNoteTitle"] as String)))
                    {
                        //Meldung
                        MessageBoxResult result = MessageBoxResult.OK;
                        string meldung = AppResources.MeldungUeberschreibungNoteInTypeObject1.Replace("µ1", "\""+holdTypeobject.name+"\"")
                            + System.Environment.NewLine + AppResources.MeldungUeberschreibungNoteInTypeObject2;

                        meldung = meldung.Replace("µ2", "\"" + (PhoneApplicationService.Current.State["memoryNoteTitle"] as String) + "\"");

                        result = MessageBox.Show(meldung,
                        AppResources.AppBarOverwriting, MessageBoxButton.OKCancel);

                        if (result == MessageBoxResult.OK)
                        {
                            types_VM.removeNote(holdTypeobject.typeObjectID, (PhoneApplicationService.Current.State["memoryNoteTitle"] as String));
                            PhoneApplicationService.Current.State.Remove("memoryNoteTitle");
                        }
                        else
                        {
                            PhoneApplicationService.Current.State.Remove("memoryNoteTitle");
                            return;
                        }

                    }
                    PhoneApplicationService.Current.State.Remove("memoryNoteTitle");
                }
                searchImage.Visibility = Visibility.Visible;
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
            /*String r = selectedColor.R.ToString("X2");
            String g = selectedColor.G.ToString("X2");
            String b = selectedColor.B.ToString("X2");

            String color = "#" + r + g + b;*/
            String title = newTypeTitle.Text;
            newTypeTitle.Text = "";
            
            try 
            { 
                Types.types_VM.createType(title, "", this.selectedImage);
                //zum gerade erzeugten Typ navigieren
                PivotMain.SelectedIndex = PivotMain.Items.Count - 2;
                PivotMain.SelectedIndex--;
                this.iconPicker.SelectedItem = null;
            }
            catch (ArgumentException ae) 
            {
                MessageBox.Show(ae.Message, "Fehler", MessageBoxButton.OK);
                //NavigationService.GoBack();
            }
        }

        /// <summary>
        /// Nach dem die Seite geladen ist, werden alle angegebenen Farben in die Listbox geladen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconPickerPage_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox l = sender as ListBox;
            iconPicker = l;
            List<IconItem> item = new List<IconItem>();
            for (int i = 0; i < icons.Count(); i++)
            {
                item.Add(new IconItem() { imagePath = icons[i] });
            };

            l.ItemsSource = item; //Fill ItemSource with all colors
            if (restoreType != null)
            {
                iconPicker.SelectedIndex = icons.IndexOf(restoreType.color);
                restoreType = null;
            }
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
        public class IconItem
        {
            public String imagePath { get; set; }
        }


        /// <summary>
        /// Wird ein icon in der Listbox ausgewählt, wird diese unter 
        /// selectedImage gespeichert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox l = sender as ListBox;
            if (l.SelectedIndex == l.Items.Count - 1) {
                PhoneApplicationService.Current.State["chooseIcon"] = true;
                l.SelectedItem = null;

                photoChooserTask.Show();
                return;
            }
            if (l.SelectedItem != null)
            {
                IconItem c = l.SelectedItem as IconItem;
                this.selectedImage = c.imagePath;
            }
            
        }

        /// <summary>
        /// Cancelt die Erstellungen eines Typs und geht eine Seite zurück.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelType(object sender, EventArgs e)
        {
            searchImage.Visibility = Visibility.Visible;
            this.PivotMain.SelectedIndex = 0;
            this.newTypeTitle.Text = "";
            this.iconPicker.SelectedItem = null;
        }

        private void CancelAssignment(object sender, EventArgs e) {
            searchImage.Visibility = Visibility.Visible;
            PhoneApplicationService.Current.State["cancelAssignment"] = true;
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
            TypeDeleteQuestion.Text = AppResources.TypeDeleteQuestion1 + t.title.ToString() +  AppResources.TypeDeleteQuestion2;
            //deleteTypePopup.IsOpen = true;
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
            if (t.typeObjects.Count > 1) {
                MessageBox.Show("Du kannst nur Typen löschen, die keine Objekte enthalten.", "Fehler beim Löschen", MessageBoxButton.OK);
                return;
            }
            Types.Types_VM.deleteType(t.typeID);
            this.PivotMain.SelectedIndex = (this.PivotMain.SelectedIndex) % this.PivotMain.Items.Count;
            this.PivotMain.SelectedIndex = (this.PivotMain.SelectedIndex == 0) ? 
                this.PivotMain.Items.Count -1 : 
                this.PivotMain.SelectedIndex--;

            //deleteTypePopup.IsOpen = false;
        }


        /// <summary>
        /// Der Löschvorgang für einen Typ wird abgebrochen und die Sicherheitsfrage geschlossen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoNotDeleteType(object sender, EventArgs e)
        {
            //deleteTypePopup.IsOpen = false;
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
            if (!(PhoneApplicationService.Current.State.ContainsKey("assignNote") ||
                PhoneApplicationService.Current.State.ContainsKey("attachEvent")))
            {
                Pivot p = sender as Pivot;
                if (p == null)
                    return;
                datawrapper.Type t = p.SelectedItem as datawrapper.Type;
                if (t == null)
                    return;
                this.currentType = t;

                if (t.typeID == -1)
                {
                    this.setAddTypeApplicationbar();
                }
                else
                {
                    this.setNormalApplicationBar();
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
        }

        private void cancelSelection(object sender, EventArgs e) {
            if (currentSelectList != null) { 
                currentSelectList.SelectedItems.Clear();
                types_VM.addAddTypeObject(this.currentType);
                this.setNormalApplicationBar();
            } 
        }

        private void deleteSelection(object sender, EventArgs e) {
            if (currentSelectList != null) {
                this.deleteTypeObjectPopup.IsOpen = true;
                
            }
        }

        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.PivotMain.IsLocked = true;
            types_VM.removeAddTypeObject(this.currentType);
            this.setSelectionApplicationBar();
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
        }

        /// <summary>
        /// Alle Selektionen werden beim Klick auf die Checkbox aufgehoben.
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
            this.setNormalApplicationBar();
            types_VM.addAddTypeObject(this.currentType);
        }

        
        private void TitleLoaded(object sender, RoutedEventArgs e)
        {
            newTypeTitle = sender as TextBox;
            if (restoreType != null)
            {
                newTypeTitle.Text = restoreType.title;
                PivotMain.SelectedIndex = types_VM.getTypeCount() - 1;
            }
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Search.xaml", UriKind.RelativeOrAbsolute));
        }

        private void LayoutRoot_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/TypesOverview.xaml", UriKind.Relative));
            e.Handled = true;
            e.Complete();
        }

        /// <summary>
        /// Erstelle die Applicationbar für das Selektionsmenü (abbrechen und löschen)
        /// </summary>
        private void setSelectionApplicationBar() {
            if (currentApplicationbarId != 2) {
                currentApplicationbarId = 2;
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
        }

        /// <summary>
        /// Erstelle die normale Applicationbar (Bearbeiten und löschen)
        /// </summary>
        private void setNormalApplicationBar() {
            if (currentApplicationbarId != 1) {
                currentApplicationbarId = 1;
                ApplicationBarIconButton btn1 = new ApplicationBarIconButton();
                ApplicationBarIconButton btn2 = new ApplicationBarIconButton();

                btn1.IconUri = new Uri("/icons/edit.png", UriKind.Relative);
                btn1.Text = AppResources.AppBarEdit;
                btn1.Click -= SaveType;
                btn1.Click += ChangeType;
                btn2.IconUri = new Uri("/icons/delete.png", UriKind.Relative);
                btn2.Text = AppResources.AppBarDelete;
                btn2.Click -= CancelType;
                btn2.Click += DeleteType;

                ApplicationBar.Buttons.Clear();
                
                ApplicationBar.Buttons.Add(btn1);
                ApplicationBar.Buttons.Add(btn2);
            }
        }

        private void setAddTypeApplicationbar() {
            if (currentApplicationbarId != 3) {
                currentApplicationbarId = 3;
                ApplicationBar.Buttons.Clear();
                ApplicationBarIconButton btn1 = new ApplicationBarIconButton();
                ApplicationBarIconButton btn2 = new ApplicationBarIconButton();
                btn1.IconUri = new Uri("/icons/save.png", UriKind.Relative);
                btn1.Text = AppResources.AppBarSave;
                btn1.Click += SaveType;
                btn2.IconUri = new Uri("/icons/cancel.png", UriKind.Relative);
                btn2.Text = AppResources.AppBarCancel;
                btn2.Click += CancelType;
                this.currentSelectList = null;
                ApplicationBar.Buttons.Add(btn1);
                ApplicationBar.Buttons.Add(btn2);
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (currentSelectList != null && currentSelectList.SelectedItems.Count > 0 && currentSelectList.IsSelectionEnabled)
            {
                e.Cancel = true;
                currentSelectList.IsSelectionEnabled = false;
            }     
        }

        private void cancelDeleteSelection(object sender, RoutedEventArgs e)
        {
            this.deleteTypeObjectPopup.IsOpen = false;
        }

        private void confirmDeleteSelection(object sender, RoutedEventArgs e)
        {
            this.PivotMain.IsLocked = false;
            types_VM.addAddTypeObject(this.currentType);
            IEnumerator enumerator = currentSelectList.SelectedItems.GetEnumerator();
            if ((bool)checkKeepNotes.IsChecked)
            {
                while (enumerator.MoveNext())
                {
                    types_VM.deleteTypeObjectSoft(((datawrapper.TypeObject)enumerator.Current).typeObjectID);
                }
            }
            else {
                while (enumerator.MoveNext())
                {
                    types_VM.deleteTypeObject(((datawrapper.TypeObject)enumerator.Current).typeObjectID);
                }
            }
            this.deleteTypeObjectPopup.IsOpen = false;
            this.currentSelectList = null;
        }

        private void selectAllCheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("assignNote") ||
                PhoneApplicationService.Current.State.ContainsKey("attachEvent")) {
                    ((CheckBox)sender).Visibility = Visibility.Collapsed;
            }           
        }

        public List<String> loadIconSettings() {
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (appSettings.Contains("icons"))
            {
                string val = (string)appSettings["icons"];
                return val.Split('|').ToList();
            }
            else {
                return (new String[] { 
	                "icons/pflanzen.png", "icons/muffin.png", "icons/tiere.png",
                    "icons/sport.png", "icons/record.png", "icons/planeten.png",
                    "icons/katze.png", "icons/game.png", "icons/kreatur.png",
                    "icons/schauplatz.png",
                    "icons/add.png"
                        }).ToList();
            }

        }

        public void saveIconSettings() {
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            appSettings.Remove("icons");
            String toSave = "";

            for(int i = 0; i < icons.Count(); i++) {
                toSave += icons[i] + "|";
            }

            toSave = toSave.Substring(0, toSave.Count() - 1);
            appSettings.Add("icons", toSave);
        }

        private void tTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (restoreType == null) restoreType = new datawrapper.Type();
            this.restoreType.title = this.newTypeTitle.Text;
        }
    }
}