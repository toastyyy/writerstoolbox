using System;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
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
using Microsoft.Xna.Framework.Audio;
using Coding4Fun.Toolkit.Audio;


using Coding4Fun.Toolkit.Audio.Helpers;
using System.Threading;

namespace WritersToolbox.views
{
    public partial class PivotPage1 : PhoneApplicationPage
    {
        private PhotoChooserTask photoChooserTask; //
        private AddNoteViewModel anvm; //
        private int NoteID; //
        private bool isPhotochooser; // 
        private ObservableCollection<MyImage> Image_Items; //
        private ObservableCollection<SoundData> sound_Items;
        private MicrophoneRecorder recorder;
        private IsolatedStorageFileStream _audioStream;//
        private string tempTitle, tempDetails, tempTags;
        private ObservableCollection<MyImage> tempImage_Items; //
        private ObservableCollection<SoundData> tempSound_Items;
        private bool isselected1, isselected2, isselected3;

        public PivotPage1()
        {
            InitializeComponent();
            isPhotochooser = false;
            isselected1 = false;
            isselected2 = false;
            NoteID = 0;
            photoChooserTask = new PhotoChooserTask();
            recorder = new MicrophoneRecorder(); 
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            anvm = new AddNoteViewModel();

            sound_Items = new ObservableCollection<SoundData>();

            if (!isPhotochooser)
            {
                if (PhoneApplicationService.Current.State.ContainsKey("memoryNoteID"))
                {
                    try
                    {
                        NoteID = int.Parse(PhoneApplicationService.Current.State["memoryNoteID"] as String);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Fehler beim Parsen von String to Int aufgetretten");
                    }
                }
                if (NoteID == 0)
                {
                    Image_Items = new ObservableCollection<MyImage>();

                }
                else
                {
                    Image_Items = anvm.getImages(NoteID);
                }

            }
            else
            {
                Image_Items = anvm.getImages(NoteID);
            }

            sound_Items = anvm.getAudios(NoteID);
            string temp_title = anvm.getTitle(NoteID);
            string temp_details = anvm.getDetails(NoteID);
            titleTextBox.Text = temp_title.Equals("") ? "Title" : temp_title;
            detailsTextBox.Text = temp_details.Equals("") ? "Details" : temp_details;
            schlagwoerterTextBox.Text = anvm.getTags(NoteID);

            this.llms_records.ItemsSource = sound_Items;
            this.llms_images.ItemsSource = Image_Items;

            if (Image_Items.Count > 0)
                selectAllCheckBox.Visibility = Visibility.Visible;
            else
                selectAllCheckBox.Visibility = Visibility.Collapsed;

            if (sound_Items.Count > 0)
                selectAllRecordCheckBox.Visibility = Visibility.Visible;
            else
                selectAllRecordCheckBox.Visibility = Visibility.Collapsed;


            addManagementApplicationBarButton();
            deleteButton.Visibility = Visibility.Collapsed;
            zurueckButton.Visibility = Visibility.Collapsed;
            deleteRecordButton.Visibility = Visibility.Collapsed;
            zurueckRecordButton.Visibility = Visibility.Collapsed;
            EndTimer.Visibility = Visibility.Collapsed;
            progressbar_background.Visibility = Visibility.Collapsed;
            progressbar.Visibility = Visibility.Collapsed;
            CurrentTime.Visibility = Visibility.Collapsed;
            
        }

