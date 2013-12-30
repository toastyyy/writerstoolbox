using System;
using System.Resources;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.IO;
using Windows.Phone.Speech.Recognition;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WritersToolbox.viewmodels;
using WritersToolbox.models;
using Windows.Storage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
namespace WritersToolbox.views
{
    public partial class PivotPage1 : PhoneApplicationPage
    {
        //private PhotoChooserTask photoChooserTask; //
        private AddNoteViewModel anvm; //
        private int NoteID; //
        bool isPhotochooser; // 
        private ObservableCollection<MyImage> items; //

        public PivotPage1()
        {
            InitializeComponent();
            isPhotochooser = false;
            NoteID = 1;

            anvm = new AddNoteViewModel();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            MessageBox.Show("You can not use Hardware back button");
            e.Cancel = true;
        }

        //Fertig
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (PhoneApplicationService.Current.State.ContainsKey("OppendImageView"))
            {
                if (PhoneApplicationService.Current.State.ContainsKey("deletedImages")
                    && PhoneApplicationService.Current.State.ContainsKey("addedImages"))
                {
                    items = anvm.getImages(NoteID, (PhoneApplicationService.Current.State["deletedImages"] as string)
                        , (PhoneApplicationService.Current.State["addedImages"] as string));
                }
                else if (PhoneApplicationService.Current.State.ContainsKey("deletedImages"))
                {
                    items = anvm.getImages(NoteID, (PhoneApplicationService.Current.State["deletedImages"] as string)
                        , "");
                }
                else if (PhoneApplicationService.Current.State.ContainsKey("addedImages"))
                {
                    items = anvm.getImages(NoteID, ""
                        , (PhoneApplicationService.Current.State["addedImages"] as string));
                }
                else
                {
                    items = anvm.getImages(NoteID, "", "");
                }

            }
            else if (!isPhotochooser)
            {
                string strCodeTiers = string.Empty;
                if (PhoneApplicationService.Current.State.ContainsKey("memoryNoteID"))
                {
                    try
                    {
                        NoteID = int.Parse(NavigationContext.QueryString["memoryNoteID"]);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Fehler beim Parsen von String to Int aufgetretten");
                    }
                }
                if (NoteID == 0)
                {
                    items = new ObservableCollection<MyImage>();

                }
                else
                {
                    items = anvm.getImages(NoteID);
                }

            }

            this.llms_images.ItemsSource = items;
            if (items.Count > 0)
                selectAllCheckBox.Visibility = Visibility.Visible;
            else
                selectAllCheckBox.Visibility = Visibility.Collapsed;

            deleteButton.Visibility = Visibility.Collapsed;
            zurueckButton.Visibility = Visibility.Collapsed;
        }

