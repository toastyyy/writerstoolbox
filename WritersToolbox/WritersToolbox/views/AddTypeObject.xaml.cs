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

namespace WritersToolbox.views
{
    public partial class AddTypeObject : PhoneApplicationPage
    {
        //TypID zudem das Objekt gehört
        private int typeID;

        //TypeObjectID des Objekts
        private int typeObjectID;

        //gibt an, ob neues Objekt oder ein Update
        private bool update = false;

        //bei selectionChanged wird Farbe hier zwischengespeichert
        private Color selectedColor;

        //Index einer bereits benutzten Farbe, wird zum Ändern gebraucht
        private int selectedColorIndex;

        //Farben für Colorpicker
        static string[] colors =
        { 
	        "#FFFFE135","#FFFFFF66","#FF008A00","#FF32CD32","#FF00FF7F","#FF808000",
            "#FFFF0000","#FFFF4500","#FFFF8C00", "#FFFF7F50","#FFDC143C","#FFFF1493",
            "#FFB22222","#FFC71585","#FFDA70D6","#FF000080","#FF4B0082","#FF800080",
            "#FFADD8E6","#FF20B2AA","#FF008080"
        };

        //Imagepaths für Imagepicker
        static string[] images =
        { 
	        "../icons/TypeObjects/character.png","../icons/TypeObjects/character.png"
        };

        private string selectedImagePath;

        //Index eines bereits benutzten Bildes, wird zum Ändern gebraucht
        private int selectedImageIndex;

        

        public AddTypeObject()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ein neues Typobjekt wird erzeugt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveTypeObject(object sender, EventArgs e)
        {
            String r = selectedColor.R.ToString("X2");
            String g = selectedColor.G.ToString("X2");
            String b = selectedColor.B.ToString("X2");

            String color = "#" + r + g + b;
            String name = toName.Text;

            try
            {
                if (update)
                {
                    Types.Types_VM.updateTypeObject(typeObjectID, name, color, selectedImagePath);
                }
                else
                {
                    Types.types_VM.createTypeObject(name, color, selectedImagePath, typeID);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Objekt konnte nicht erstellt werden");
            }

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
            if (update)
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
            if (NavigationContext.QueryString.ContainsKey("typeID"))
            {
                var tID = NavigationContext.QueryString["typeID"];
                typeID = int.Parse(tID);
                Title.Text = "Neues Objekt";
            }
            else if (NavigationContext.QueryString.ContainsKey("item"))
            {
                update = true;
                var item = NavigationContext.QueryString["item"];
                typeObjectID = int.Parse(item);
                toName.Text = Types.Types_VM.getNameForTypeObject(typeObjectID);
                selectedColorIndex = Array.IndexOf(colors, Types.Types_VM.getColorForTypeObject(typeObjectID).ToString());
                selectedImageIndex = Array.IndexOf(images, Types.Types_VM.getImagePathForTypeObject(typeObjectID));
                Title.Text = "Objekt ändern";
            }
        }

        /// <summary>
        /// Nach dem Laden werden alle vorhandenen Bilder als itemSource 
        /// dem Picker zur Verfügung gestellt. Bei einem Update wird das
        /// bereits benutzte Bild verwendet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImagePickerPageLoaded(object sender, RoutedEventArgs e)
        {
            List<TypobjectImage> item = new List<TypobjectImage>();
            for (int i = 0; i < images.Length; i++)
            {
                item.Add(new TypobjectImage() { ImagePath = images[i] });
            };

            ImagePicker.ItemsSource = item; //Fill ItemSource with all colors
            if (update)
                ImagePicker.SelectedIndex = selectedImageIndex;
        }


        /// <summary>
        /// Klasse zum Zwischenspeichern und Binding des Imagepaths
        /// </summary>
        public class TypobjectImage
        {
            public string ImagePath { get; set; }
        }

        /// <summary>
        /// Bildauswähl Button wurde gedrückt und Popup erscheint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pickImage(object sender, RoutedEventArgs e)
        {
            Imagepicker_popup.IsOpen = true;
        }


        /// <summary>
        /// Bildauswählen wird abgebrochen und Popup geschlossen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelImagePicker(object sender, RoutedEventArgs e)
        {
            Imagepicker_popup.IsOpen = false;
        }

        /// <summary>
        /// Beim endgültigen Auswählen im Popup wird der Imagepath zwischen gespeichert
        /// und das Popup geschlossen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectImage(object sender, RoutedEventArgs e)
        {
            selectedImagePath = (ImagePicker.SelectedItem as TypobjectImage).ImagePath;
            Imagepicker_popup.IsOpen = false;
        }

    }
}