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
using WritersToolbox.datawrapper;

namespace WritersToolbox.views
{
    public partial class addNote : PhoneApplicationPage
    {

        //Konstanten.         
        private const double JUMP_INTERVAL = 0.1;
        private const int MANAGEMENT = 1;
        private const int MEDIA = 2;
        private const int PLAY = 1;
        private const int PAUSE = 2;
        private const int STOP = 0;
        //Um Bilder von MediaLibrary auszuwählen.
        private PhotoChooserTask photoChooserTask;     
        //ViewModel zwischen neue Notiz anlegen und MemoryNote.     
        private AddNoteViewModel anvm;        
        //Primäre Schlüßel.              
        private int NoteID;                      
        //Bilder.                        
        private ObservableCollection<MyImage> Image_Items; 
        //Memors. 
        private ObservableCollection<SoundData> sound_Items;
        //Zwischenspeicher von Bilder.  
        private ObservableCollection<MyImage> tempImage_Items;
        //Zwischenspeicher von Memos.
        private ObservableCollection<SoundData> tempSound_Items;
        //Zwischenspeicher von Daten der Notiz  
        private string tempTitle, tempDetails, tempTags; 
        //Recorder Object um aufzunehmen.
        private MicrophoneRecorder recorder;        
        //Um alle Selektieren zu kontrollieren.
        private bool isAllMemosSelected, isAllPicturesSelected;     
        //Tackt.       
        private DispatcherTimer playTimer,dauerTimer;   
        //letztes abgespielte Memo.               
        private Grid lastMemo;
        //ManagementBarButtons.
        private ApplicationBarIconButton saveAs, save, cancel;
        //MediaBatButtons.
        private ApplicationBarIconButton pp, reward, forward, stop;
        // 1 = Management, 2 = Media. Um ManagementBatButtons zu kontrollieren.
        private int applicationBarButton_Modus;
        //1 = Play, 2 = Pause, 0 = Stop. Um MediaBarButtons zu kontrollieren.
        private int playPauseButton_Modus;    
        //Um slider zu kontrollieren.
        private bool progressbarKontrol;
        //
        private bool isPhotoChooserOpened;
        //
        TimeSpan dauer;
        /// <summary>
        /// Default Konstruktor.
        /// </summary>
        public addNote()
        {
            //Componenten initialisieren.
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask();
            recorder = new MicrophoneRecorder();
            anvm = new AddNoteViewModel();
            //Wenn NoteID schon zu Verfügung ist, wird geholt und geparst.
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
            //Wenn NoteID 0 ist, dann wird eine leere Collection von Bilder zurückgegeben,
            //Sonst werden die Bilder von NoteID zurückgegeben
            //Das wird ausgeführt, wenn noch gar kein Bild in Fullscreen gezeigt ist.
            if (!PhoneApplicationService.Current.State.ContainsKey("OppendImageView"))
            {
                Image_Items = anvm.getImages(NoteID);
                //Bilder in der List hinzufügen.
                this.llms_images.ItemsSource = Image_Items;
            }

            //Wenn NoteID 0 ist, dann wird eine leere Collection von Memos zurückgegeben,
            //Sonst werden die Memos von NoteID zurückgegeben
            sound_Items = anvm.getAudios(NoteID);
            //Memos in der List hinzufügen.
            this.llms_records.ItemsSource = sound_Items;
            //Alle notwendige Daten der Notiz zzurückgeben.
            string temp_title = anvm.getTitle(NoteID);
            string temp_details = anvm.getDetails(NoteID);
            titleTextBox.Text = temp_title.Equals("") ? "Title" : temp_title;
            detailsTextBox.Text = temp_details.Equals("") ? "Details" : temp_details;
            schlagwoerterTextBox.Text = anvm.getTags(NoteID);

            //Um die Größe des Textboxs anzupassen.
            textBoxResize(titleTextBox, 72);
            textBoxResize(detailsTextBox, 408);
            textBoxResize(schlagwoerterTextBox, 300);

            //Design aktualisieren.
            if (Image_Items.Count > 0)
            {
                selectAllCheckBox.Visibility = Visibility.Visible;
            }
            else
            {
                selectAllCheckBox.Visibility = Visibility.Collapsed;
            }

            if (sound_Items.Count > 0)
            {
                selectAllRecordCheckBox.Visibility = Visibility.Visible;
            }
            else
            {
                selectAllRecordCheckBox.Visibility = Visibility.Collapsed;
            }
            addManagementApplicationBarButton();

            
        }

        /// <summary>
        /// Hilfsmethode, um ApplicationBarButton mit ManagmentButtons(Speichern, Zuordnen und Schließen) zu erfüllen.
        /// </summary>
        private void addManagementApplicationBarButton()
        {
            //Mediabuttons von ApplicationBarButton löschen.
            removeMediaApplicationBarButton();
            //Wenn die Notize schon zugewiesen ist, dann wird auf die Zuordnungsoption verzichtet.
            if (!PhoneApplicationService.Current.State.ContainsKey("assignedNote"))
            {
                saveAs = new ApplicationBarIconButton(new Uri("/icons/speichernUnter.png", UriKind.Relative));
                saveAs.Text = "zuordnen";
                saveAs.Click += saveAsButton_Click;
                ApplicationBar.Buttons.Add(saveAs);
            }
            //ApplicationBarButton mit ManagmentButtons erfüllen.
            
            save = new ApplicationBarIconButton(new Uri("/icons/speichern.png", UriKind.Relative));
            cancel = new ApplicationBarIconButton(new Uri("/icons/cancel.png", UriKind.Relative));

            
            save.Text = "speichern";
            cancel.Text = "schließen";

            //Events zu Buttons hinzufügen.
            
            save.Click += saveButton_Click;
            cancel.Click += cancelButton_Click;
            
            
            ApplicationBar.Buttons.Add(save);
            ApplicationBar.Buttons.Add(cancel);
            //ApplicationBarStatus aktualisieren.
            applicationBarButton_Modus = MANAGEMENT;
        }

        /// <summary>
        /// Hilfsmethode, um ApplicationBarButton mit MediaButtons(PP, Reward, Forwad und Stop) erfüllen.
        /// </summary>
        private void addMediaApplicationBarButton()
        {
            //ManagmentButtons von ApplicationBarButtons löschen.
            removeManagementApplicationBarButton();

            //ApplicationBarButton mit MediaButtons erfüllen.
            pp = new ApplicationBarIconButton(new Uri("/icons/pause.png", UriKind.Relative));
            reward = new ApplicationBarIconButton(new Uri("/icons/reward.png", UriKind.Relative));
            forward = new ApplicationBarIconButton(new Uri("/icons/forward.png", UriKind.Relative));
            stop = new ApplicationBarIconButton(new Uri("/icons/stop.png", UriKind.Relative));

            pp.Text = "pp";
            reward.Text = "rückwärts";
            forward.Text = "vorwärts";
            stop.Text = "stop";
            //Events zu Buttons hinzufügen.
            pp.Click += play_pause_Click;
            reward.Click += soundbar_reward_button_Click;
            forward.Click += soundbar_forward_button_Click;
            stop.Click += soundbar_stop_button_Click;

            ApplicationBar.Buttons.Add(reward);
            ApplicationBar.Buttons.Add(pp);
            ApplicationBar.Buttons.Add(forward);
            ApplicationBar.Buttons.Add(stop);
            //ApplicationBarStatus aktualisieren.
            applicationBarButton_Modus = MEDIA;

        }

