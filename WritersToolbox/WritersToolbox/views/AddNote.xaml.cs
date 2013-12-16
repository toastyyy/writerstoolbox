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

        PhotoChooserTask photoChooserTask;
        Image img;
        public AddNote()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
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
                img.Height = 150;
                img.Width = 200;
                Canvas.SetLeft(img, 10);
                Canvas.SetTop(img, 10);
                canvasNote.Children.Add(img); 
             
            }
        }

        private void canvasNote_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {

        }

        private void detailsNote_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            var x = Canvas.GetLeft(img);
            var y = Canvas.GetTop(img);
            if (x + e.DeltaManipulation.Translation.X > Canvas.GetLeft(detailsNote)
                && y + e.DeltaManipulation.Translation.Y > Canvas.GetTop(detailsNote)
                && x + e.DeltaManipulation.Translation.X < Canvas.GetLeft(detailsNote) + img.Width
                && y + e.DeltaManipulation.Translation.Y < Canvas.GetTop(detailsNote) + img.Height + 100)
            {
                Canvas.SetLeft(img, x + e.DeltaManipulation.Translation.X);
                Canvas.SetTop(img, y + e.DeltaManipulation.Translation.Y);
            }
        }

    }
}