        //Fertig
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
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
                    Image_Items = anvm.getImages(NoteID, (PhoneApplicationService.Current.State["deletedImages"] as string)
                        , (PhoneApplicationService.Current.State["addedImages"] as string));
                }
                else if (PhoneApplicationService.Current.State.ContainsKey("deletedImages"))
                {
                    Image_Items = anvm.getImages(NoteID, (PhoneApplicationService.Current.State["deletedImages"] as string)
                        , "");
                }
                else if (PhoneApplicationService.Current.State.ContainsKey("addedImages"))
                {
                    Image_Items = anvm.getImages(NoteID, ""
                        , (PhoneApplicationService.Current.State["addedImages"] as string));
                }
                else
                {
                    Image_Items = anvm.getImages(NoteID, "", "");
                }

            }
            llms_images.ItemsSource = Image_Items;

            if (PhoneApplicationService.Current.State.ContainsKey("edit"))
            {
                tempTitle = titleTextBox.Text;
                tempDetails = detailsTextBox.Text;
                tempTags = schlagwoerterTextBox.Text;

                tempSound_Items = new ObservableCollection<SoundData>(sound_Items);
                tempImage_Items = new ObservableCollection<MyImage>(Image_Items);
                
            }
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
                choose(e.OriginalFileName);
                if (Image_Items.Count > 0)
                {
                    selectAllCheckBox.Visibility = Visibility.Visible;
                }
             }
         }

        //Fertig
        public void choose(string filename)
        {
            isPhotochooser = true;

            MediaLibrary medianbibliothek = new MediaLibrary();

            foreach (Picture picture in medianbibliothek.Pictures)
            {
                if (picture.Name.Equals(Path.GetFileName(filename)))
                {
                    MyImage foto = new MyImage(picture);
                    Image_Items.Insert(0, foto);

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

        //Fertig
        public void img_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            llms_images.EnforceIsSelectionEnabled = true;
            deleteButton.Visibility = Visibility.Visible;
            zurueckButton.Visibility = Visibility.Visible;
        }

        //Fertig
        private void titleTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            titleTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xCC, 0x63, 0x61, 0x61));
            SolidColorBrush _s = new SolidColorBrush(Colors.Transparent);
            titleTextBox.Background = _s;
            if (titleTextBox.Text.ToString().Trim().ToUpper().Equals("TITLE"))
            {
                titleTextBox.Text = "";
            }
        }

        //Fertig
        private void titleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            titleTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0x33, 0x63, 0x61, 0x61));
            if (titleTextBox.Text.Trim().Length == 0)
            {
                titleTextBox.Text = "Title";
            }
        }

        //Fertig
        private void detailsTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            detailsTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xCC, 0x63, 0x61, 0x61));
            SolidColorBrush _s = new SolidColorBrush(Colors.Transparent);
            detailsTextBox.Background = _s;
            if (detailsTextBox.Text.ToString().ToUpper().Equals("DETAILS"))
            {
                detailsTextBox.Text = "";
            }
        }

        //Fertig
        private void detailsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            detailsTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0x33, 0x63, 0x61, 0x61));
            if (detailsTextBox.Text.Trim().Length == 0)
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
                    if (titleTextBox.Text.ToString().Trim().ToUpper().Equals("TITLE"))
                    {
                        titleTextBox.Text = "";
                    }
                    titleTextBox.Text += result.RecognitionResult.Text + " ";
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
                    if (detailsTextBox.Text.ToString().ToUpper().Trim().Equals("DETAILS"))
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

        //Fertig
        private void image_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {

                if (!llms_images.SelectedItems.Contains((MyImage)e.AddedItems[0]))
                {
                    llms_images.SelectedItems.Add(((MyImage)e.AddedItems[0]));
                }
            }
            if (e.RemovedItems.Count > 0)
            {

                if (llms_images.SelectedItems.Contains((MyImage)e.RemovedItems[0]))
                {
                    llms_images.SelectedItems.Remove(((MyImage)e.RemovedItems[0]));
                }

            }

            if (llms_images.SelectedItems.Count < llms_images.ItemsSource.Count)
            {
                isselected2 = false;
                selectAllCheckBox.IsChecked = false;

            }
            else if (llms_images.SelectedItems.Count == llms_images.ItemsSource.Count)
            {
                isselected2 = true;
                selectAllCheckBox.IsChecked = true;

            }
            if (llms_images.SelectedItems.Count == 0)
            {
                llms_images.EnforceIsSelectionEnabled = false;
                zurueckButton.Visibility = Visibility.Collapsed;
                deleteButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                zurueckButton.Visibility = Visibility.Visible;
                deleteButton.Visibility = Visibility.Visible;
            } 
        }

        //Frtig
        private void saveButton_Click(object sender, EventArgs e)
        {
            bool issaved = false;

            if(!titleTextBox.Text.Trim().Equals("") || !titleTextBox.Text.Trim().ToUpper().Equals("TITLE"))
            {
                issaved = true;
            }
            if(!detailsTextBox.Text.Trim().Equals("") || !detailsTextBox.Text.Trim().ToUpper().Equals("DETAILS"))
            {
                issaved = true;
            }
            if(!schlagwoerterTextBox.Text.Trim().Equals(""))
            {
                issaved = true;
            }
            if(Image_Items.Count >= 1)
            {
                issaved = true;
            }
            if(sound_Items.Count >= 1)
            {
                issaved = true;
            }
            if (!issaved)
            {
                MessageBox.Show("Sie müssen mindestens Details der Notiz eingeben!!");
            }
            else
            {
                string title = (titleTextBox.Text.Trim().Equals("") || titleTextBox.Text.Trim().ToUpper().Equals("TITLE"))
                    ? DateTime.Now.ToString("F")
                    : titleTextBox.Text.Trim();

                string details = (detailsTextBox.Text.Trim().Equals("") || detailsTextBox.Text.Trim().ToUpper().Equals("DETAILS"))
                    ? ""
                    : detailsTextBox.Text.Trim();
                anvm.save(NoteID, DateTime.Now, title, details,
                    Image_Items, sound_Items, schlagwoerterTextBox.Text, DateTime.Now);

                PhoneApplicationService.Current.State.Remove("deletedImages");
                PhoneApplicationService.Current.State.Remove("addedImages");
                PhoneApplicationService.Current.State.Remove("OppendImageView");
                PhoneApplicationService.Current.State.Remove("memoryNoteID");

                NavigationService.GoBack();
            }


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
            if (!isselected2)
            {
                isselected2 = false;
                llms_images.EnforceIsSelectionEnabled = true;
                deleteButton.Visibility = Visibility.Visible;
                zurueckButton.Visibility = Visibility.Visible;
                llms_images.SelectedItems.Clear();
                ObservableCollection<MyImage> items = (ObservableCollection<MyImage>)llms_images.ItemsSource;

                foreach (MyImage item in items)
                {
                    llms_images.SelectedItems.Add(item);
                }
                isselected2 = true;
            }
          
        }

        //Fertig
        private void addButton_Click(object sender, RoutedEventArgs e)
        {

            photoChooserTask.Show();
        }

        //Fertig
        private void selectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

            if (llms_images.SelectedItems.Count == llms_images.ItemsSource.Count)
            {
                llms_images.SelectedItems.Clear();
                llms_images.EnforceIsSelectionEnabled = false;
                deleteButton.Visibility = Visibility.Collapsed;
                zurueckButton.Visibility = Visibility.Collapsed;
            }

        }

        //Fertig
        private void zurueckButton_Click(object sender, RoutedEventArgs e)
        {
            llms_images.SelectedItems.Clear();
            llms_images.EnforceIsSelectionEnabled = false;
            deleteButton.Visibility = Visibility.Collapsed;
            zurueckButton.Visibility = Visibility.Collapsed;
        }

        //Fertig
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<MyImage> tempitems = new ObservableCollection<MyImage>(
                (ObservableCollection<MyImage>)llms_images.ItemsSource);

            foreach (MyImage item in tempitems)
            {
                if (llms_images.SelectedItems.Contains(item))
                {
                    if (PhoneApplicationService.Current.State.ContainsKey("deletedImages"))
                    {
                        string cachImages = (PhoneApplicationService.Current.State["deletedImages"] as string);
                        cachImages += item.Name + "|";
                        PhoneApplicationService.Current.State["deletedImages"] = cachImages;
                    }
                    else
                    {
                        string cachImages = item.Name + "|";
                        PhoneApplicationService.Current.State["deletedImages"] = cachImages;
                    }

                    llms_images.ItemsSource.Remove(item);
                    llms_images.SelectedItems.Remove(item);
                    Image_Items.Remove(item);
                }
            }

            if (llms_images.ItemsSource.Count == 0)
            {
                selectAllCheckBox.IsChecked = false;
            }
        }

        //Fertig
        private bool isImageCollectionEquals(ObservableCollection<MyImage> c1, ObservableCollection<MyImage> c2)
        {
            bool isequals = false;
            if (c1.Count == c2.Count)
            {
                if (c1.Count == 0 && c2.Count == 0)
                {
                    isequals = true;
                }
                for (int i = 0; i < c1.Count && !isequals; i++)
                {
                    isequals = c1[i].Equals(c2[i]);
                }
            }
            return isequals;
        }

        //Fertig
        private bool isSoundCollectionEquals(ObservableCollection<SoundData> c1, ObservableCollection<SoundData> c2)
        {
            bool isequals = false;
            if (c1.Count == c2.Count)
            {
                if (c1.Count == 0 && c2.Count == 0)
                {
                    isequals = true;
                }
                for (int i = 0; i < c1.Count && !isequals; i++)
                {
                    isequals = c1[i].Equals(c2[i]);
                }
            }
            return isequals;
        }

        //Fertig
        private void cancelButton_Click(object sender, EventArgs e)
        {

            //!Image_Items.Equals(tempImage_Items) ||
            //            !sound_Items.Equals(tempSound_Items)
            bool isfully = false;
            MessageBoxResult result = MessageBoxResult.Cancel ;
            if (PhoneApplicationService.Current.State.ContainsKey("edit"))
            {
                if (!titleTextBox.Text.Trim().Equals(tempTitle) || 
                        !detailsTextBox.Text.Trim().Equals(tempDetails) ||
                        !schlagwoerterTextBox.Text.Trim().Equals(tempTags)
                    || !isImageCollectionEquals(tempImage_Items, Image_Items)
                    || !isSoundCollectionEquals(tempSound_Items, sound_Items))
                {
                    isfully = true;
                }
            }
            else if((!titleTextBox.Text.Trim().Equals("") && !titleTextBox.Text.Trim().ToUpper().Equals("TITLE")) ||
                (!detailsTextBox.Text.Trim().Equals("") && !detailsTextBox.Text.Trim().ToUpper().Equals("DETAILS")) ||
                !schlagwoerterTextBox.Text.Trim().Equals("") ||
                Image_Items.Count >= 1 ||
                sound_Items.Count >= 1)
            {
                isfully = true;
            }

            if (isfully)
            {
                result = MessageBox.Show("möchten Sie Ihre Einträge wegwerfen !",
                        "schließen", MessageBoxButton.OKCancel);
            }

            if (result == MessageBoxResult.OK || !isfully)
            {
                PhoneApplicationService.Current.State.Remove("memoryNoteID");
                PhoneApplicationService.Current.State.Remove("deletedImages");
                PhoneApplicationService.Current.State.Remove("addedImages");
                PhoneApplicationService.Current.State.Remove("OppendImageView");
                PhoneApplicationService.Current.State.Remove("edit");
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    foreach (SoundData item in sound_Items)
                    {
                        if (isoStore.FileExists(item.FilePath))
                        {
                            isoStore.DeleteFile(item.FilePath);
                        }
                    }
                }
                NavigationService.GoBack();
            }
        }


        //Fertig
        private void RecordAudioChecked(object sender, RoutedEventArgs e)
        {
            AudioPlayer.Stop();
            llms_records.IsEnabled = false;
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("/icons/aufnahme_aktiv.png", UriKind.Relative));
            addRecordButton.Background = brush;
            recorder.Start();
            if(bar_status == 2)
            {
                removeMediaApplicationBarButton();
                addManagementApplicationBarButton();
            }
            EndTimer.Visibility = Visibility.Collapsed;
            progressbar_background.Visibility = Visibility.Collapsed;
            progressbar.Visibility = Visibility.Collapsed;
            CurrentTime.Visibility = Visibility.Collapsed;
        }

        //Fertig
        private void RecordAudioUnchecked(object sender, RoutedEventArgs e)
        {
            recorder.Stop();

            SaveTempAudio(recorder.Buffer);

            llms_records.IsEnabled = true;

            if (sound_Items.Count > 0)
            {
                selectAllRecordCheckBox.Visibility = Visibility.Visible;
            }
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("/icons/aufnahme.png", UriKind.Relative));
            addRecordButton.Background = brush;
        }

        //Fertig 
        private void SaveTempAudio(MemoryStream buffer)
        {

            if (buffer == null)
                throw new ArgumentNullException("Leere Buffer.");

            if (_audioStream != null)
            {
                AudioPlayer.Stop();
                AudioPlayer.Source = null;

                _audioStream.Dispose();
            }

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string _tempFileName = string.Format("{0}.wav", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));

                if (isoStore.FileExists(_tempFileName))
                    isoStore.DeleteFile(_tempFileName);
          

                var bytes = buffer.GetWavAsByteArray(recorder.SampleRate);

                _audioStream = isoStore.CreateFile(_tempFileName);
                _audioStream.Write(bytes, 0, bytes.Length);
                
                SoundData mysound = new SoundData() {FilePath = _tempFileName };
                sound_Items.Add(mysound);
                llms_records.ItemsSource = sound_Items;
                _audioStream.Close();
            }
        }

        //Fertig
        private void record_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {

                if (!llms_records.SelectedItems.Contains((SoundData)e.AddedItems[0]))
                {
                    llms_records.SelectedItems.Add(((SoundData)e.AddedItems[0]));
                }
            }
            if (e.RemovedItems.Count > 0)
            {

                if (llms_records.SelectedItems.Contains((SoundData)e.RemovedItems[0]))
                {
                    llms_records.SelectedItems.Remove(((SoundData)e.RemovedItems[0]));
                }

            }

            if (llms_records.SelectedItems.Count < llms_records.ItemsSource.Count)
            {
                isselected1 = false;
                if (isselected3)
                {
                    selectAllRecordCheckBox.IsChecked = false;
                }
                

            }
            else if (llms_records.SelectedItems.Count == llms_records.ItemsSource.Count)
            {
                isselected1 = true;
                selectAllRecordCheckBox.IsChecked = true;

            }
            if (llms_records.SelectedItems.Count == 0)
            {
                llms_records.EnforceIsSelectionEnabled = false;
                zurueckRecordButton.Visibility = Visibility.Collapsed;
                deleteRecordButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                zurueckRecordButton.Visibility = Visibility.Visible;
                deleteRecordButton.Visibility = Visibility.Visible;
            } 
        }
        private DispatcherTimer playTimer;
        //Fertig
        private void Sound_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!(bool)addRecordButton.IsChecked)
            {
                Grid selector = sender as Grid;

                if (selector == null)
                    return;

                string text = ((SoundData)selector.DataContext).FilePath;
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(text, FileMode.Open, FileAccess.Read))
                    {
                        AudioPlayer.SetSource(fileStream);
                        fileStream.Close();

                    }
                }
                lastPlay.Text = text;
                if (bar_status == 1)
                {
                    removeManagementApplicationBarButton();
                    addMediaApplicationBarButton();
                    zurueckRecordButton.Visibility = Visibility.Visible;
                    deleteRecordButton.Visibility = Visibility.Visible;
                    progressbar.Visibility = Visibility.Visible;
                    EndTimer.Visibility = Visibility.Visible;
                    CurrentTime.Visibility = Visibility.Visible;
                    progressbar_background.Visibility = Visibility.Visible;
                }

                pp_status = 1;

                //progressbar          

                timer();
                pp.IconUri = new Uri("/icons/pause.png", UriKind.Relative);

         
            }
            
        }

        private void timer()
        {
            if (playTimer == null)
            {
                playTimer = new DispatcherTimer();
                playTimer.Interval = TimeSpan.FromMilliseconds(100); //eine Sekunde
                playTimer.Tick += new EventHandler(playTimer_Tick);                
                playTimer.Start();
            }
            _test = false;

        }

        bool _test = false;
        //stellt die Current Time und die Endtime des abgespieleten soundfiles dar
        //und erzeugt die "Füllung" der progressbar
        public void playTimer_Tick(object sender, EventArgs e) {

            string totalSeconds = AudioPlayer.NaturalDuration.TimeSpan.TotalMilliseconds.ToString();
            progressbar.Maximum = Convert.ToDouble(totalSeconds);
            progressbar.ValueChanged -= progressbar_ValueChanged;
            progressbar.Value = AudioPlayer.Position.TotalMilliseconds;
            progressbar.ValueChanged += progressbar_ValueChanged;

            TimeSpan _tsEndTime = AudioPlayer.NaturalDuration.TimeSpan.Subtract(new TimeSpan(0, 0, 1));
            //pp.IconUri = new Uri("/icons/pause.png", UriKind.Relative);
            Debug.WriteLine(progressbar.Value + " / " + progressbar.Maximum);
            CurrentTime.Text = String.Format(@"{0:hh\:mm\:ss}", AudioPlayer.Position);
            EndTimer.Text = String.Format(@"{0:hh\:mm\:ss}", _tsEndTime.ToString()).Substring(0, 8);
            if (progressbar.Value > 0)
                _test = true;
            if (_test && progressbar.Value == 0  && progressbar.Maximum >0)
            {
                pp.IconUri = new Uri("/icons/play.png", UriKind.Relative);
                pp_status = 0;                
                CurrentTime.Text = "bin da";
                
                if (playTimer != null)
                {
                    playTimer.Stop();
                    playTimer = null;
                    _test = false;
                }
                //playTimer = null;
                //playTimer.Tick -= playTimer_Tick;
                
            }
            
        }


        //private bool playCompleted = false;

        private void progressbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            //hier nimmt er nicht nur die händischen veränderungen, sondern jede -> folge: erspielt jede sekunde doppelt ab.
                TimeSpan _t = new TimeSpan(0, 0, 0, 0, (int)progressbar.Value);
            //new TimeSpan(
                AudioPlayer.Position = _t;

                

        }

        //Die Methode wird beim Laden, Stop, löschen und zurück aufgerufen.
        private ApplicationBarIconButton saveAs, save, cancel;
        private int bar_status = 1; // 1 = Management, 2 = Media.
        private void addManagementApplicationBarButton()
        {
            removeMediaApplicationBarButton();
            saveAs = new ApplicationBarIconButton(new Uri("/icons/speichernUnter.png", UriKind.Relative));
            save = new ApplicationBarIconButton(new Uri("/icons/speichern.png", UriKind.Relative));
            cancel = new ApplicationBarIconButton(new Uri("/icons/cancel.png", UriKind.Relative));

            saveAs.Text = "zuordnen";
            save.Text = "speichern";
            cancel.Text = "schließen";

            saveAs.Click += saveAsButton_Click;
            save.Click += saveButton_Click;
            cancel.Click += cancelButton_Click;

            ApplicationBar.Buttons.Add(saveAs);
            ApplicationBar.Buttons.Add(save);
            ApplicationBar.Buttons.Add(cancel);
            bar_status = 1;
        }

        private ApplicationBarIconButton pp, reward, forward, stop;
        private void addMediaApplicationBarButton()
        {
            removeManagementApplicationBarButton();

            pp = new ApplicationBarIconButton(new Uri("/icons/pause.png", UriKind.Relative));
            reward = new ApplicationBarIconButton(new Uri("/icons/reward.png", UriKind.Relative));
            forward = new ApplicationBarIconButton(new Uri("/icons/forward.png", UriKind.Relative));
            stop = new ApplicationBarIconButton(new Uri("/icons/stop.png", UriKind.Relative));

            pp.Text = "pp";
            reward.Text = "rückwärts";
            forward.Text = "vorwärts";
            stop.Text = "stop";

            pp.Click += play_pause_Click;   //TODO
            reward.Click += soundbar_reward_button_Click;
            forward.Click += soundbar_forward_button_Click;
            stop.Click += soundbar_stop_button_Click;

            ApplicationBar.Buttons.Add(pp);
            ApplicationBar.Buttons.Add(reward);
            ApplicationBar.Buttons.Add(forward);
            ApplicationBar.Buttons.Add(stop);

            bar_status = 2;


        }


        //Fertig
        private void removeManagementApplicationBarButton()
        {
            ApplicationBar.Buttons.Remove(saveAs);
            ApplicationBar.Buttons.Remove(save);
            ApplicationBar.Buttons.Remove(cancel);
        }

        //Fertig
        private void removeMediaApplicationBarButton()
        {
            ApplicationBar.Buttons.Remove(pp);
            ApplicationBar.Buttons.Remove(forward);
            ApplicationBar.Buttons.Remove(reward);
            ApplicationBar.Buttons.Remove(stop);
        }

        //Fertig
        int pp_status = 1;  //1 = play, 2 = pause.
        private void play_pause_Click(object sender, EventArgs e)
        {
            pp_status += 1;
            

            if (pp_status == 1)
            {
                pp.IconUri = new Uri("/icons/pause.png", UriKind.Relative);
                AudioPlayer.Play();
                if (playTimer == null)
                {
                    timer();
                }
                playTimer.Start();
            }
            else 
            {
                pp.IconUri = new Uri("/icons/play.png", UriKind.Relative);
                AudioPlayer.Pause();
                playTimer.Stop();
                playTimer = null;
                pp_status = 0;
            }

        }

        //Fertig
        private void soundbar_reward_button_Click(object sender, EventArgs e)
        {
            TimeSpan ts;         
            ts = AudioPlayer.Position;
            if (ts.Seconds > 3)
            {
                TimeSpan _t = new TimeSpan(0, 0, 3);
                AudioPlayer.Position -= _t;
                AudioPlayer.Play();
                if (playTimer!= null && !playTimer.IsEnabled) {
                    playTimer.Start();
                }
                
                pp_status = 1;
                pp.IconUri = new Uri("/icons/pause.png", UriKind.Relative);
            }
        }


        //Fertig
        private void soundbar_forward_button_Click(object sender, EventArgs e)
        {
            TimeSpan ts;
            ts = AudioPlayer.Position;
            if (ts.Seconds + 3 < AudioPlayer.NaturalDuration.TimeSpan.TotalSeconds)
            {
                TimeSpan _t = new TimeSpan(0, 0, 3);
                AudioPlayer.Position += _t;
                AudioPlayer.Play();
                if (playTimer != null && !playTimer.IsEnabled)
                {
                    playTimer.Start();
                }
                pp_status = 1;
                pp.IconUri = new Uri("/icons/pause.png", UriKind.Relative);
            }
        }



        //F.Fertig
        private void soundbar_stop_button_Click(object sender, EventArgs e)
        {
            
            llms_records.EnforceIsSelectionEnabled = false;
            removeMediaApplicationBarButton();
            addManagementApplicationBarButton();
            playTimer.Stop();
            _test = false;
            if (playTimer != null) {
                playTimer = null;
            }
            bar_status = 1;
            lastPlay.Text = "";
            AudioPlayer.Stop();
            
            progressbar.Visibility = Visibility.Collapsed;
            EndTimer.Visibility = Visibility.Collapsed;
            CurrentTime.Visibility = Visibility.Collapsed;
            progressbar_background.Visibility = Visibility.Collapsed;
           
        }

        //Fertig
        private void deleteRecordButton_Click(object sender, RoutedEventArgs e)
        {

            if (llms_records.EnforceIsSelectionEnabled)
            {
                ObservableCollection<SoundData> tempitems = new ObservableCollection<SoundData>(
                (ObservableCollection<SoundData>)llms_records.ItemsSource);

                foreach (SoundData item in tempitems)
                {
                    if (llms_records.SelectedItems.Contains(item))
                    {
                        llms_records.ItemsSource.Remove(item);
                        llms_records.SelectedItems.Remove(item);
                        sound_Items.Remove(item);
                    }
                }
                lastPlay.Text = "";
            }
            else
            {
                string temp_sound = lastPlay.Text;
                SoundData s = ((ObservableCollection<SoundData>)llms_records.ItemsSource).Single(x => x.FilePath == temp_sound);
                if (llms_records.ItemsSource.Contains(s))
                {
                    llms_records.ItemsSource.Remove(s);
                    sound_Items.Remove(s);
                }
                lastPlay.Text = "";
            }

            if (llms_records.ItemsSource.Count == 0 )
            {
                selectAllRecordCheckBox.IsChecked = false;
            }

            deleteRecordButton.Visibility = Visibility.Collapsed;
            zurueckRecordButton.Visibility = Visibility.Collapsed;
            if (bar_status == 2) {
                removeMediaApplicationBarButton();
                addManagementApplicationBarButton();
                bar_status = 1;
            }
            pp_status = 1;
            progressbar.Visibility = Visibility.Collapsed;
            EndTimer.Visibility = Visibility.Collapsed;
            CurrentTime.Visibility = Visibility.Collapsed;
            progressbar_background.Visibility = Visibility.Collapsed;
            //soundbar_hintergrund.Visibility = Visibility.Collapsed;
            AudioPlayer.Stop();
            if (playTimer != null)
            {
                playTimer.Stop();
            }

        }

        //Fertig
        private void zurueckRecordButton_Click(object sender, RoutedEventArgs e)
        {
            llms_records.EnforceIsSelectionEnabled = false;
            lastPlay.Text = "";
            deleteRecordButton.Visibility = Visibility.Collapsed;
            zurueckRecordButton.Visibility = Visibility.Collapsed;
            if (bar_status == 2)
            {
                removeMediaApplicationBarButton();
                addManagementApplicationBarButton();
                bar_status = 1;
            }
            
            progressbar.Visibility = Visibility.Collapsed;
            EndTimer.Visibility = Visibility.Collapsed;
            CurrentTime.Visibility = Visibility.Collapsed;
            progressbar_background.Visibility = Visibility.Collapsed;
            //soundbar_hintergrund.Visibility = Visibility.Collapsed;
            AudioPlayer.Stop();
            if(playTimer != null)
            {
                playTimer.Stop();
            }
            

        }

        //Fertig
        private void selectAllRecordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

            if (llms_records.SelectedItems.Count == llms_records.ItemsSource.Count)
            {
                llms_records.EnforceIsSelectionEnabled = false;
                llms_records.SelectedItems.Clear();
            }
            if (lastPlay.Text.Trim().Equals(""))
            {
                deleteRecordButton.Visibility = Visibility.Collapsed;
                zurueckRecordButton.Visibility = Visibility.Collapsed;
            }
        }

        //Fertig
        private void selectAllRecordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!isselected1)
            {
                isselected3 = false;
                llms_records.EnforceIsSelectionEnabled = true;
                deleteRecordButton.Visibility = Visibility.Visible;
                zurueckRecordButton.Visibility = Visibility.Visible;
                llms_records.SelectedItems.Clear();
                ObservableCollection<SoundData> items = (ObservableCollection<SoundData>)llms_records.ItemsSource;

                foreach (SoundData item in items)
                {
                    llms_records.SelectedItems.Add(item);
                }
                isselected3 = true;
            }
        }

        //Fertig
        private void Grid_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid g = (Grid)sender;
            
            llms_records.EnforceIsSelectionEnabled = true;

            llms_records.SelectedItems.Add(((SoundData)g.DataContext));

            deleteRecordButton.Visibility = Visibility.Visible;
            zurueckRecordButton.Visibility = Visibility.Visible;
        }

        //TODO
        private void saveAsButton_Click(object sender, EventArgs e)
        {
            //PhoneApplicationService.Current.State.Remove("deletedImages");
            //PhoneApplicationService.Current.State.Remove("addedImages");
            //PhoneApplicationService.Current.State.Remove("OppendImageView");
            //PhoneApplicationService.Current.State.Remove("memoryNoteID");
        }

        //Fertig
        private void schlagwoerterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double height = heightCalculator(schlagwoerterTextBox.Text, schlagwoerterTextBox.FontFamily, schlagwoerterTextBox.FontSize,
                     schlagwoerterTextBox.FontStyle, schlagwoerterTextBox.FontWeight);
            if (height > 300)
            {
                detailsTextBox.Height = height;
            }
        }

        //Fertig
        private async void micro3_Tap(object sender, System.Windows.Input.GestureEventArgs e)
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
                   schlagwoerterTextBox.Text += result.RecognitionResult.Text + "; ";
                }
            }
        }

        //Fertigs
        private void schlagwoerterTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            schlagwoerterTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xCC, 0x63, 0x61, 0x61));
            SolidColorBrush _s = new SolidColorBrush(Colors.Transparent);
            schlagwoerterTextBox.Background = _s;
        }

        //Fertig
        private void schlagwoerterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            schlagwoerterTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0x33, 0x63, 0x61, 0x61));
        }



       

        

        


    }
}