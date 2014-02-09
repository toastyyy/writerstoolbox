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
using System.Windows.Media;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;

namespace WritersToolbox.views
{
    public partial class TypeObjectEdit : PhoneApplicationPage
    {
        TypeDetailViewModel tdvm = null;
        Boolean changed = false;

        //bei selectionChanged wird Farbe hier zwischengespeichert
        private Color selectedColor;

        //Index einer bereits benutzten Farbe, wird zum Ändern gebraucht
        private int selectedColorIndex;

        // PhotoChooser
        private PhotoChooserTask photoChooserTask;

        private BitmapImage changedImage = null;
        
        //Farben für Colorpicker
        static string[] colors =
        { 
	        "#FFFFE135","#FFFFFF66","#FF008A00","#FF32CD32","#FF00FF7F","#FF808000",
            "#FFFF0000","#FFFF4500","#FFFF8C00", "#FFFF7F50","#FFDC143C","#FFFF1493",
            "#FFB22222","#FFC71585","#FFDA70D6","#FF000080","#FF4B0082","#FF800080",
            "#FFADD8E6","#FF20B2AA","#FF008080"
        };

 
        public static string GetSHA256Hash(string str)
        {
            SHA256 mySHA256 = new SHA256Managed();
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            mySHA256.ComputeHash(bytes);
            return BitConverter.ToString(mySHA256.Hash).Replace("-", "").ToLower();
        }

        public TypeObjectEdit()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
        }

        /// <summary>
        /// Beim Navigieren zu dieser Seite wird das ausgewählte Objekt aus
        /// dem Navigationskontext herausgefiltert und die Details dazu mit dem
        /// Viewmodel geladen.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(PhoneApplicationService.Current.State.ContainsKey("preventUpdate") &&
                (Boolean)PhoneApplicationService.Current.State["preventUpdate"] == true) {
                PhoneApplicationService.Current.State["preventUpdate"] = false;
                return;
            }
            if (NavigationContext.QueryString.ContainsKey("typeObjectID"))
            {
                int toID = int.Parse(NavigationContext.QueryString["typeObjectID"]);
                tdvm = new TypeDetailViewModel(toID);
                this.DataContext = tdvm;

                changed = false;
            }
        }

        /// <summary>
        /// Anonyme Klasse in der die Farbe gespeichert wird.
        /// </summary>
        public class ColorItem
        {
            public Color Color { get; set; }
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

            TypesViewModel tvm = new TypesViewModel();
            if (name.Equals("")) {
                MessageBox.Show("Du musst einen Namen angeben", "Speichern fehlgeschlagen", MessageBoxButton.OK);
                return;
            }

            String fileName = this.tdvm.TypeObject.imageString;
            // Bild speichern
            if (this.changedImage != null) {
                fileName = "";
                var bmp = new WriteableBitmap(this.changedImage);
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!storage.DirectoryExists("user_images")) {
                        storage.CreateDirectory("user_images");
                    }

                    fileName = GetSHA256Hash(DateTime.Now.ToString()) + ".png";

                    using (IsolatedStorageFileStream stream = storage.CreateFile(@"user_images\\" + fileName))
                    {
                        bmp.SaveJpeg(stream, 200, 100, 0, 95);
                        stream.Close();
                    }
                }
            }
            tvm.updateTypeObject(tdvm.TypeObject.typeObjectID, name, color, @"user_images\\" + fileName);
            changed = false;
            NavigationService.Navigate(new Uri("/views/TypeObjectDetails2.xaml?typeObjectID=" + this.tdvm.TypeObject.typeObjectID, UriKind.Relative));
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
                    NavigationService.Navigate(new Uri("/views/TypeObjectDetails2.xaml?typeObjectID=" + this.tdvm.TypeObject.typeObjectID, UriKind.Relative));
                }
            }
            else {
                NavigationService.Navigate(new Uri("/views/TypeObjectDetails2.xaml?typeObjectID=" + this.tdvm.TypeObject.typeObjectID, UriKind.Relative));
            }

        }

        private void tTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!tTitle.Text.Equals(this.tdvm.TypeObject.name)) { 
                changed = true;
            }
        }

        private void colorPicker_Loaded(object sender, RoutedEventArgs e)
        {
            // ermittle die aktuell verwendete farbe
            ColorItem ci = new ColorItem() { Color = fromHexToColor(this.tdvm.TypeObject.color) };
            string s = ci.Color.ToString();
            selectedColorIndex = Array.IndexOf(colors, s); 

            List<ColorItem> item = new List<ColorItem>();
            for (int i = 0; i < colors.Length; i++)
            {
                item.Add(new ColorItem() { Color = fromHexToColor(colors[i]) });
            };
            colorPicker.ItemsSource = item; //Fill ItemSource with all colors
            colorPicker.SelectedIndex = selectedColorIndex;
        }

        private void colorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox l = sender as ListBox;
            ColorItem c = l.SelectedItem as ColorItem;
            if (c != null) { 
                selectedColor = c.Color;
                changed = true;
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
                this.changedImage = bmp;
                this.changed = true;
            }
        }

        private void imageButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneApplicationService.Current.State["preventUpdate"] = true;
            photoChooserTask.Show();
        }
    }
}