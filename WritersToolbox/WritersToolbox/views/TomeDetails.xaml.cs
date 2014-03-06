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
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace WritersToolbox.views
{
    public partial class TomeDetails : PhoneApplicationPage
    {
        //Verbindung zwischen View und Model des Bandes.
        private TomeDetailsViewModel tome_VM;
        //Primäreschlüßel des bandes.
        private int tomeID;
        //Ablage des InformationsCode.
        private int informationCode;
        //Buttons zum speichern/abbrechen beim Bearbeiten eines Chapters
        private ApplicationBarIconButton save, cancel;
        //DefaultBarButtons.
        private ApplicationBarIconButton edit, deleteTypeObject;
        //Textbox Chapter
        TextBox b = new TextBox();
        //Chapter
        private datawrapper.Chapter chapter;
        private bool wasfocuslost;
        public TomeDetails()
        {
            InitializeComponent();
            addDefaultApplicationBarButton();
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
            if (NavigationContext.QueryString.ContainsKey("tomeID"))
            {
                tomeID = int.Parse(NavigationContext.QueryString["tomeID"]);
                tome_VM = new TomeDetailsViewModel(tomeID);
                DataContext = tome_VM;
                int code = tome_VM.getInformation();
                switch (code)
                {
                    case 1:
                        rbNumberOfChapter.IsChecked = true;
                        break;
                    case 2:
                        rbNumberOfEvent.IsChecked = true;
                        break;
                    case 3:
                        rbNumberOfTypeObject.IsChecked = true;
                        break;
                    case 400:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 0;
                        fontSizeList.SelectedIndex = 0;
                        break;
                    case 410:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 1;
                        fontSizeList.SelectedIndex = 0;
                        break;
                    case 420:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 2;
                        fontSizeList.SelectedIndex = 0;
                        break;
                    case 430:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 3;
                        fontSizeList.SelectedIndex = 0;
                        break;
                    case 440:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 4;
                        fontSizeList.SelectedIndex = 0;
                        break;
                    case 401:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 0;
                        fontSizeList.SelectedIndex = 1;
                        break;
                    case 402:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 0;
                        fontSizeList.SelectedIndex = 2;
                        break;
                    case 403:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 0;
                        fontSizeList.SelectedIndex = 3;
                        break;
                    case 411:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 1;
                        fontSizeList.SelectedIndex = 1;
                        break;
                    case 412:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 1;
                        fontSizeList.SelectedIndex = 2;
                        break;
                    case 413:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 1;
                        fontSizeList.SelectedIndex = 3;
                        break;
                    case 421:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 2;
                        fontSizeList.SelectedIndex = 1;
                        break;
                    case 422:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 2;
                        fontSizeList.SelectedIndex = 2;
                        break;
                    case 423:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 2;
                        fontSizeList.SelectedIndex = 3;
                        break;
                    case 431:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 3;
                        fontSizeList.SelectedIndex = 1;
                        break;
                    case 432:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 3;
                        fontSizeList.SelectedIndex = 2;
                        break;
                    case 433:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 3;
                        fontSizeList.SelectedIndex = 3;
                        break;
                    case 441:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 4;
                        fontSizeList.SelectedIndex = 1;
                        break;
                    case 442:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 4;
                        fontSizeList.SelectedIndex = 2;
                        break;
                    case 443:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 4;
                        fontSizeList.SelectedIndex = 3;
                        break;
                    case 5:
                        rbNumberOfWord.IsChecked = true;
                        break;
                }
            }

            llstructure.ItemsSource = tome_VM.structur;
        }

        /// <summary>
        /// Wird unmittelbar aufgerufen, nachdem die Page entladen und nicht mehr 
        /// die aktuelle Quelle eines übergeordneten Frame ist.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //Ausgewählte Information in der Datenbank aktualisieren.
            tome_VM.updateInformation(informationCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            var lastPage = NavigationService.BackStack.FirstOrDefault();
            if (lastPage != null && lastPage.Source.ToString().Equals("/views/EventDetail.xaml"))
            {
                NavigationService.RemoveBackEntry();
            }
        }
        /// <summary>
        /// Information Button wird geklickt
        ///     Bildquelle Information ändert sich -> checked
        ///     Bildquelle Typeobjekt und structure ändert sich -> unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void information_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!doubleTap && !newChapterMode)
            {
                BitmapImage _bitmapimage = new BitmapImage(new Uri("/icons/info_checked.png", UriKind.RelativeOrAbsolute));
                information.Source = _bitmapimage;
                informationGrid.Visibility = Visibility.Visible;

                _bitmapimage = new BitmapImage(new Uri("/icons/struktur_unchecked.png", UriKind.RelativeOrAbsolute));
                structure.Source = _bitmapimage;
                structureGrid.Visibility = Visibility.Collapsed;

                _bitmapimage = new BitmapImage(new Uri("/icons/typen_unchecked.png", UriKind.RelativeOrAbsolute));
                typeObject.Source = _bitmapimage;
                TypeObjectGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                wasfocuslost = false;
            }
        }

        /// <summary>
        /// structure Button wird geklickt
        ///     Bildquelle structure ändert sich -> checked
        ///     Bildquelle Typobject und information ändert sich -> unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void structure_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!doubleTap && !newChapterMode)
            {
                BitmapImage _bitmapimage = new BitmapImage(new Uri("/icons/info_unchecked.png", UriKind.RelativeOrAbsolute));
                information.Source = _bitmapimage;
                informationGrid.Visibility = Visibility.Collapsed;

                _bitmapimage = new BitmapImage(new Uri("/icons/struktur_checked.png", UriKind.RelativeOrAbsolute));
                structure.Source = _bitmapimage;
                structureGrid.Visibility = Visibility.Visible;

                _bitmapimage = new BitmapImage(new Uri("/icons/typen_unchecked.png", UriKind.RelativeOrAbsolute));
                typeObject.Source = _bitmapimage;
                TypeObjectGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                wasfocuslost = false;
            }
        }

        /// <summary>
        /// typeObject Button wird geklickt
        ///     Bildquelle typeobjekt ändert sich -> checked
        ///     Bildquelle Information und structure ändert sich -> unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void typeObjekt_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!doubleTap && !newChapterMode)
            {
                BitmapImage _bitmapimage = new BitmapImage(new Uri("/icons/info_unchecked.png", UriKind.RelativeOrAbsolute));
                information.Source = _bitmapimage;
                informationGrid.Visibility = Visibility.Collapsed;

                _bitmapimage = new BitmapImage(new Uri("/icons/struktur_unchecked.png", UriKind.RelativeOrAbsolute));
                structure.Source = _bitmapimage;
                structureGrid.Visibility = Visibility.Collapsed;

                _bitmapimage = new BitmapImage(new Uri("/icons/typen_checked.png", UriKind.RelativeOrAbsolute));
                typeObject.Source = _bitmapimage;
                TypeObjectGrid.Visibility = Visibility.Visible;
            }
            else
            {
                wasfocuslost = false;
            }
        }

        //FEHLER
        /// <summary>
        /// Öffnet Detailsansicht des Typeobjektes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void typeObject_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            //typeObjektID von sender Object herauslesen.
            int typeObjectID = ((datawrapper.TypeObject)((Grid)sender).DataContext).typeObjectID;
            //TypeobjectID durch URL übergeben.
            NavigationService.Navigate(new Uri("/views/TypeObjectDetails2.xaml?typeObjectID=" + typeObjectID, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Auswahl Anzahl Kapitel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNumberOfChapter_Checked(object sender, RoutedEventArgs e)
        {
            informationCode = 1;
            numberInforamtionText.Text = "" + tome_VM.getNumberOfChapters();
            inforamtionText.Text = "Kapitel";
        }

        /// <summary>
        /// Auswahl Anzahl Ereignisse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNumberOfEvent_Checked(object sender, RoutedEventArgs e)
        {
            informationCode = 2;
            numberInforamtionText.Text = "" + tome_VM.getNumberOfEvents();
            inforamtionText.Text = "Ereignis-se";
        }

        /// <summary>
        /// Auswahl Anzahl Typeobjekte.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNumberOfTypeObject_Checked(object sender, RoutedEventArgs e)
        {
            informationCode = 3;
            numberInforamtionText.Text = "" + tome_VM.getNumberOfTypeObjects();
            inforamtionText.Text = "Typeobjekt-e";
        }

        /// <summary>
        /// Auswahl Anzahhl Seiten.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNumberOfPage_Checked(object sender, RoutedEventArgs e)
        {
            if (informationCode > 399)
            {
                //Um die zweite Stelle herauszulesen.
                formatList.SelectedIndex = (int)(informationCode % 100 / 10);
                //Um die letzte Stelle herauszulesen.
                fontSizeList.SelectedIndex = (int)(informationCode % 10);
            }
            formatList.IsEnabled = true;
            fontSizeList.IsEnabled = true;
        }

        /// <summary>
        /// Auswahl Anzahl Seiten aufheben.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNumberOfPage_Unchecked(object sender, RoutedEventArgs e)
        {
            formatList.IsEnabled = false;
            fontSizeList.IsEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (formatList != null && fontSizeList != null)
            {
                informationCode = 400 + formatList.SelectedIndex * 10 + fontSizeList.SelectedIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fontSizeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (formatList != null && fontSizeList != null)
            {
                informationCode = 400 + formatList.SelectedIndex * 10 + fontSizeList.SelectedIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNumberOfWord_Checked(object sender, RoutedEventArgs e)
        {
            informationCode = 5;
        }

        /// <summary>
        /// Ereignislist aufklappen bzw. zusammenklappen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!doubleTap && !newChapterMode)
            {
                Image img = sender as Image;
                Grid parent = (img.Parent as Grid).Parent as Grid;
                Border border = (parent.Children[0] as Grid).Children[1] as Border;
                TextBox textbox = border.Child as TextBox;
                if (!textbox.Text.Equals("Neues Kapitel") && !newChapterMode)
                {
                    LongListMultiSelector llms = ((parent.Children[1]) as LongListMultiSelector);
                    if (llms.Visibility == Visibility.Collapsed)    //Ist die Ereignislist zusammengeklappt, 
                    {                                               //dann wird sie aufgeklappt.
                        img.Source = new BitmapImage(new Uri("/icons/on.png", UriKind.RelativeOrAbsolute));
                        llms.Visibility = Visibility.Visible;
                    }
                    else      //Ist die Ereignislist aufgeklappt,
                    {         //dann wird sie zusammenfeklappt.
                        img.Source = new BitmapImage(new Uri("/icons/off.png", UriKind.RelativeOrAbsolute));
                        llms.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                wasfocuslost = false;
            }

        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {

            llstructure.IsSelectionEnabled = false;
            l.IsSelectionEnabled = false;
        }

        private void StackPanel_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            llstructure.IsSelectionEnabled = true;
        }

        private void chapterItem_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            llstructure.IsSelectionEnabled = true;
        }

        private void Chapter_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            llstructure.IsSelectionEnabled = true;
        }
        LongListMultiSelector l;
        private void llmsEvent_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            l = sender as LongListMultiSelector;
            datawrapper.Event _e = e.OriginalSource as datawrapper.Event;
            l.IsSelectionEnabled = true;
            l.SelectedItems.Add(_e);
        }

        // false wenn doubleTap eintritt
        bool singleTap;

        // zustand ob d ich Chapter im "doubleTap" -> bearbeitungsmodus befindet
        bool doubleTap = false;

        /// <summary>
        /// Tap auf ein Chapter (Textbox) 
        /// klappt Liste der Ereignisse auf/zu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ChapterTextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            singleTap = true;
            // verzögerung um "tap" von "doubleTap" zu unterscheiden
            
            
            await Task.Delay(200);

            //abfrage ob gesingle- oder gedoubletaped wurde, abfrage ob sich chapter in "bearbeitung" befindet
            if (singleTap && !doubleTap)
            {
                //geklickte Textbox(chapter) holen, entsprechende Eventliste aufklappen/zuklappen + IMG ändern
                //chapter = (sender as TextBox).DataContext as datawrapper.Chapter;
                b = sender as TextBox;
                Image img = ((b.Parent as Border).Parent as Grid).Children[0] as Image;
                Grid parent = ((b.Parent as Border).Parent as Grid).Parent as Grid;
                if (!b.Text.Equals("Neues Kapitel") && !newChapterMode)
                {

                    LongListMultiSelector llms = ((parent.Children[1]) as LongListMultiSelector);
                    if (llms.Visibility == Visibility.Collapsed)    //Ist die Ereignislist zusammengeklappt, 
                    {                                               //dann wird sie aufgeklappt.
                        img.Source = new BitmapImage(new Uri("/icons/on.png", UriKind.RelativeOrAbsolute));
                        llms.Visibility = Visibility.Visible;
                    }
                    else      //Ist die Ereignislist aufgeklappt,
                    {         //wird sie zusammengeklappt.
                        img.Source = new BitmapImage(new Uri("/icons/off.png", UriKind.RelativeOrAbsolute));
                        llms.Visibility = Visibility.Collapsed;
                    }
                }
                else {

                    b.DoubleTap -= ChapterTextBox_TapTap;
                    newChapterTextbox.IsEnabled = true;
                    newChapterTextbox.Visibility = Visibility.Visible;
                   
                    //abfrage, da sonst bei jedem geklickten Kapitel die neues Chapter -> Titel -Textbox gefocust wird (irritierend)
                    if (b.Text.Equals("Neues Kapitel")) {
                        newChapterTextbox.Focus();
                    }
                    

                    newChapterRectangle.Visibility = Visibility.Visible;
                    newChapterTextTitle.Visibility = Visibility.Visible;

                    newChapterTextbox.Select(newChapterTextbox.Text.Length, 0);
                    if (!newChapterMode)
                    {
                        addAddChapterApplicationBarButton();
                    }

                    newChapterMode = true;
                }


            }
            wasfocuslost = false;

        }

        public bool newChapterMode = false;
        /// <summary>
        /// doppelklick auf ein Chapter (Textbox)
        /// Content kann verändert werden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChapterTextBox_TapTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            b = (TextBox)sender;
            
            if (!newChapterMode && !b.Text.Equals("Neues Kapitel"))
            {
                singleTap = false;
            doubleTap = true;  
                              
                chapter = (sender as TextBox).DataContext as datawrapper.Chapter;
                b.IsReadOnly = false;
                //Workaround...
                b.LostFocus -= ChapterTextbox_LostFocus;
                WorkaroundButton.Focus();
                wasfocuslost = false;
                b.Focus();
                Debug.WriteLine("tapTap");
                b.LostFocus += ChapterTextbox_LostFocus;
                //Workaround ende...
                structure.Tap -= structure_Tap;
                information.Tap -= information_Tap;
                typeObject.Tap -= typeObject_Tap;
            }


        }

        /// <summary>
        /// Chapter verliert den Focus
        /// Chapter kann nichtmehr bearbeitet werden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChapterTextbox_LostFocus(object sender, RoutedEventArgs e)
        {

            MessageBoxResult result = MessageBoxResult.Cancel;

            Debug.WriteLine("lostfocus");
            if (doubleTap && !newChapterMode)
            {
                bool isExist = tome_VM.isChapterNameDuplicate(b.Text);

                //Abfrage "Neues Kapitel" auf Schreibweisen (Klein/Groß etc.) prüfen
                if (b.Text.Trim().Equals("") || b.Text.Equals(chapter.title) || b.Text.Equals("Neues Kapitel"))
                {
                    b.Text = chapter.title;
                    wasfocuslost = true;
                    b.IsReadOnly = true;
                    doubleTap = false;
                    singleTap = false;
                    structure.Tap += structure_Tap;
                    information.Tap += information_Tap;
                    typeObject.Tap += typeObject_Tap;
                }
                else if (isExist && !wasfocuslost)
                {
                    b.IsReadOnly = false;
                    doubleTap = true;
                    //Fehlermeldung              
                    result = MessageBox.Show("Dieses Kapitel exisitiert schon. Bitte geben Sie einen anderen Titel an!",
                            "Information", MessageBoxButton.OK);
                    wasfocuslost = false;
                    b.LostFocus -= ChapterTextbox_LostFocus;
                    WorkaroundButton.Focus();
                    b.Focus();
                    b.LostFocus += ChapterTextbox_LostFocus;
                }
                else if (!isExist)
                {
                    //Änderung speichern
                    chapter.title = b.Text;
                    tome_VM.updateChapter(chapter);
                    wasfocuslost = true;
                    b.IsReadOnly = true;
                    doubleTap = false;
                    singleTap = true;


                    structure.Tap += structure_Tap;
                    information.Tap += information_Tap;
                    typeObject.Tap += typeObject_Tap;
                    

                }
                else
                {
                    wasfocuslost = false;
                }

                
            }


        }

        /// <summary>
        /// Die "Default" applicationbar wird eingefügt
        /// </summary>
        private void addDefaultApplicationBarButton()
        {
            //Default Buttons von ApplicationBarButton löschen.
            removeAddChapterApplicationBarButton();

            //ApplicationBarButton mit ManagmentButtons erfüllen.

            edit = new ApplicationBarIconButton(new Uri("/icons/edit.png", UriKind.Relative));
            deleteTypeObject = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));


            edit.Text = "Ändern";
            deleteTypeObject.Text = "Löschen";

            //Events zu Buttons hinzufügen.


            //TODO
            //edit.Click += saveButton_Click;
            //cancel.Click += cancelButton_Click;


            ApplicationBar.Buttons.Add(edit);
            ApplicationBar.Buttons.Add(deleteTypeObject);
        }

        private void removeAddChapterApplicationBarButton()
        {
            ApplicationBar.Buttons.Remove(save);
            ApplicationBar.Buttons.Remove(cancel);
        }

        private void removeDefaultApplicationBarButton()
        {
            ApplicationBar.Buttons.Remove(edit);
            ApplicationBar.Buttons.Remove(deleteTypeObject);
        }

        private void addAddChapterApplicationBarButton()
        {
            //Default Buttons von ApplicationBarButton löschen.
            removeDefaultApplicationBarButton();

            //ApplicationBarButton mit Buttons zum Speichern/Abbrechen beim Anlegen eines Kapitels.

            save = new ApplicationBarIconButton(new Uri("/icons/save.png", UriKind.Relative));
            cancel = new ApplicationBarIconButton(new Uri("/icons/cancel.png", UriKind.Relative));


            save.Text = "Speichern";
            cancel.Text = "Abbrechen";

            //Events zu Buttons hinzufügen.


            //TODO
            save.Click += saveButton_Click;
            cancel.Click += cancelButton_Click;


            ApplicationBar.Buttons.Add(save);
            ApplicationBar.Buttons.Add(cancel);
        }

        /// <summary>
        /// Click auf Save-Button um Kapitel zu speichern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.Cancel;
            bool doesExist = tome_VM.isChapterNameDuplicate(newChapterTextbox.Text);

            if (newChapterTextbox.Text.Trim().Equals("") || newChapterTextbox.Text.Equals("Neues Kapitel"))
            {
                //Fehlermeldung ungültig oder leer           
                result = MessageBox.Show("Der Titel ist leer oder ungültig. Bitte geben Sie einen anderen Titel an!",
                        "Information", MessageBoxButton.OK);
                newChapterTextbox.SelectAll();
            }
            else if (doesExist)
            {
                //Fehlermeldung Kapitel existiert          
                result = MessageBox.Show("Dieses Kapitel exisitiert bereits. Bitte geben Sie einen anderen Titel an!",
                        "Information", MessageBoxButton.OK);
                newChapterTextbox.SelectAll();
            }
            else
            {
                addDefaultApplicationBarButton();
                datawrapper.Chapter _c = new datawrapper.Chapter()
                {
                    addedDate = DateTime.Now,
                    chapterNumber = llstructure.ItemsSource.Count + 1,
                    deleted = false,
                    title = newChapterTextbox.Text,
                    updatedDate = DateTime.Now
                };
                tome_VM.saveChapter(_c);
                newChapterMode = false;

                //b.IsReadOnly = true;
                
                newChapterTextbox.IsEnabled = false;
                newChapterTextbox.Visibility = Visibility.Collapsed;
                newChapterRectangle.Visibility = Visibility.Collapsed;
                newChapterTextTitle.Visibility = Visibility.Collapsed;
                newChapterTextbox.Text = "";
            }



        }

        /// <summary>
        /// Click auf Cancel-Button um das Hinzufügen eines Kapitels abzubrechen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (!newChapterTextbox.Text.Trim().Equals("")) {
                MessageBoxResult result =
                MessageBox.Show("Möchten sie ihre Eingabe wirklich verwerfen?",
                   "Abbrechen", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    addDefaultApplicationBarButton();
                    newChapterMode = false;
                    //b.IsReadOnly = true;
                    newChapterTextbox.IsEnabled = false;
                    newChapterTextbox.Visibility = Visibility.Collapsed;
                    newChapterRectangle.Visibility = Visibility.Collapsed;
                    newChapterTextTitle.Visibility = Visibility.Collapsed;
                    newChapterTextbox.Text = "";
                }
            } else{
                addDefaultApplicationBarButton();
                newChapterMode = false;
                //b.IsReadOnly = true;
                newChapterTextbox.IsEnabled = false;
                newChapterTextbox.Visibility = Visibility.Collapsed;
                newChapterRectangle.Visibility = Visibility.Collapsed;
                newChapterTextTitle.Visibility = Visibility.Collapsed;
                newChapterTextbox.Text = "";
            }
           


            
        }




        /// <summary>
        /// Tap auf ein Event
        /// führt zu EventDetails -> Änderung des Events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Event_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!doubleTap && !newChapterMode)
            {

                datawrapper.Event _event = (sender as TextBlock).DataContext as datawrapper.Event;
                NavigationService.Navigate(new Uri("/views/EventDetail.xaml?eventID=" + _event.eventID, UriKind.RelativeOrAbsolute));
                var lastPage = NavigationService.BackStack.FirstOrDefault();
                if (lastPage != null && lastPage.Source.ToString().Equals("/views/TomeDetails.xaml?tomeID=" + tomeID))
                {
                    NavigationService.RemoveBackEntry();
                }
            }
            else
            {
                wasfocuslost = false;
            }
        }

        

    }
}
