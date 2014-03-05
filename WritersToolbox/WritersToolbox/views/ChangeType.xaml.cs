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
        //Farben für Colorpicker
        static string[] colors =
        { 
	        "#FFFFE135","#FFFFFF66","#FF008A00","#FF32CD32","#FF00FF7F","#FF808000",
            "#FFFF0000","#FFFF4500","#FFFF8C00", "#FFFF7F50","#FFDC143C","#FFFF1493",
            "#FFB22222","#FFC71585","#FFDA70D6","#FF000080","#FF4B0082","#FF800080",
            "#FFADD8E6","#FF20B2AA","#FF008080"
        };


        public ChangeType()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
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
        /// Wird die View zum Ändern benutzt, wird die benutzte Farbe selektiert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorPickerPage_Loaded(object sender, RoutedEventArgs e)
        {
            List<ColorItem> item = new List<ColorItem>();
            for (int i = 0; i < colors.Length; i++)
            {
                item.Add(new ColorItem() { Color = fromHexToColor(colors[i]) });
            };

            selectedColorIndex = Array.IndexOf(colors, this.tvm.Type.color.ToString()); 


            colorPicker.ItemsSource = item; //Fill ItemSource with all colors
            colorPicker.SelectedIndex = selectedColorIndex;
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
            WriteableBitmap wb = this.multiplicateImageWithColor("images/headImage_grayscale_top.jpg", selectedColor);
            this.headerBackground.Source = wb;
            this.changed = true;
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
                //Code to display the photo on the page in an image control named myImage.
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                this.imageButton.Source = bmp;
                this.changedImage = e.OriginalFileName;
                this.changed = true;
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
    }
}