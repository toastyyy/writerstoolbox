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
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.IO;

namespace WritersToolbox.views
{
    public partial class PivotPage1 : PhoneApplicationPage
    {
        PhotoChooserTask photoChooserTask;
        List<Image> listImages;
        Dictionary<Image, string> d;
        Image img;
        int imageIndexe = 1;
        public PivotPage1()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            listImages = new List<Image>();
            d = new Dictionary<Image, string>();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double height = heightCalculator(textBox1.Text, textBox1.FontFamily, textBox1.FontSize,
                textBox1.FontStyle, textBox1.FontWeight);
            if (height > 432)
            {
                textBox1.Height = height;
            }

        }

        private double heightCalculator(String str, FontFamily font, double size,
            FontStyle fontstyle, FontWeight fontweight)
        {
            TextBlock l = new TextBlock();
            l.FontFamily = font;
            l.FontSize = size;
            l.FontStyle = fontstyle;
            l.FontWeight = fontweight;
            l.Text = str;
            return l.ActualHeight + 70;
        }

        //TODO
        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            //micro1.Source = GetImageByName("/Resources/micro.png");
        }

        //TODO
        public WriteableBitmap GetImageByName(string imageName)
        {
            WriteableBitmap b = null;
            try
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                string resourceName = asm.GetName().Name + ".Properties.Resources";
                var rm = new System.Resources.ResourceManager(resourceName, asm);
                b = new WriteableBitmap((BitmapImage)rm.GetObject(imageName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }

            return b;

        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            
            string tempJPEG = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
            if (e.TaskResult == TaskResult.OK)
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (myIsolatedStorage.FileExists(tempJPEG))
                    {
                        myIsolatedStorage.DeleteFile(tempJPEG);
                    }

                    IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(tempJPEG);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(e.ChosenPhoto);
                    WriteableBitmap wb = new WriteableBitmap(bitmap);

                    // Encode WriteableBitmap object to a JPEG stream.
                    Extensions.SaveJpeg(wb, fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);

                    //wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                    fileStream.Close();
        
                    img = new Image();
                    img.Source = wb;
                    img.Width = 198;
                    img.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(img_Tap);
                    GridManager.removeChild(ref ContentPanel_Bilder, addPictur);
                    if (imageIndexe > 1 && imageIndexe % 2 == 0)
                    {
                        GridManager.addRow(ref ContentPanel_Bilder, 200);

                    }
                    GridManager.addCol(ref ContentPanel_Bilder, 225);
                    GridManager.addObjectInGrid(ref ContentPanel_Bilder, ref img, imageIndexe - 1);
                    GridManager.addObjectInGrid(ref ContentPanel_Bilder, ref addPictur, imageIndexe);
                    
                    listImages.Add(img);
                    d.Add(img, tempJPEG);
                    imageIndexe++;
                }

            }

        }

        private void img_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image i = (Image)e.OriginalSource;
            NavigationService.Navigate(new Uri("/views/ImageView.xaml?path=" + d[i], UriKind.Relative));
        }

        private void addPictur_Click(object sender, RoutedEventArgs e)
        {
            photoChooserTask.Show();
        }
    
        
    }
}