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

namespace WritersToolbox.views
{
    public partial class AddNote : PhoneApplicationPage
    {
        TextBlock textBlock;
        PhotoChooserTask photoChooserTask;
        Image img;
        public AddNote()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            //Text(210.0, 150.0, "It works", Colors.Red);

        }

        //private void detailsNote_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        //{
        //    if (sender.Equals(detailsNote))
        //    {
        //        UIElement element = detailsNote;
        //        //Canvas c = this.canvasNote;
        //        var x = Canvas.GetLeft(element);
        //        var y = Canvas.GetTop(element);
        //        Canvas.SetLeft(element, x + e.DeltaManipulation.Translation.X);
        //        Canvas.SetTop(element, y + e.DeltaManipulation.Translation.Y);
        //    }
        //}

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
                img.Height = 150;
                img.Width = 200;
                Canvas.SetLeft(img, 10);
                Canvas.SetTop(img, 10);
                imageContainer.Children.Add(img); 
             
            }
        }

        private void test_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            UIElement element = img;
            //Canvas c = this.canvasNote;
            var x = Canvas.GetLeft(element);
            var y = Canvas.GetTop(element);
            Canvas.SetLeft(element, x + e.DeltaManipulation.Translation.X);
            Canvas.SetTop(element, y + e.DeltaManipulation.Translation.Y);
        }

        //private void Text(double x, double y, string text, Color color)
        //{

        //    textBlock = new TextBlock();

        //    //textBlock.Text = text;

        //    textBlock.Foreground = new SolidColorBrush(color);


        //    Canvas.SetLeft(textBlock, x);

        //    Canvas.SetTop(textBlock, y);

        //    canvasNote.Children.Add(textBlock);

        //}

        //private void canvasNote_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        //{

        //}

        private void TextBlock_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            textBlock.Text = String.Format(
        "The key {0} was pressed while focus was on {1}",
        e.Key.ToString(), (e.OriginalSource as FrameworkElement).Name);
        }


    }
}