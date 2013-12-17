using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Shapes;

namespace WritersToolbox.views
{
    public partial class AddNote : PhoneApplicationPage
    {

        PhotoChooserTask photoChooserTask;
        Image img;
        HashSet<Rectangle> rectangles;
        List<Image> imageList = new List<Image>();
        int index = -1;
        public AddNote()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            rectangles = new HashSet<Rectangle>();
            detailsNote.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            photoChooserTask.Show();
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bi = new BitmapImage();
                bi.SetSource(e.ChosenPhoto);
                WriteableBitmap b = new WriteableBitmap(bi);
                img =  new Image();
                img.Source = b;
                img.Hold += new EventHandler<GestureEventArgs>(img_hold);
                img.Height = 150;
                img.Width = 200;
                Canvas.SetLeft(img, Canvas.GetLeft(detailsNote) + 40);
                Canvas.SetTop(img, Canvas.GetTop(detailsNote) + 50);
                canvasNote.Children.Add(img);
                imageList.Add(img);
                index++;
            }
        }

        private void img_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {

        }

        private void img_hold(object sender, GestureEventArgs e)
        {
            MessageBoxResult m = MessageBox.Show("möchten sie es löschen!?", "löschen", MessageBoxButton.OKCancel);
            if (m == MessageBoxResult.OK)
            {
                canvasNote.Children.Remove(imageList[index]);
            }
        }
        private void detailsNote_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (index >= 0)
            {
                Image temp_img = imageList[index];
                if (e.PinchManipulation != null)
                {
                    CompositeTransform ctf = new CompositeTransform();
                    temp_img.RenderTransform = ctf;
                    var transform = (CompositeTransform)temp_img.RenderTransform;
                    var x = Canvas.GetLeft(temp_img);
                    var y = Canvas.GetTop(temp_img);
                    // Scale Manipulation
                    if (x + e.DeltaManipulation.Translation.X > Canvas.GetLeft(detailsNote)
                            && y + e.DeltaManipulation.Translation.Y > Canvas.GetTop(detailsNote)
                            && x + e.DeltaManipulation.Translation.X < Canvas.GetLeft(detailsNote) + temp_img.Width
                            && y + e.DeltaManipulation.Translation.Y < Canvas.GetTop(detailsNote) + temp_img.Height + 100)
                    {
                        transform.ScaleX = e.PinchManipulation.CumulativeScale;
                        transform.ScaleY = e.PinchManipulation.CumulativeScale;

                        // Translate manipulation
                        var originalCenter = e.PinchManipulation.Original.Center;
                        var newCenter = e.PinchManipulation.Current.Center;
                        transform.TranslateX = newCenter.X - originalCenter.X;
                        transform.TranslateY = newCenter.Y - originalCenter.Y;
                    }

                    // end 
                    e.Handled = true;
                }
                if (e.DeltaManipulation != null)
                {
                    var x = Canvas.GetLeft(temp_img);
                    var y = Canvas.GetTop(temp_img);
                    if (x + e.DeltaManipulation.Translation.X > Canvas.GetLeft(canvasNote)
                        && y + e.DeltaManipulation.Translation.Y > Canvas.GetTop(canvasNote)
                        && x + e.DeltaManipulation.Translation.X < Canvas.GetLeft(canvasNote) + temp_img.Width
                        && y + e.DeltaManipulation.Translation.Y < Canvas.GetTop(canvasNote) + temp_img.Height + 100)
                    {
                        Canvas.SetLeft(temp_img, x + e.DeltaManipulation.Translation.X);
                        Canvas.SetTop(temp_img, y + e.DeltaManipulation.Translation.Y);
                    }

                    e.Handled = true;
                }
            }
            
        }

        private void detailsNote_GotFocus(object sender, RoutedEventArgs e)
        {
            detailsNote.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void detailsNote_DoubleTap(object sender, GestureEventArgs e)
        {
            saveButton.Focus();
            Point p = e.GetPosition(detailsNote);
            Point p1;
            for (int i = imageList.Count - 1; i >= 0; i--)
            {
                var x1 = Canvas.GetLeft(imageList[i]);
                var y1 = Canvas.GetTop(imageList[i]);
                p1 = new Point(x1, y1);
                if (p.X > p1.X && p.Y > p1.Y && p.X < p1.X + imageList[i].Width && p.Y < p1.Y + imageList[i].Height)
                {
                    index = i;
                    break;
                }
            }          
        }

        private void detailsNote_Tap(object sender, GestureEventArgs e)
        {
            detailsNote.Focus();
        }

        private void detailsNote_Hold(object sender, GestureEventArgs e)
        {
            if (index >= 0)
            {
                MessageBoxResult m = MessageBox.Show("möchten sie es löschen!?", "löschen", MessageBoxButton.OKCancel);
                if (m == MessageBoxResult.OK)
                {
                    canvasNote.Children.Remove(imageList[index]);
                }
            }          
        }

        private void detailsNote_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            //this.ScrollBar.
            //this.ScrollBar.UpdateLayout();
        }
    }
}