        //Fertig
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double height = heightCalculator(detailsTextBox.Text, detailsTextBox.FontFamily, detailsTextBox.FontSize,
                detailsTextBox.FontStyle, detailsTextBox.FontWeight);
            if (height > 410)
            {
                detailsTextBox.Height = height;
            }

        }

        //Fertig
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

        //Fertig
        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {

            if (e.TaskResult == TaskResult.OK)
            {
                isPhotochooser = true;

                MediaLibrary medianbibliothek = new MediaLibrary();

                foreach (Picture picture in medianbibliothek.Pictures)
                {
                    if (picture.Name.Equals(Path.GetFileName(e.OriginalFileName)))
                    {
                        MyImage foto = new MyImage(picture);
                        items.Insert(0, foto);

                        if (PhoneApplicationService.Current.State.ContainsKey("addedImages"))
                        {
                            string cachImages = (PhoneApplicationService.Current.State["addedImages"] as string);
                            cachImages += foto.Name + "|";
                            PhoneApplicationService.Current.State["addedImages"] = cachImages;
                        }
                        else
                        {
                            string cachImages = foto.Name + "|";
                            PhoneApplicationService.Current.State["addedImages"] = cachImages;
                        }

                    }

                }
             }
         }

        //Fertig
        public void img_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            llms_images.EnforceIsSelectionEnabled = true;
            deleteButton.Visibility = Visibility.Visible;
            zurueckButton.Visibility = Visibility.Visible;
        }

        //Fertig
        private void titelTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (titelTextBox.Text.ToString().Equals("Titel"))
            {
                titelTextBox.Text = "";
            }
        }

        //Fertig
        private void titelTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (titelTextBox.Text.Trim().ToString().Equals(""))
            {
                titelTextBox.Text = "Titel";
            }
        }

        //Fertig
        private void detailsTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (detailsTextBox.Text.ToString().Equals("Details"))
            {
                detailsTextBox.Text = "";
            }
        }

        //Fertig
        private void detailsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (detailsTextBox.Text.Trim().ToString().Equals(""))
            {
                detailsTextBox.Text = "Details";
            }
        }

        //Fertig
        private async void micro1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var sr = new SpeechRecognizerUI();
            sr.Settings.ReadoutEnabled = true;
            sr.Settings.ShowConfirmation = false;
            var result = await sr.RecognizeWithUIAsync();
            sr.Recognizer.Grammars.AddGrammarFromPredefinedType("webSearch", SpeechPredefinedGrammar.WebSearch);

            if (result.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
            {
                if ((int)result.RecognitionResult.TextConfidence < (int)SpeechRecognitionConfidence.Medium)
                {

                    MessageBox.Show("Wir haben nicht verstanden, versuchen Sie nochmal !!");
                    await sr.RecognizeWithUIAsync();
                }
                else
                {
                    if (titelTextBox.Text.ToString().Equals("Titel"))
                    {
                        titelTextBox.Text = "";
                    }
                    titelTextBox.Text += result.RecognitionResult.Text + " ";
                }
            }
        }

        //Fertig
        private async void micro2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var sr = new SpeechRecognizerUI();
            sr.Settings.ReadoutEnabled = true;
            sr.Settings.ShowConfirmation = false;
            var result = await sr.RecognizeWithUIAsync();
            sr.Recognizer.Grammars.AddGrammarFromPredefinedType("webSearch", SpeechPredefinedGrammar.WebSearch);

            if (result.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
            {
                if ((int)result.RecognitionResult.TextConfidence < (int)SpeechRecognitionConfidence.Medium)
                {

                    MessageBox.Show("Wir haben nicht verstanden, versuchen Sie nochmal !!");
                    await sr.RecognizeWithUIAsync();
                }
                else
                {
                    if (detailsTextBox.Text.ToString().Equals("Details"))
                    {
                        detailsTextBox.Text = "";
                    }
                    detailsTextBox.Text += result.RecognitionResult.Text + " ";
                }
            }
        }

        //Fertig
        private void imageView_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image i = sender as Image;
            PhoneApplicationService.Current.State["imageView"] = i;
            NavigationService.Navigate(new Uri("/views/ImageView.xaml", UriKind.Relative));
            
        }

        //TODO
        private void image_selectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //TODO
        private void saveButton_Click(object sender, EventArgs e)
        {
            
        }

        //Fertig
        private void imageView_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image image = (Image)sender;
            BitmapImage i = (BitmapImage) image.Source;

            llms_images.EnforceIsSelectionEnabled = true;

            llms_images.SelectedItems.Add(((MyImage)image.DataContext));

            deleteButton.Visibility = Visibility.Visible;
            zurueckButton.Visibility = Visibility.Visible;
        }

        //Fertig
        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {

                llms_images.EnforceIsSelectionEnabled = true;
                deleteButton.Visibility = Visibility.Visible;
                zurueckButton.Visibility = Visibility.Visible;
                llms_images.SelectedItems.Clear();
                ObservableCollection<MyImage> items = (ObservableCollection<MyImage>)llms_images.ItemsSource;

                foreach (MyImage item in items)
                {
                    llms_images.SelectedItems.Add(item);
                }
                
        }

        //Fertig
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            photoChooserTask.Show();
        }

        //Fertig
        private void selectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            llms_images.SelectedItems.Clear();
            llms_images.EnforceIsSelectionEnabled = false;
            deleteButton.Visibility = Visibility.Collapsed;
            zurueckButton.Visibility = Visibility.Collapsed;
        }

        //Fertig
        private void zurueckButton_Click(object sender, RoutedEventArgs e)
        {
            llms_images.SelectedItems.Clear();
            llms_images.EnforceIsSelectionEnabled = false;
            deleteButton.Visibility = Visibility.Collapsed;
            zurueckButton.Visibility = Visibility.Collapsed;
        }

        //Fertig <- noch nicht.
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<MyImage> tempitems = new ObservableCollection<MyImage>(
                (ObservableCollection<MyImage>)llms_images.ItemsSource);

            foreach (MyImage item in tempitems)
            {
                if (llms_images.SelectedItems.Contains(item))
                {
                    llms_images.ItemsSource.Remove(item);
                    items.Remove(item);
                }
            }
        }


    }
}