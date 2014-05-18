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
using WritersToolbox.viewmodels;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;

namespace WritersToolbox.views
{
    public partial class ChangeType : PhoneApplicationPage
    {

        //bei selectionChanged wird Farbe hier zwischengespeichert
        private Color selectedColor;

        //Index einer bereits benutzten Farbe, wird zum Ändern gebraucht
        private int selectedColorIndex = 0;

        private int typeIndex = -1;

        private TypeViewModel tvm = null;

        private PhotoChooserTask photoChooserTask;

        private String changedImage = "";

        private Boolean changed = false;

        static List<String> icons;

        public ChangeType()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            icons = this.loadIconSettings();
        }
        /// <summary>
        /// Ein neuer Typ wird erzeugt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveType(object sender, EventArgs e)
        {
            String color = this.selectedColor.ToString();
            String title = tTitle.Text;
            String fileName = this.tvm.Type.imageString;
            if (!this.changedImage.Equals(""))
            {
                fileName = this.changedImage;
            }
            try
            {
                this.tvm.updateType(typeIndex, title, color, fileName);
                NavigationService.GoBack();
            }
            catch (ArgumentException ae)
            {
                MessageBox.Show(ae.Message, "Fehler", MessageBoxButton.OK);
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
            iconPicker.SelectedIndex = icons.IndexOf(this.tvm.Type.imageString);
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
        /// Wird eine Farbe in der Listbox ausgewählt, wird diese unter 
        /// selectedColor gespeichert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IconPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox l = sender as ListBox;
            if (l.SelectedIndex == l.Items.Count - 1)
            {
                PhoneApplicationService.Current.State["chooseIcon"] = true;
                l.SelectedItem = null;

                photoChooserTask.Show();
                return;
            }
            if (l.SelectedItem != null)
            {
                IconItem c = l.SelectedItem as IconItem;
                this.changedImage = c.imagePath;
            }
        }

        /// <summary>
        /// Cancelt die Erstellungen eines Typs und geht eine Seite zurück.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelType(object sender, EventArgs e)
        {
            if (changed)
            {
                var result = MessageBox.Show("Möchtest du deine Änderungen verwerfen?", "Abbrechen", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    NavigationService.GoBack();
                }
            }
            else {
                NavigationService.GoBack();
            }
        }


        /// <summary>
        /// Es wird die TypID ausgelesen, Name geladen und die Passende Farbe dem Colorindex zugewiesen
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("item"))
            {
                var item = NavigationContext.QueryString["item"];
                var indexParsed = int.Parse(item);
                this.typeIndex = indexParsed;
                this.tvm = new TypeViewModel(typeIndex);
                this.tvm.loadData();
                this.DataContext = this.tvm;
            }
        }

        /// <summary>
        /// Wendet einen Multiplikationsfilter auf das angewendete Bild an. 
        /// ACHTUNG: Funktioniert nur hinreichend bei Bildern in Graustufen.
        /// </summary>
        /// <param name="fileName">Dateiname. Objekt muss als Datei im Projekt mit der Build Action 'Content' vorhanden sein.</param>
        /// <param name="c">Anzuwendende Farbe für die Überlagerung</param>
        /// <returns>Neues Bild mit angewendetem Filter</returns>
        private WriteableBitmap multiplicateImageWithColor(String fileName, Color c)
        {
            var file = System.Windows.Application.GetResourceStream(new Uri(fileName, UriKind.Relative));
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(file.Stream);
            WriteableBitmap wb = new WriteableBitmap(bmp);


            for (int x = 0; x < wb.PixelWidth; x++)
            {
                for (int y = 0; y < wb.PixelHeight; y++)
                {
                    Byte brightness = wb.GetBrightness(x, y);
                    Color newColor = new Color();
                    newColor.A = 255;
                    newColor.R = (byte)(c.R * (brightness / 255.0));
                    newColor.G = (byte)(c.G * (brightness / 255.0));
                    newColor.B = (byte)(c.B * (brightness / 255.0));
                    wb.SetPixel(x, y, newColor);
                }
            }
            return wb;
        }

        private void imageButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneApplicationService.Current.State["preventUpdate"] = true;
            photoChooserTask.Show();
        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                if (icons.IndexOf(e.OriginalFileName) < 0)
                {
                    this.changedImage = e.OriginalFileName;
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
        /// Workaround: Setzen der Hintergrundfarbe der Textbox beim focus auf transparent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tTitle_GotFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush _s = new SolidColorBrush(Colors.Transparent);
            this.tTitle.Background = _s;
        }

        private void tTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!tTitle.Text.Equals(this.tvm.Type.title))
            {
                changed = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NavigationService.RemoveBackEntry();
        }

        public List<String> loadIconSettings()
        {
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            if (appSettings.Contains("icons"))
            {
                string val = (string)appSettings["icons"];
                return val.Split('|').ToList();
            }
            else
            {
                return (new String[] { 
	                "icons/pflanzen.png", "icons/muffin.png", "icons/tiere.png",
                    "icons/sport.png", "icons/record.png", "icons/planeten.png",
                    "icons/katze.png", "icons/game.png",
                    "icons/add.png"
                        }).ToList();
            }

        }

        public void saveIconSettings()
        {
            IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
            appSettings.Remove("icons");
            String toSave = "";

            for (int i = 0; i < icons.Count(); i++)
            {
                toSave += icons[i] + "|";
            }

            toSave = toSave.Substring(0, toSave.Count() - 1);
            appSettings.Add("icons", toSave);
        }
    }
}