        /// <summary>
        /// Um die ManagementButtons von ApplicationBarButton zu entfernen.
        /// </summary>
        private void removeManagementApplicationBarButton()
        {
            if (!PhoneApplicationService.Current.State.ContainsKey("assignedNote"))
            {
                ApplicationBar.Buttons.Remove(saveAs);
            }
            ApplicationBar.Buttons.Remove(save);
            ApplicationBar.Buttons.Remove(cancel);
        }

        /// <summary>
        /// Um die ManagementButtons von ApplicationBarButton zu entfernen.
        /// </summary>
        private void removeMediaApplicationBarButton()
        {
            ApplicationBar.Buttons.Remove(pp);
            ApplicationBar.Buttons.Remove(forward);
            ApplicationBar.Buttons.Remove(reward);
            ApplicationBar.Buttons.Remove(stop);
        }

        /// <summary>
        /// Event der Zurückbutton des Handys überschreiben.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NoteDiscard();
        }

        /// <summary>
        /// Diese Methode wird ausgeführt, wenn es zu diesem Screen navigiert wird.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Source der AudioPlayer auf null einsetzen wenn es schon vorher was abgespielt ist,
            //Damit wenn es zu diesem Screen navigiert wird, nicht automatisch abgespielt wird.
            if (AudioPlayer.Source != null)
            {
                AudioPlayer.Source = null;
            }
            if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                if (PhoneApplicationService.Current.State.ContainsKey("typeObjectID"))
                {
                    //Wenn in Title nicht geändert wurde, dann wird automatisch der aktuelle Datum für Title gegeben.
                    string title = (titleTextBox.Text.Trim().Equals("") || titleTextBox.Text.Trim().ToUpper().Equals("TITLE"))
                        ? DateTime.Now.ToString("F")
                        : titleTextBox.Text.Trim();

                    string details = (detailsTextBox.Text.Trim().Equals("") || detailsTextBox.Text.Trim().ToUpper().Equals("DETAILS"))
                        ? ""
                        : detailsTextBox.Text.Trim();

                    //Notiz zuordnen.
                    anvm.saveAsTypeObject(NoteID, DateTime.Now, title, details,
                        Image_Items, sound_Items, schlagwoerterTextBox.Text, DateTime.Now, 
                        (int)PhoneApplicationService.Current.State["typeObjectID"]);

                    //Hilfsvariable in ApplicationService löschen.
                    PhoneApplicationService.Current.State.Remove("deletedImages");
                    PhoneApplicationService.Current.State.Remove("addedImages");
                    PhoneApplicationService.Current.State.Remove("OppendImageView");
                    PhoneApplicationService.Current.State.Remove("memoryNoteID");
                    PhoneApplicationService.Current.State.Remove("assignNote");
                    PhoneApplicationService.Current.State.Remove("typeObjectID");
                    NavigationService.GoBack();
                    return;
                }
            }
            //Überprüfen ob es von Imageview navigiert wurde.
            if (PhoneApplicationService.Current.State.ContainsKey("OppendImageView"))
            {
                //Überprüfen ob vor der Navigation ein Bild hinzugefügt oder gelöscht ist.
                if (PhoneApplicationService.Current.State.ContainsKey("deletedImages")
                    || PhoneApplicationService.Current.State.ContainsKey("addedImages"))
                {
                    //speichern die neue gelöschte oder hinzugefügte Bildern einer Notiz in einem ObservableCollection.
                    string addedImages = PhoneApplicationService.Current.State.ContainsKey("addedImages")
                        ? PhoneApplicationService.Current.State["addedImages"] as string
                        : "";
                    string deletedImages = PhoneApplicationService.Current.State.ContainsKey("deletedImages")
                        ? PhoneApplicationService.Current.State["deletedImages"] as string
                        : "";

                    Image_Items = anvm.getImages(NoteID, deletedImages, addedImages);
                }
                else
                {
                    Image_Items = anvm.getImages(NoteID, "", "");
                }

            }

            //ObservaleCollection von MyImage zu den LonglistMultiSelector übergeben.
            llms_images.ItemsSource = Image_Items;

            //Hier wird geprüft ob es von einem anderen Screen die Notiz geöffnet ist.
            //Um die Daten der Notiz in einem Zwischenspeicher zu speichern, um die 
            //Überprüfung durchzuführen.
            if (!isPhotoChooserOpened && PhoneApplicationService.Current.State.ContainsKey("edit"))
            {
                this.ScreenTitle.Text = "NOTIZ BEARBEITEN";
                tempTitle = titleTextBox.Text;
                tempDetails = detailsTextBox.Text;
                tempTags = schlagwoerterTextBox.Text;
                tempSound_Items = new ObservableCollection<SoundData>(sound_Items);
                tempImage_Items = new ObservableCollection<MyImage>(Image_Items);                
            }
            isPhotoChooserOpened = false;
        }

        /// <summary>
        /// Die methode überprüft, ob die Höhe des eingegebenen Textes mit der Höhe des DetailsTextbox passt.
        /// Wenn nicht dann wird es die Höhe des Detailstextbox vergrössert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBoxResize(detailsTextBox, 408);
        }

        /// <summary>
        /// Die methode überprüft, ob die Höhe des eingegebenen Textes mit der Höhe des titleTextBox passt.
        /// Wenn nicht dann wird es die Höhe des titleTextBox vergrössert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void titleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBoxResize(titleTextBox, 72);
        }

        /// <summary>
        /// Die Methode überprüft, ob die Höhe des eingegebenen Textes mit der Höhe des schlagwoerterTextBox passt.
        /// Wenn nicht dann wird es die Höhe des schlagwoerterTextBox vergrössert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void schlagwoerterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBoxResize(schlagwoerterTextBox, 300);
        }

        /// <summary>
        /// Hilfsmethode um die Höhe des Textes zu berechnen.
        /// </summary>
        /// <param name="str">Text</param>
        /// <param name="font">Font</param>
        /// <param name="size">Größe</param>
        /// <param name="fontstyle">Fontstyle</param>
        /// <param name="fontweight">Fontweight</param>
        /// <param name="t">Textbox</param>
        /// <returns></returns>
        private double heightCalculator(String str, FontFamily font, double size,
            FontStyle fontstyle, FontWeight fontweight, TextBox t)
        {
            TextBlock l = new TextBlock();
            l.TextWrapping = TextWrapping.Wrap;
            l.FontFamily = font;
            l.FontSize = size;
            l.FontStyle = fontstyle;
            l.FontWeight = fontweight;
            l.Width = t.Width;
            l.Text = str;          
            return l.ActualHeight + 71;
        }

        /// <summary>
        /// Um TextBox zu resizen.
        /// </summary>
        /// <param name="textBox">textBox</param>
        /// <param name="size">Anfangsgröße</param>
        private void textBoxResize(TextBox textBox, double size)
        {
            //Durch die Hilfsmethode heightCalculator() wird die Höhe des eingegebenes Text berechnet.
            double height = heightCalculator(textBox.Text, textBox.FontFamily, textBox.FontSize,
                textBox.FontStyle, textBox.FontWeight, textBox);
            //Aktualisierung der Höhe des textBox
            if (height > size)
            {
                if (textBox == titleTextBox)
                {
                    double heightDifference = height - textBox.Height;
                    detailsTextBox.Margin = new Thickness(detailsTextBox.Margin.Left, detailsTextBox.Margin.Top + heightDifference,
                            detailsTextBox.Margin.Right, detailsTextBox.Margin.Bottom);
                }
                textBox.Height = height;

            }
        }

        /// <summary>
        /// Event um Photochooser zu öffnen, um die Bilder von Media auszuwählen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            //ob ein Bild ausgewählt ist.
            if (e.TaskResult == TaskResult.OK)
            {
                isPhotoChooserOpened = true;
                //Hilfsmethode.
                picturSelect(e.OriginalFileName);
                //Um checkbox alle auswählen aktiviere, wenn ein Bild hinzugefügt ist.
                if (Image_Items.Count > 0)
                {
                    selectAllCheckBox.Visibility = Visibility.Visible;
                }
             }
         }

        /// <summary>
        /// Hilfsmethode um den gehölten Bildname von PhotoChooser in Media zu suchen und speichern.
        /// </summary>
        /// <param name="filename"></param>
        private void picturSelect(string filename)
        {
            try
            {           
                MediaLibrary mediabibliothek = new MediaLibrary();
                //Bild in MediaLibrary aussuchen.
                Picture picture = mediabibliothek.Pictures.Where(p =>
                    p.GetPath().Equals(filename)).Single();
                //gefundenes Bild in MediaLibrary als MyImage WrapperKlasse für Images erzeugen,
                //um es in ObservableCollection hinzuzufügen.
                MyImage foto = new MyImage(picture);
                Image_Items.Add(foto);

                //Überprüfen ob addedImages Variable in ApplicationService zu verfügung ist,
                //damit es den Inhalt geholt und dazu das neue Bild hinzugefügt, wenn nicht dann die Variable
                //addedImages neu erzeugen.
                if (PhoneApplicationService.Current.State.ContainsKey("addedImages"))
                {
                    string cachImages = (PhoneApplicationService.Current.State["addedImages"] as string);
                    cachImages += filename + "|";
                    PhoneApplicationService.Current.State["addedImages"] = cachImages;
                }
                else
                {
                    string cachImages = filename + "|";
                    PhoneApplicationService.Current.State["addedImages"] = cachImages;
                }
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Wenn Title der Notiz Fokus bekommt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void titleTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //Hintergrund dynamisch ändern.
            titleTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xCC, 0x63, 0x61, 0x61));
            SolidColorBrush _s = new SolidColorBrush(Colors.Transparent);
            titleTextBox.Background = _s;
            if (titleTextBox.Text.ToString().Trim().ToUpper().Equals("TITLE"))
            {
                titleTextBox.Text = "";
            }
        }

        /// <summary>
        /// Wenn Title der Notiz FoKus verliert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void titleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //Border des Textbox dynamisch ändern.
            titleTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0x33, 0x63, 0x61, 0x61));
            if (titleTextBox.Text.Trim().Length == 0)
            {
                titleTextBox.Text = "Title";
            }
        }

        /// <summary>
        /// Wenn Details der Notiz Fokus bekommt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detailsTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //Hintergrund der Detailstextbox dynamisch ändern.
            detailsTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xCC, 0x63, 0x61, 0x61));
            SolidColorBrush _s = new SolidColorBrush(Colors.Transparent);
            detailsTextBox.Background = _s;
            if (detailsTextBox.Text.ToString().ToUpper().Equals("DETAILS"))
            {
                detailsTextBox.Text = "";
            }
        }

        /// <summary>
        /// Wenn Details der Notiz den Fokus verliert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void detailsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //Border des DetailsTextBox dynamisch ändern.
            detailsTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0x33, 0x63, 0x61, 0x61));
            if (detailsTextBox.Text.Trim().Length == 0)
            {
                detailsTextBox.Text = "Details";
            }
        }

        /// <summary>
        /// Wenn schlagwoerterTextBox den Fokus bekommt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void schlagwoerterTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //Hintergrund und Border der schlagwoerterTextBox dynamisch ändern.
            schlagwoerterTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0xCC, 0x63, 0x61, 0x61));
            SolidColorBrush _s = new SolidColorBrush(Colors.Transparent);
            schlagwoerterTextBox.Background = _s;
        }

        /// <summary>
        /// Wenn SchlagwörterTextBox den Fokus verliert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void schlagwoerterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //Border des schlagwoerterTextBox dynamisch ändern.
            schlagwoerterTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(0x33, 0x63, 0x61, 0x61));
        }

        /// <summary>
        /// Speech to Text Funktion, für Title der Notiz.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void micro1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SpeechToText(titleTextBox);     
        }

        /// <summary>
        /// Speech to Text Funktion, für Details der Notiz.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void micro2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SpeechToText(detailsTextBox);
        }

        /// <summary>
        /// Speech to Text Funktion, für Schlagwörter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void micro3_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SpeechToText(schlagwoerterTextBox);
        }

        /// <summary>
        /// Anwendung der Speech to Text funktion.
        /// </summary>
        /// <param name="textbox"></param>
        private async void SpeechToText(TextBox textbox)
        {
            var sr = new SpeechRecognizerUI();
            sr.Settings.ReadoutEnabled = true;
            sr.Settings.ShowConfirmation = false;
            //Sprache Aufnehmen.
            var result = await sr.RecognizeWithUIAsync();
            sr.Recognizer.Grammars.AddGrammarFromPredefinedType("webSearch", SpeechPredefinedGrammar.WebSearch);
            //Aufnahme ausführen bzw. in Text umwandelt.
            if (result.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
            {
                if ((int)result.RecognitionResult.TextConfidence < (int)SpeechRecognitionConfidence.Medium)
                {
                    MessageBox.Show("Wir haben nicht verstanden, versuchen Sie nochmal !!");
                    await sr.RecognizeWithUIAsync();
                }
                else
                {
                    //Auf welche Textbox wurde die Speech to Text funktion angewendet.
                    if (textbox == schlagwoerterTextBox)
                    {
                        textbox.Text += result.RecognitionResult.Text + "; ";
                    }
                    else if (textbox == titleTextBox)
                    {
                        if (textbox.Text.ToString().Trim().ToUpper().Equals("TITLE"))
                        {
                            textbox.Text = "";
                        }
                        textbox.Text += result.RecognitionResult.Text + " ";
                    }
                    else if (textbox == detailsTextBox)
                    {
                        if (textbox.Text.ToString().ToUpper().Trim().Equals("DETAILS"))
                        {
                            textbox.Text = "";
                        }
                        textbox.Text += result.RecognitionResult.Text + " ";
                    }
                    
                }
            }
        }

        /// <summary>
        /// Wenn ein Imag in Bilderpivot geclickt wird, wird es im Fullscreen Modus angezeigt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageView_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image i = sender as Image;
            PhoneApplicationService.Current.State["imageView"] = i;
            NavigationService.Navigate(new Uri("/views/ImageView.xaml", UriKind.Relative));          
        }

        /// <summary>
        /// Wenn neue Selektion in LongListMultiSelector der Bilder stattfindet,
        /// wird diese event ausgeführt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void image_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Ob ein Bild von der List selektiert.
            if (e.AddedItems.Count > 0)
            {
                //wird es in SelectedItems des LongListMultiSelector hinzugefügt.
                if (!llms_images.SelectedItems.Contains((MyImage)e.AddedItems[0]))
                {
                    llms_images.SelectedItems.Add(((MyImage)e.AddedItems[0]));
                }
            }
            //Ob eine Selektion eines Bildes von der List ausgehoben.
            if (e.RemovedItems.Count > 0)
            {
                //wird es in SelectedItems des LongListMultiSelector gelöscht.
                if (llms_images.SelectedItems.Contains((MyImage)e.RemovedItems[0]))
                {
                    llms_images.SelectedItems.Remove(((MyImage)e.RemovedItems[0]));
                }
            }
            //Wenn Anzahl der selektierte Bilder kleiner als Anzahl der Bilder in der List.
            if (llms_images.SelectedItems.Count < llms_images.ItemsSource.Count)
            {
                isAllPicturesSelected = false;
                selectAllCheckBox.IsChecked = false;
            }
            //Wenn Anzahl der selektierte Bilder gleich Anzahl der Bilder in der List.
            else if (llms_images.SelectedItems.Count == llms_images.ItemsSource.Count)
            {
                isAllPicturesSelected = true;
                selectAllCheckBox.IsChecked = true;
            }
            //Wenn Anzahl der selektierte Bilder gleich 0 ist.
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

        /// <summary>
        /// Wenn neue Selektion in LongListMultiSelector der Records stattfindet,
        /// wird diese event ausgeführt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void record_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Ob ein Record von der List selektiert.
            if (e.AddedItems.Count > 0)
            {
                //wird es in SelectedItems des LongListMultiSelector hinzugefügt.
                if (!llms_records.SelectedItems.Contains((SoundData)e.AddedItems[0]))
                {
                    llms_records.SelectedItems.Add(((SoundData)e.AddedItems[0]));
                }
            }
            //Ob eine Selektion eines Recordes von der List ausgehoben.
            if (e.RemovedItems.Count > 0)
            {
                //wird es in SelectedItems des LongListMultiSelector gelöscht.
                if (llms_records.SelectedItems.Contains((SoundData)e.RemovedItems[0]))
                {
                    llms_records.SelectedItems.Remove(((SoundData)e.RemovedItems[0]));
                }

            }
            //Wenn Anzahl der selektierte Bilder kleiner als Anzahl der Bilder in der List.
            if (llms_records.SelectedItems.Count < llms_records.ItemsSource.Count)
            {
                isAllMemosSelected = false;
                selectAllRecordCheckBox.IsChecked = false;
            }
            //Wenn Anzahl der selektierte Bilder gleich Anzahl der Bilder in der List.
            else if (llms_records.SelectedItems.Count == llms_records.ItemsSource.Count)
            {
                isAllMemosSelected = true;
                selectAllRecordCheckBox.IsChecked = true;

            }
            //Wenn Anzahl der selektierte Bilder gleich 0 ist.
            if (llms_records.SelectedItems.Count == 0)
            {
                llms_records.EnforceIsSelectionEnabled = false;
                addRecordButton.IsEnabled = true;
                zurueckRecordButton.Visibility = Visibility.Collapsed;
                deleteRecordButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                zurueckRecordButton.Visibility = Visibility.Visible;
                deleteRecordButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Notiz speichern.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            //Hilfsvariable für die Kontrolle der Änderungen.
            bool isChanged = false;
            //Änderungen kontrollieren.
            if(!titleTextBox.Text.Trim().Equals("") || !titleTextBox.Text.Trim().ToUpper().Equals("TITLE"))
            {
                isChanged = true;
            }
            if(!detailsTextBox.Text.Trim().Equals("") || !detailsTextBox.Text.Trim().ToUpper().Equals("DETAILS"))
            {
                isChanged = true;
            }
            if(!schlagwoerterTextBox.Text.Trim().Equals(""))
            {
                isChanged = true;
            }
            if(Image_Items.Count >= 1)
            {
                isChanged = true;
            }
            if(sound_Items.Count >= 1)
            {
                isChanged = true;
            }

            //Wenn nichts geändert wurde, dann wird eine Message für Benutzer gezeigt,
            //sonst die Notiz speichern, die Hilfsvariable in ApplicationService löschen
            //und zurück zu dem vorherigen Screen.
            if (!isChanged)
            {
                MessageBox.Show("Sie müssen mindestens Details der Notiz eingeben!!");
            }
            else
            {
                //Wenn in Title nicht geändert wurde, dann wird automatisch der aktuelle Datum für Title gegeben.
                string title = (titleTextBox.Text.Trim().Equals("") || titleTextBox.Text.Trim().ToUpper().Equals("TITLE"))
                    ? DateTime.Now.ToString("F")
                    : titleTextBox.Text.Trim();

                string details = (detailsTextBox.Text.Trim().Equals("") || detailsTextBox.Text.Trim().ToUpper().Equals("DETAILS"))
                    ? ""
                    : detailsTextBox.Text.Trim();
                if(PhoneApplicationService.Current.State.ContainsKey("assignedNote"))
                {
                    if (PhoneApplicationService.Current.State.ContainsKey("typeObjectID"))
                    {
                        //Änderung der zugeordneten Notiz speichern.
                        anvm.saveAsTypeObject(NoteID, DateTime.Now, title, details,
                            Image_Items, sound_Items, schlagwoerterTextBox.Text, DateTime.Now,
                            (int)PhoneApplicationService.Current.State["typeObjectID"]);
                    }
                }
                else
                {
                    //Notiz spiechern.
                    anvm.save(NoteID, DateTime.Now, title, details,
                        Image_Items, sound_Items, schlagwoerterTextBox.Text, DateTime.Now);
                }

                //Hilfsvariable in ApplicationService löschen.
                PhoneApplicationService.Current.State.Remove("deletedImages");
                PhoneApplicationService.Current.State.Remove("addedImages");
                PhoneApplicationService.Current.State.Remove("OppendImageView");
                PhoneApplicationService.Current.State.Remove("memoryNoteID");
                PhoneApplicationService.Current.State.Remove("assignedNote");
                PhoneApplicationService.Current.State.Remove("assignNote");
                PhoneApplicationService.Current.State.Remove("typeObjectID");
                PhoneApplicationService.Current.State.Remove("edit");
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// Wenn ein Bild in BilderPivot festgedruckt, wird diese Event ausgeführt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageView_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //ausgewähltes Bild vom Sender Object holen.
            Image image = (Image)sender;
            //BitmapImage von Image rausholen.
            BitmapImage i = (BitmapImage) image.Source;
            //Selektion aktivieren.
            llms_images.EnforceIsSelectionEnabled = true;
            //Selektiertes Bild in Collection der Selektierte Bilder der MultiLongListSelector hinzufügen.
            llms_images.SelectedItems.Add(((MyImage)image.DataContext));
            deleteButton.Visibility = Visibility.Visible;
            zurueckButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Wenn die Bilder alle ausgewählt sind.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //Wenn Anzahl der selektierte Bilder kleiner als Anzahl der Bilder in der List.
            if (!isAllPicturesSelected)
            {
                //Selektierung aktivieren.
                llms_images.EnforceIsSelectionEnabled = true;
                deleteButton.Visibility = Visibility.Visible;
                zurueckButton.Visibility = Visibility.Visible;
                //SelectedItems der List leeren.
                llms_images.SelectedItems.Clear();
                //Alle Bilder von der List holen.
                ObservableCollection<MyImage> items = (ObservableCollection<MyImage>)llms_images.ItemsSource;
                //Alle geholt Bilder von der List in der SelectedItems hinzufügen.
                foreach (MyImage item in items)
                {
                    llms_images.SelectedItems.Add(item);
                }
                isAllPicturesSelected = true;
            }
          
        }

        /// <summary>
        /// neues Bild hinzufügen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (lastMemo != null)
            {
                stopClick();
            }
            //Event für PhotoChooser hinzufügen.
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            //PhotoChooser hinzufügen.
            photoChooserTask.Show();
        }

        /// <summary>
        /// Wenn Auswahl von alle auswählen in BilderPivot aufgehoben wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //Wenn Anzahl der selektierten Bilder gleich Anzahl der Bilder in der List.
            if (llms_images.SelectedItems.Count == llms_images.ItemsSource.Count)
            {
                //Selektierung in LongListMultiSelector löschen.
                llms_images.SelectedItems.Clear();
                //Selektierung deaktivieren.
                llms_images.EnforceIsSelectionEnabled = false;
                deleteButton.Visibility = Visibility.Collapsed;
                zurueckButton.Visibility = Visibility.Collapsed;
            }

        }

        /// <summary>
        /// Wenn zurückButton in BilderPivot gecklickt wird, 
        /// wird diese Event ausgeführt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zurueckButton_Click(object sender, RoutedEventArgs e)
        {
            //Selektierung in LongListMultiSelector löschen.
            llms_images.SelectedItems.Clear();
            //Selektierung deaktivieren.
            llms_images.EnforceIsSelectionEnabled = false;
            deleteButton.Visibility = Visibility.Collapsed;
            zurueckButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Ein oder mehrere Bild-er löschen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            //Zwischenspeicher von Bilder.
            ObservableCollection<MyImage> tempitems = new ObservableCollection<MyImage>(
                (ObservableCollection<MyImage>)llms_images.ItemsSource);

            //über Zwischenspeicher von Bilder iterieren.
            foreach (MyImage item in tempitems)
            {
                //Überprüfen ob das Bild selektiert.
                if (llms_images.SelectedItems.Contains(item))
                {
                    //Überprüfen ob Variable deletedImage in HashTable der ApplicationServ
                    if (PhoneApplicationService.Current.State.ContainsKey("deletedImages"))
                    {
                        string cachImages = (PhoneApplicationService.Current.State["deletedImages"] as string);
                        cachImages += item.path + "|";
                        PhoneApplicationService.Current.State["deletedImages"] = cachImages;
                    }
                    else//Wenn nicht dann wird die Variable in ApplicationService erstellt, 
                        //und der Pfad des Bilds drin speicher.
                    {
                        string cachImages = item.path + "|";
                        PhoneApplicationService.Current.State["deletedImages"] = cachImages;
                    }
                    //Aktuelles Bild von LongListMultiSelector löschen.
                    llms_images.ItemsSource.Remove(item);
                    llms_images.SelectedItems.Remove(item);
                    //Aktuelles Bild von ObservableCollection löschen.
                    Image_Items.Remove(item);
                }
            }
            //Wenn alle Bilder gelöscht sind, dann muss Checkbox "alle Auswählen" ausgeblindet.
            if (llms_images.ItemsSource.Count == 0)
            {
                selectAllCheckBox.IsChecked = false;
            }
        }

        /// <summary>
        /// Wenn Schließen Button geclickt wird, wird diese Event ausgefürht.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            NoteDiscard();
        }

        /// <summary>
        /// Hilfsmethode um die Notiz nach schließen oder zurückkehren bei neuer Notiz anlegen wegzuwerfen.
        /// </summary>
        private void NoteDiscard()
        {
            //Hilfsvariable, um zu prüfen, ob etwas geändert wurde.
            bool isfully = false;

            MessageBoxResult result = MessageBoxResult.Cancel;
            //Hier wird geprüft, ob die Notiz, von anderem Screen geöffnet ist,
            //oder von neue Notiz anlegen.
            if (PhoneApplicationService.Current.State.ContainsKey("edit"))
            {
                if (!titleTextBox.Text.Trim().Equals(tempTitle) ||
                        !detailsTextBox.Text.Trim().Equals(tempDetails) ||
                        !schlagwoerterTextBox.Text.Trim().Equals(tempTags)
                    || !MyImage.isImageCollectionEquals(tempImage_Items, Image_Items)
                    || !SoundData.isSoundCollectionEquals(tempSound_Items, sound_Items))
                {
                    isfully = true;
                }
            }
            else if ((!titleTextBox.Text.Trim().Equals("") && !titleTextBox.Text.Trim().ToUpper().Equals("TITLE")) ||
                (!detailsTextBox.Text.Trim().Equals("") && !detailsTextBox.Text.Trim().ToUpper().Equals("DETAILS")) ||
                !schlagwoerterTextBox.Text.Trim().Equals("") ||
                Image_Items.Count >= 1 ||
                sound_Items.Count >= 1)
            {
                isfully = true;
            }

            //Abfragen, wenn was geändert oder hinzugefügt ist, ob es wirklich weggeworfen werden muss.
            if (isfully)
            {
                result = MessageBox.Show("möchten Sie Ihre Einträge wegwerfen !",
                        "schließen", MessageBoxButton.OKCancel);
            }

            //Wenn ok ausgewählt ist und gar nichts in der Notiz geändert oder hinzugefügt ist.
            //dann wird alles(gespeicherte memos und variable) gelöscht. 
            if (result == MessageBoxResult.OK || !isfully)
            {
                if (!PhoneApplicationService.Current.State.ContainsKey("edit"))
                {
                    using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        foreach (SoundData item in sound_Items)
                        {
                            if (isoStore.FileExists(item.filePath))
                            {
                                isoStore.DeleteFile(item.filePath);
                            }
                        }
                    }
                }
                PhoneApplicationService.Current.State.Remove("deletedImages");
                PhoneApplicationService.Current.State.Remove("addedImages");
                PhoneApplicationService.Current.State.Remove("OppendImageView");
                PhoneApplicationService.Current.State.Remove("memoryNoteID");
                PhoneApplicationService.Current.State.Remove("assignedNote");
                PhoneApplicationService.Current.State.Remove("assignNote");
                PhoneApplicationService.Current.State.Remove("typeObjectID");
                PhoneApplicationService.Current.State.Remove("edit");
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// Wenn ein Memo Aufgenommen wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordAudioChecked(object sender, RoutedEventArgs e)
        {
            //Die Größe der List anpassen.
            llms_records.Margin = new Thickness(27, 84, 0, 17);
            //AudioPlayer stopen, wenn er noch am laufen ist.
            AudioPlayer.Stop();
            //Ob schon ein Memo abgespielt ist.
            if (lastMemo != null)
            {
                //MemoGrid wird initialisiert.
                //((Grid)lastMemo.Children[0]).Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x2E, 0x2E, 0x2E));
                ((Image)lastMemo.Children[0]).Source = new BitmapImage(new Uri("/icons/play_reordButton.png", UriKind.Relative));
                lastMemo = null;
            }
            //Liste der Memos sperren.
            llms_records.IsEnabled = false;
            //Icon des Recordes ändern, dass der Record aktiv ist.
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("/icons/aufnahme_aktiv.png", UriKind.Relative));
            addRecordButton.Background = brush;
            //Record starten
            recorder.Start();
            //DauerTimer erzeugen.
            dauerTimerGenerator();
            //Überprüfen ob ApplicationBarButton aus den MediaButtons besteht.
            if(applicationBarButton_Modus == MEDIA)
            {
                addManagementApplicationBarButton();
            }

            EndTimer.Visibility = Visibility.Collapsed;
            progressbar_background.Visibility = Visibility.Collapsed;
            progressbar.Visibility = Visibility.Collapsed;
            CurrentTime.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Aufnahme eines Memos stopen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordAudioUnchecked(object sender, RoutedEventArgs e)
        {
            //Aufnahme stopen.
            recorder.Stop();
            //DauerTimer zerstören.
            if (dauerTimer != null)
            {
                dauerTimer.Stop();
                dauerTimer = null;
            }
            //Durch Hilfsmethode SaveTempAudio die Aufnahme in IsolatedStorage speichern.
            SaveTempAudio(recorder.Buffer);
            //List der Memos freischalten.
            llms_records.IsEnabled = true;
            //Wenn mindestens ein Memo gespeichert ist, dann wird CheckBox "alle auswählen" erscheinen.
            if (sound_Items.Count > 0)
            {
                selectAllRecordCheckBox.Visibility = Visibility.Visible;
            }
            //Icon des Records ändern auf bereits Aufzunehmen.
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("/icons/aufnahme.png", UriKind.Relative));
            addRecordButton.Background = brush;
        }

        /// <summary>
        /// Hilfsmethode um Aufnahme in IsolatedStorage zu speichern.
        /// </summary>
        /// <param name="buffer"></param>
        private void SaveTempAudio(MemoryStream buffer)
        {
            //Audiodatei.
            IsolatedStorageFileStream _audioStream;
            //Überprüfen ob buffer leer ist.
            if (buffer == null)
                throw new ArgumentNullException("Leere Buffer.");
            //IsolatedStorage öffnen.
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //Filename generieren.
                string[] fileNames = isoStore.GetFileNames("Record*");
                int lastindex = findlastFile(fileNames);
                string filename = "Record " + (lastindex + 1);
                //Name des Records durch aktuelles Datum erstellen.
                string _tempFileName = string.Format("{0}.wav", filename);
                
                //Überprüfen ob der Name schon in IsolatedStorage schon zu Verfügung steht.
                if (isoStore.FileExists(_tempFileName))
                {                
                    isoStore.DeleteFile(_tempFileName); //Wenn ja, der alte Record löschen.
                }                      
                //Speichern die Aufnahme als Bytearray.
                var bytes = buffer.GetWavAsByteArray(recorder.SampleRate);
                //Datei mit dem name "_tempFileName" von IsolatedStorage erstellen
                //und die Datei referenz zu _audioStream übergeben.
                _audioStream = isoStore.CreateFile(_tempFileName);
                //Bytearrax in Datei "bytes" schreiben.
                _audioStream.Write(bytes, 0, bytes.Length);
                //Datei "_audioStream" schließen.
                _audioStream.Close();
                //Erstellen ein SoundData-Object, das den Path des Recordes in IsolatedStorage beinhaltet
                //damit es zu ObservableCollection übergegeben wird.
                SoundData mysound = new SoundData()
                {
                    filePath = _tempFileName,
                    erstellDatum = DateTime.Now,
                    dauer = this.dauer
                };              
                sound_Items.Add(mysound);
                //LongListMultiSelector aktualisieren.
                llms_records.ItemsSource = sound_Items;        
            }
        }

        /// <summary>
        /// Um den Name des letzten erzeugte Memo zurückzugeben.
        /// </summary>
        /// <param name="ary">alle Memosnamen.</param>
        /// <returns>letztes Memo</returns>
        private int findlastFile(string[] ary)
        {
            int lastFile = 0;
            if (ary.Length > 0)
            {
                lastFile = int.Parse(ary[0].Substring(7, ary[0].Length - 11));
                for (int i = 1; i < ary.Length; i++)
                {
                    if (lastFile
                        < int.Parse(ary[i].Substring(7, ary[i].Length - 11)))
                    {
                        lastFile = int.Parse(ary[i].Substring(7, ary[i].Length - 11));
                    }
                }
            }
            return lastFile;
        }

        /// <summary>
        /// Wenn ein Memo geclickt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sound_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Überprüfen, ob Aufnahme aktiv ist.
            if (!(bool)addRecordButton.IsChecked)
            {
                //Größe der List der Records anpassen.
                llms_records.Margin = new Thickness(27,84,0,92);
                //ob ein Record vorher gespielt ist.
                if (lastMemo != null)
                {
                    //Wird hier das letzte gespielte Record initialisiert.
                    //lastMemo.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                    ((Image)lastMemo.Children[0]).Source = new BitmapImage(new Uri("/icons/play_reordButton.png", UriKind.Relative));
                    lastMemo = null;
                }
              
                //ausgewählte Memo von Sender Object holen.
                Grid selector = sender as Grid;
                //Überprüfen ob was ausgewählt nicht null ist.
                if (selector == null)
                    return;
                //Grid Auf Abspielenmodus aktualisieren.
                //selector.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xEB, 0xEB, 0xEB));
                ((Image)selector.Children[0]).Source = new BitmapImage(new Uri("/icons/speaker_reordButton.png", UriKind.Relative));             
                //Name des ausgewählte Memo zuruückgeben.
                string filePath = ((SoundData)selector.DataContext).filePath;
                try
                {
                    //Suche nach den Memo in IsolatedStorage durch den geholten Name.
                    using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        //Die Datei öffnen.
                        using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(filePath, FileMode.Open, FileAccess.Read))
                        {
                            //Die geöffnete Datei zu AudioPlayer-Element übergeben.
                            AudioPlayer.SetSource(fileStream);
                            //Datei schließen.
                            fileStream.Close();
                        }
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("Die Datei existiert nicht.");
                }
                //lastMemo aktualisieren.
                lastMemo = selector;
                //Überprüfen ob die ApplicationBarButton auf ManagmentModus ist.
                if (applicationBarButton_Modus == MANAGEMENT)
                {
                    //Die ApplicationBarButton auf MediaModus umstellen.
                    addMediaApplicationBarButton();
                    deleteRecordButton.Visibility = Visibility.Visible;
                    progressbar.Visibility = Visibility.Visible;
                    EndTimer.Visibility = Visibility.Visible;
                    CurrentTime.Visibility = Visibility.Visible;
                    progressbar_background.Visibility = Visibility.Visible;
                }
                //Play_Pause status ändern.
                playPauseButton_Modus = PLAY;
                //Timer erstellen um Slider zu aktualisieren, während abspielen des Memos.
                timerGenerator();
                //Icon des Play_Pause Button auf Pause ändern.
                pp.IconUri = new Uri("/icons/pause.png", UriKind.Relative);      
            }
            
        }

        /// <summary>
        /// Hilfsmethode um Timer zu erzeugen.
        /// </summary>
        private void timerGenerator()
        {
            //Wenn playTimer nicht existiert.
            if (playTimer == null)
            {
                playTimer = new DispatcherTimer();
                playTimer.Interval = TimeSpan.FromMilliseconds(100); //eine Sekunde
                playTimer.Tick += new EventHandler(playTimer_Tick);                
                playTimer.Start();
            }
            progressbarKontrol = false; //Überprüfen ob so funktioniert.

        }

        private void dauerTimerGenerator()
        {
            //Wenn playTimer nicht existiert.
            if (dauerTimer == null)
            {
                dauerTimer = new DispatcherTimer();
                dauerTimer.Interval = TimeSpan.FromMilliseconds(1000); //eine Sekunde
                dauerTimer.Tick += new EventHandler(dauerTimer_Tick);
                dauer = new TimeSpan();
                dauerTimer.Start();
            }
        }

        private void dauerTimer_Tick(object sender, EventArgs e)
        {
            dauer += new TimeSpan(0, 0, 1);
        }


        /// <summary>
        /// Stellt die Current Time und die Endtime des abgespieleten soundfiles dar
        /// und erzeugt die "Füllung" der progressbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void playTimer_Tick(object sender, EventArgs e) {
            if (playPauseButton_Modus == PLAY)
            {
                //Gesamte Dauer des Memos als String holen.
                string totalSeconds = AudioPlayer.NaturalDuration.TimeSpan.TotalMilliseconds.ToString();
                //Maximum der Slider einsetzen.
                progressbar.Maximum = Convert.ToDouble(totalSeconds);
                //ValueChanged Event von Slider deaktivieren.
                progressbar.ValueChanged -= progressbar_ValueChanged;
                //Aktuelle Seconds des abgespielten Memos zu Slider eingeben.
                progressbar.Value = AudioPlayer.Position.TotalMilliseconds;
                //ValueChanged Event von Slider aktivieren.
                progressbar.ValueChanged += progressbar_ValueChanged;
                //Eine Sekunde von normale Dauer substrahieren.
                TimeSpan _tsEndTime = AudioPlayer.NaturalDuration.TimeSpan.Subtract(new TimeSpan(0, 0, 1));

                //Current- und Endtime des Audioplayer aktualisieren.
                CurrentTime.Text = String.Format(@"{0:hh\:mm\:ss}", AudioPlayer.Position);
                EndTimer.Text = String.Format(@"{0:hh\:mm\:ss}", _tsEndTime.ToString()).Substring(0, 8);
                //Überprüfen, ob es Memo zu End abgespielt ist.
                if (progressbar.Value > 0)
                    progressbarKontrol = true;
                if (progressbarKontrol && progressbar.Value == 0 && progressbar.Maximum > 0)
                {
                    pp.IconUri = new Uri("/icons/play.png", UriKind.Relative);
                    if (lastMemo != null)
                    {
                        //lastMemo.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                        ((Image)lastMemo.Children[0]).Source = new BitmapImage(new Uri("/icons/pause_reordButton.png", UriKind.Relative));
                    }
                    playPauseButton_Modus = STOP;
                    //Wenn playTimer noch aktiv ist, muss die Referenz gelöscht werden.             
                    if (playTimer != null)
                    {
                        playTimer.Stop();
                        playTimer = null;
                        progressbarKontrol = false;
                    }

                }
            }         
        }

        /// <summary>
        /// Event, Um Slider zu aktualisieren.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void progressbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeSpan _t = new TimeSpan(0, 0, 0, 0, (int)progressbar.Value);
            AudioPlayer.Position = _t;
        }

        /// <summary>
        /// Diese Event wird sowohl für Pause als auch für Play ausgeführt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void play_pause_Click(object sender, EventArgs e)
        {
            //1 ist für Play status nachgedacht.
            playPauseButton_Modus += 1;
            if (playPauseButton_Modus == PLAY) //Play
            {
                pp.IconUri = new Uri("/icons/pause.png", UriKind.Relative);
                AudioPlayer.Play();
                //Wenn playTimer nicht existiert, dann wird neu erzeugt.
                timerGenerator();
                //Timer starten.
                playTimer.Start();
            }
            else if(playPauseButton_Modus == PAUSE) //Pause
            {
                pp.IconUri = new Uri("/icons/play.png", UriKind.Relative);
                if (lastMemo != null)
                {
                    //lastMemo.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                    ((Image)lastMemo.Children[0]).Source = new BitmapImage(new Uri("/icons/pause_reordButton.png", UriKind.Relative));
                }
                AudioPlayer.Pause();
                playPauseButton_Modus = STOP;
            }

        }

        /// <summary>
        /// Memo mit einem Zeitraum "JUMP_INTERVAL" rückwerts abspielen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundbar_reward_button_Click(object sender, EventArgs e)
        {
            //Aktuelle Position der AudioPlayer holen.
            TimeSpan ts = AudioPlayer.Position;
            //Gesamte Dauer des Memos in Milliseconds holen.
            double totalMilliseconds = AudioPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            //Überprüfen, ob aktuelle Position des Memos nach dem rückwerts nicht im Negativen Bereich befindet.
            //Wenn ja, dann wird AudioPlayer auf 0 eingestezt.
            if (ts.TotalMilliseconds - (totalMilliseconds * JUMP_INTERVAL) > 0)
            {
                //vordefinierte Jumpintervall von Aktualler Dauer substrahieren.
                TimeSpan _t = new TimeSpan(0, 0, 0, 0, (int)(totalMilliseconds * JUMP_INTERVAL));
                AudioPlayer.Position -= _t;
                AudioPlayer.Play();
                //Wenn playTimer nicht existiert, neu erzeugen.
                timerGenerator();
                playPauseButton_Modus = PLAY;
                pp.IconUri = new Uri("/icons/pause.png", UriKind.Relative);

            }
            else
            {
                progressbar.Value = 0.0;
                AudioPlayer.Stop();
                if (lastMemo != null)
                {
                    //lastMemo.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                    ((Image)lastMemo.Children[0]).Source = new BitmapImage(new Uri("/icons/pause_reordButton.png", UriKind.Relative));
                }
                pp.IconUri = new Uri("/icons/play.png", UriKind.Relative);
            }
        }

        /// <summary>
        /// Memo mit einem Zeitraum "JUMP_INTERVAL" vorwerts abspielen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundbar_forward_button_Click(object sender, EventArgs e)
        {
            //Aktuelle Position der AudioPlayer holen.
            TimeSpan ts = AudioPlayer.Position;
            //Gesamte Dauer des Memos in Milliseconds holen.
            double totalMilliseconds = AudioPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            //Überprüfen, ob aktuelle Position des Memos nachdem vorwerts nicht den Maximal Dauer überschreitet.
            //Wenn ja, dann wird AudioPlayer auf 0 eingestezt.
            if (ts.TotalMilliseconds + (totalMilliseconds * JUMP_INTERVAL) < totalMilliseconds)
            {
                //vordefinierte Jumpintervall von Aktualler Dauer addieren.
                TimeSpan _t = new TimeSpan(0, 0, 0, 0, (int)(totalMilliseconds * JUMP_INTERVAL));
                AudioPlayer.Position += _t;
                AudioPlayer.Play();
                //Wenn playTimer nicht existiert, dann neu erzeugen.
                timerGenerator();
                playPauseButton_Modus = PLAY;
                pp.IconUri = new Uri("/icons/pause.png", UriKind.Relative);
            }
            else
            {
                progressbar.Value = 0.0;
                AudioPlayer.Stop();
                pp.IconUri = new Uri("/icons/play.png", UriKind.Relative);
                if (lastMemo != null)
                {
                    //lastMemo.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                    ((Image)lastMemo.Children[0]).Source = new BitmapImage(new Uri("/icons/pause_reordButton.png", UriKind.Relative));
                }
            }
        }

        /// <summary>
        /// Memo stopen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void soundbar_stop_button_Click(object sender, EventArgs e)
        {
            stopClick();
        }

        /// <summary>
        /// Hilfsmethode um Memo zu stopen, weil Sie auch Ohne Event ausführbar gemacht werden muss.
        /// </summary>
        private void stopClick()
        {
            //Größe der Liste aktualisieren.
            llms_records.Margin = new Thickness(27, 84, 0, 17);
            //Selektion deaktivieren.
            llms_records.EnforceIsSelectionEnabled = false;
            addManagementApplicationBarButton();
            //Überprüfen ob was gespielt wurde, damit es aktualisiert wird.
            if (lastMemo != null)
            {
                //lastMemo.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
                ((Image)lastMemo.Children[0]).Source = new BitmapImage(new Uri("/icons/play_reordButton.png", UriKind.Relative));
                lastMemo = null;
            }
            //Überprüfen, ob ein playTimer existiert, damit es gelöscht wird.
            if (playTimer != null)
            {
                playTimer.Stop();
                playTimer = null;
            }
            AudioPlayer.Stop();

            deleteRecordButton.Visibility = Visibility.Collapsed;
            progressbar.Visibility = Visibility.Collapsed;
            EndTimer.Visibility = Visibility.Collapsed;
            CurrentTime.Visibility = Visibility.Collapsed;
            progressbar_background.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Ein oder mehrere Memo-s löschen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteRecordButton_Click(object sender, RoutedEventArgs e)
        {
            //Größe der List aktualisiern.
            llms_records.Margin = new Thickness(27, 84, 0, 17);
            AudioPlayer.Stop();
            //Wenn Timer noch existiert, muss gelöscht werden.
            if (playTimer != null)
            {
                playTimer.Stop();
                playTimer = null;
            }
            //Abfrage, ob es wirklich was gelöscht werden muss.
            MessageBoxResult result = MessageBox.Show("möchten Sie die Aufnahme löschen !",
                        "Warnung", MessageBoxButton.OKCancel);

            //Wenn ja.
            if (result == MessageBoxResult.OK)
            {
                //Löschen was selektiert ist.
                if (llms_records.EnforceIsSelectionEnabled)
                {
                    //Alle Memos in eine Collection copieren.
                    ObservableCollection<SoundData> tempitems = new ObservableCollection<SoundData>(
                    (ObservableCollection<SoundData>)llms_records.ItemsSource);
                    //Überprüfen, welche Memo ist selektiert, damit es gelöscht wird.
                    foreach (SoundData item in tempitems)
                    {
                        if (llms_records.SelectedItems.Contains(item))
                        {
                            llms_records.ItemsSource.Remove(item);
                            llms_records.SelectedItems.Remove(item);
                            sound_Items.Remove(item);
                        }
                    }
                }
                else    //Löschen was zu letzt abgespielt ist.
                {
                    if (lastMemo != null)
                    {
                        //Letztes gespielte Memo holen und löschen.
                        Grid tempGrid = (Grid)lastMemo.Children[1];
                        string temp_sound = ((TextBlock)tempGrid.Children[0]).Text;
                        SoundData s = ((ObservableCollection<SoundData>)llms_records.ItemsSource).Single(x => x.filePath == temp_sound);
                        if (llms_records.ItemsSource.Contains(s))
                        {
                            llms_records.ItemsSource.Remove(s);
                            sound_Items.Remove(s);
                        }
                    }
                }

                if (llms_records.ItemsSource.Count == 0)
                {
                    selectAllRecordCheckBox.IsChecked = false;
                }

                deleteRecordButton.Visibility = Visibility.Collapsed;
                zurueckRecordButton.Visibility = Visibility.Collapsed;
                //Überprüfen, ob ApplicationBarButton auf MediaModus war, damit sie 
                //mit ManagementButtons erfüllt wird.
                if (applicationBarButton_Modus == MEDIA)
                {
                    addManagementApplicationBarButton();
                }
                playPauseButton_Modus = PLAY;
                progressbar.Visibility = Visibility.Collapsed;
                EndTimer.Visibility = Visibility.Collapsed;
                CurrentTime.Visibility = Visibility.Collapsed;
                progressbar_background.Visibility = Visibility.Collapsed;
            }
            
        }

        /// <summary>
        /// Diese Event wird ausgeführt, wenn zurückButton in RecordPivot geclickt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zurueckRecordButton_Click(object sender, RoutedEventArgs e)
        {
            //Selektierung deaktivieren.
            llms_records.EnforceIsSelectionEnabled = false;
            addRecordButton.IsEnabled = true;
            deleteRecordButton.Visibility = Visibility.Collapsed;
            zurueckRecordButton.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// Wenn Auswahl von alle auswählen in RecordPivot aufgehoben wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllRecordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //Wenn alle Memos selektiert sind.
            if (llms_records.SelectedItems.Count == llms_records.ItemsSource.Count)
            {
                llms_records.EnforceIsSelectionEnabled = false;
                llms_records.SelectedItems.Clear();
            }
        }

        /// <summary>
        /// Wenn auf alle auswählen geclickt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllRecordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //Wenn Anzahl der selektierte Records kleiner als Anzahl der Records in der List.
            if (!isAllMemosSelected)
            {
                //Selektierung aktivieren.
                llms_records.EnforceIsSelectionEnabled = true;
                deleteRecordButton.Visibility = Visibility.Visible;
                zurueckRecordButton.Visibility = Visibility.Visible;
                llms_records.SelectedItems.Clear();
                //Alle Memos selektieren.
                ObservableCollection<SoundData> items = (ObservableCollection<SoundData>)llms_records.ItemsSource;
                foreach (SoundData item in items)
                {
                    llms_records.SelectedItems.Add(item);
                }
            }
        }

        /// <summary>
        /// Wenn ein Memos festgedruckt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Ausgewähle Memo von Sender-Object holen.
            Grid g = (Grid)sender;
            //Aufnehmen ausblinden.
            addRecordButton.IsEnabled = false;
            //Selektierung aktivieren.
            llms_records.EnforceIsSelectionEnabled = true;
            //Ausgewählte Memo selektieren.
            llms_records.SelectedItems.Add(((SoundData)g.DataContext));
            deleteRecordButton.Visibility = Visibility.Visible;
            zurueckRecordButton.Visibility = Visibility.Visible;
        }

        //TODO
        private void saveAsButton_Click(object sender, EventArgs e)
        {
            PhoneApplicationService.Current.State["assignNote"] = true;
            NavigationService.Navigate(new Uri("/views/StartPage.xaml", UriKind.Relative));

            //Hilfsvariable für die Kontrolle der Änderungen.
            bool isChanged = false;
            //Änderungen kontrollieren.
            if (!titleTextBox.Text.Trim().Equals("") || !titleTextBox.Text.Trim().ToUpper().Equals("TITLE"))
            {
                isChanged = true;
            }
            if (!detailsTextBox.Text.Trim().Equals("") || !detailsTextBox.Text.Trim().ToUpper().Equals("DETAILS"))
            {
                isChanged = true;
            }
            if (!schlagwoerterTextBox.Text.Trim().Equals(""))
            {
                isChanged = true;
            }
            if (Image_Items.Count >= 1)
            {
                isChanged = true;
            }
            if (sound_Items.Count >= 1)
            {
                isChanged = true;
            }

            //Wenn nichts geändert wurde, dann wird eine Message für Benutzer gezeigt,
            //sonst die Notiz zuordnen, die Hilfsvariable in ApplicationService löschen
            //und zurück zu dem vorherigen Screen.
            if (!isChanged)
            {
                MessageBox.Show("Sie müssen mindestens Details der Notiz eingeben!!");
            }
            else
            {
                PhoneApplicationService.Current.State["assignNote"] = true;
                NavigationService.Navigate(new Uri("/views/StartPage.xaml", UriKind.Relative));
            }
        }

    }
}