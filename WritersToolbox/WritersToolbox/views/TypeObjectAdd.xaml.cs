using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Diagnostics;
using Microsoft.Phone.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Media.Imaging;
using System.Security.Cryptography;
using System.IO.IsolatedStorage;
using WritersToolbox.viewmodels;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using System.IO;
using Windows.Storage;
using Microsoft.Xna.Framework.Media;

namespace WritersToolbox.views
{
    public partial class TypeObjectAdd : PhoneApplicationPage
    {
        //bei selectionChanged wird Farbe hier zwischengespeichert
        private Color selectedColor;

        //Index einer bereits benutzten Farbe, wird zum Ändern gebraucht
        private int selectedColorIndex;

        private int typeID;
        //Farben für Colorpicker

        // PhotoChooser
        private PhotoChooserTask photoChooserTask;

        private String changedImage = "";

        private Boolean changed;
        static string[] colors =
        { 
	        "#FFFFE135","#FFFFFF66","#FF008A00","#FF32CD32","#FF00FF7F","#FF808000",
            "#FFFF0000","#FFFF4500","#FFFF8C00", "#FFFF7F50","#FFDC143C","#FFFF1493",
            "#FFB22222","#FFC71585","#FFDA70D6","#FF000080","#FF4B0082","#FF800080",
            "#FFADD8E6","#FF20B2AA","#FF008080"
        };

        private string selectedImagePath;

        public static string GetSHA256Hash(string str)
        {
            SHA256 mySHA256 = new SHA256Managed();
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            mySHA256.ComputeHash(bytes);
            return BitConverter.ToString(mySHA256.Hash).Replace("-", "").ToLower();
        }

        public TypeObjectAdd()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
        }

        /// <summary>
        /// Ein neues Typobjekt wird erzeugt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveTypeObject(object sender, EventArgs e)
        {

            NavigationService.GoBack();
        }


        /// <summary>
        /// Nach dem die Seite geladen ist, werden alle angegebenen Farben in die Listbox geladen.
        /// Wird die View zum Ändern benutzt, wird die benutzte Farbe selektiert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPickerPage_Loaded(object sender, RoutedEventArgs e)
        {
            List<ColorItem> item = new List<ColorItem>();
            for (int i = 0; i < colors.Length; i++)
            {
                item.Add(new ColorItem() { Color = fromHexToColor(colors[i])});
            };

            colorPicker.ItemsSource = item; //Fill ItemSource with all colors
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
        /// Cancelt die Erstellungen eines TypObjekts und geht eine Seite zurück.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelTypeObject(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// Beim Navigieren zu dieser Seite wird der ausgewählte Typ aus
        /// dem Navigationskontext herausgefiltert um das zu erstellende neue
        /// Typobjekt dem Typ zuordnen zu können.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (PhoneApplicationService.Current.State.ContainsKey("preventUpdate") &&
            (Boolean)PhoneApplicationService.Current.State["preventUpdate"] == true)
            {
                PhoneApplicationService.Current.State["preventUpdate"] = false;
                return;
            }
            if (NavigationContext.QueryString.ContainsKey("typeID"))
            {
                var tID = NavigationContext.QueryString["typeID"];
                typeID = int.Parse(tID);
            }
        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //Code to display the photo on the page in an image control named myImage.
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                this.imageButton.Source = bmp;
                this.changedImage = e.OriginalFileName;
                this.changed = true;
            }
        }

        private void imageButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneApplicationService.Current.State["preventUpdate"] = true;
            photoChooserTask.Show();
        }

        private void tTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!tTitle.Text.Equals("")) {
                changed = true;
            }
        }

        /// <summary>
        /// Workaround: Setzen der Hintergrundfarbe der Textbox beim focus auf transparent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush _s = new SolidColorBrush(Colors.Transparent);
            this.tTitle.Background = _s;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            String name = tTitle.Text;
            String color = this.selectedColor.ToString();
            if (name.Equals(""))
            {
                MessageBox.Show("Du musst einen Namen angeben", "Speichern fehlgeschlagen", MessageBoxButton.OK);
                return;
            }
            String fileName = "";
            // Bild speichern
            if (this.changedImage != null && !this.changedImage.Equals(""))
            {
                if (!this.changedImage.Equals(""))
                {
                    fileName = this.changedImage;
                }
            }
                // viewmodel erst hier erzeugen, weil es nur beim erstellen benötigt wird!
                TypesViewModel tvm = new TypesViewModel();
                tvm.createTypeObject(name, color, fileName, this.typeID);
                NavigationService.Navigate(new Uri("/views/Types.xaml?item=" + this.typeID, UriKind.Relative));

            
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (changed)
            {
                string message = "Willst du deine Änderungen wirklich verwerfen?";
                string caption = "Abbrechen";
                MessageBoxButton buttons = MessageBoxButton.OKCancel;
                // Show message box
                MessageBoxResult result = MessageBox.Show(message, caption, buttons);
                if (result == MessageBoxResult.OK)
                {
                    NavigationService.Navigate(new Uri("/views/Types.xaml?item=" + this.typeID, UriKind.Relative));
                }
            }
            else {
                NavigationService.Navigate(new Uri("/views/Types.xaml?item=" + this.typeID, UriKind.Relative));
            }
        }

        private void colorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            // ermittle die aktuell verwendete farbe
            List<ColorItem> item = new List<ColorItem>();
            for (int i = 0; i < colors.Length; i++)
            {
                item.Add(new ColorItem() { Color = fromHexToColor(colors[i]) });
            };
            colorPicker.ItemsSource = item; //Fill ItemSource with all colors
        }

        private void colorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox l = sender as ListBox;
            ColorItem c = l.SelectedItem as ColorItem;
            if (c != null)
            {
                selectedColor = c.Color;
                WriteableBitmap bmp = multiplicateImageWithColor("images/headImage_grayscale_top.jpg", c.Color);

                this.headerBackground.Source = bmp;
                changed = true;
            }
        }
        
        /// <summary>
        /// Wendet einen Multiplikationsfilter auf das angewendete Bild an. 
        /// ACHTUNG: Funktioniert nur hinreichend bei Bildern in Graustufen.
        /// </summary>
        /// <param name="fileName">Dateiname. Objekt muss als Datei im Projekt mit der Build Action 'Content' vorhanden sein.</param>
        /// <param name="c">Anzuwendende Farbe für die Überlagerung</param>
        /// <returns>Neues Bild mit angewendetem Filter</returns>
        private WriteableBitmap multiplicateImageWithColor(String fileName, Color c) {
            var file = System.Windows.Application.GetResourceStream(new Uri(fileName, UriKind.Relative));
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(file.Stream);
            WriteableBitmap wb = new WriteableBitmap(bmp);


            for (int x = 0; x < wb.PixelWidth; x++) {
                for (int y = 0; y < wb.PixelHeight; y++) {
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
    }
}