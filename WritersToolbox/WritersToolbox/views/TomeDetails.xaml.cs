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
using System.Threading;
using System.Windows.Threading;
using WritersToolbox.Resources;

namespace WritersToolbox.views
{
    public partial class TomeDetails : PhoneApplicationPage
    {
        //Verbindung zwischen View und Model des Bandes.
        private TomeDetailsViewModel tome_VM;
        //Primäreschlüßel des bandes.
        private int tomeID;
        //Buttons zum speichern/abbrechen beim Bearbeiten eines Chapters
        private ApplicationBarIconButton save, cancel, delete, cancel_2, cancelAssign;
        
        //timer für den Slider der Infodetails
        private DispatcherTimer infoTimer;
        //interval des Slider Timers, hier 8sek
        private readonly TimeSpan infoInterval = new TimeSpan(0, 0, 8);
        // Nummer der im Slider angezeigten Info (Kapitel, Ereignisse, typobjekte, Seiten, Wörter)
        private int InfoStatus = 1;

        //Textbox Chapter
        TextBox b = new TextBox();
        //Chapter
        private datawrapper.Chapter chapter;
        private bool wasfocuslost;

        private bool isSectionOpened;

        private Image oldImage;
        private LongListMultiSelector oldLlms;
        private datawrapper.Chapter oldChapter;
        private bool isNewEventAndChapterButtonsRemoved;

        // false wenn doubleTap eintritt
        private bool singleTap;

        // zustand ob d ich Chapter im "doubleTap" -> bearbeitungsmodus befindet
        private bool doubleTap = false;

        private bool newChapterMode = false;

        private bool isChapterControlOpened;
        public bool chapterSelected = false;
        public bool eventSelected = false;


        public List<LongListMultiSelector> llmsEventListe = new List<LongListMultiSelector>();

        public TomeDetails()
        {
            InitializeComponent();
        }

        public void addCancelAssignButton()
        {
            cancelAssign = new ApplicationBarIconButton();
            cancelAssign.IconUri = new Uri("/icons/cancel.png", UriKind.Relative);
            cancelAssign.Text = AppResources.AppBarCancel;
            cancelAssign.Click += new EventHandler(cancelAssignment);
            ApplicationBar.Buttons.Add(cancelAssign);
        }

        private void cancelAssignment(object sender, EventArgs e)
        {
            ApplicationBar.Buttons.Remove(cancelAssign);
            PhoneApplicationService.Current.State["cancelAssignment"] = true;
            Title.Visibility = Visibility.Collapsed;
            NavigationService.GoBack();
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
            if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                searchImage.Visibility = Visibility.Collapsed;
                Title.Visibility = Visibility.Visible;
                addCancelAssignButton();
            }
            else
            {
                searchImage.Visibility = Visibility.Visible;
            }
            if (NavigationContext.QueryString.ContainsKey("tomeID"))
            {
                
                tomeID = int.Parse(NavigationContext.QueryString["tomeID"]);
                tome_VM = new TomeDetailsViewModel(tomeID);
                DataContext = tome_VM;
                //ist das hier richtig? funktionieren tuts
                informationSlide();
                int code = tome_VM.getInformation();
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
            infoTimer.Stop();
            infoTimer = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {

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
                detailInfotexteErstellen();

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
        /// Informationslider wird erstellt:
        /// Dispatchertimer wird erstellt
        /// Intervall wird festgelegt
        /// Aktivität beim Wechsel wird festgelegt (infoTimer_Tick)
        /// </summary>
        private void informationSlide() 
        {
            if (infoTimer == null) {
                infoTimer = new DispatcherTimer();
                infoTimer.Interval = infoInterval;
                infoTimer.Tick += new EventHandler(infoTimer_Tick);
                infoTimer.Start();
            }
            numberInforamtionText.Text = "" + tome_VM.getNumberOfChapters();
            inforamtionText.Text = "Kapitel";
        }


        
        
        /// <summary>
        /// Wenn der Timer tickt, werden die Infos durchlaufen
        /// SliderNumberOfInfo gibt an welche Info an der Reihe ist
        /// (Kapitel, Ereignisse, typobjekte, Seiten, Wörter)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void infoTimer_Tick(object sender, EventArgs e) {

            System.Diagnostics.Debug.WriteLine(infoTimer);

            int n;
            switch (InfoStatus)
                {
                    case 0:
                        numberInforamtionText.Text = "" + tome_VM.getNumberOfChapters();
                        inforamtionText.Text = "Kapitel";
                        break;
                    case 1:
                        n = tome_VM.getNumberOfEvents();
                        numberInforamtionText.Text = n.ToString();
                        if(n == 1){
                            inforamtionText.Text = "Event";
                        } else {
                            inforamtionText.Text = "Events";
                        }
                        break;
                    case 2:
                        n = tome_VM.getNumberOfTypeObjects();
                        numberInforamtionText.Text = n.ToString();
                        if(n == 1){
                            inforamtionText.Text = "Typ Objekt";
                        } else {
                            inforamtionText.Text = "Typ Objekte";
                        }
                        break;
                    case 3:
                        n = getNumberOfPages();
                        numberInforamtionText.Text = n.ToString();
                        if (n == 1)
                        {
                            inforamtionText.Text = "Seite";
                        }
                        else
                        {
                            inforamtionText.Text = "Seiten";
                        }
                        break;
                    case 4:
                        n = tome_VM.getNumberOfWords();
                        numberInforamtionText.Text = n.ToString();
                        if (n == 1)
        {
                            inforamtionText.Text = "Wort";
        }
                        else
        {
                            inforamtionText.Text = "Wörter";
                        }
                        break;
        }

            //Slider fängt von vorne an
            if (InfoStatus == 4)
        {
                InfoStatus = 0;
            }
            else 
            {
                InfoStatus++;    
            }

        }

        /// <summary>
        /// rechnet die Anzahl an Seiten aus. 
        /// </summary>
        /// <returns></returns>       
        private int getNumberOfPages() {
            int NumberSigns = tome_VM.getsNumberOfSignsFinaltext();

            //Tabelle Anzahl Wörter
            //            9       10      12      14
            //12*19       2600    2200    1500    1100
            //13,5*21,5   3500    2900    2000    1500
            //14,8*21     3700    3200    2100    1600
            //15,5*22     4000    3400    2400    1800
            //17*22       4500    3700    2600    2000

            int[,] wordTabell = new int[,]{{2600, 2200, 1500, 1100}, 
                                           {3500, 2900, 2000, 1500}, 
                                           {3700, 3200, 2100, 1600}, 
                                           {4000, 3400, 2400, 1800}, 
                                           {4500, 3700, 2600, 2000}};

            int formatIndex = formatList.SelectedIndex;
            int fontsizeIndex = fontSizeList.SelectedIndex;
            //System.Diagnostics.Debug.WriteLine("formatindex: " + formatIndex);

            


            return (int) (NumberSigns / wordTabell[formatIndex, fontsizeIndex] + 0.5);

           
        }


        /// <summary>
        /// Inhalt der Liste mit Formaten wird gewechselt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (formatList != null && fontSizeList != null)
            {
                detailInfotexteErstellen();
                // informationCode = 400 + formatList.SelectedIndex * 10 + fontSizeList.SelectedIndex;
            }
        }

        /// <summary>
        /// Inhalt der iste mit Schriftgrößen wird gewechselt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fontSizeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (formatList != null && fontSizeList != null)
            {
                detailInfotexteErstellen();
                //informationCode = 400 + formatList.SelectedIndex * 10 + fontSizeList.SelectedIndex;
            }
        }


        /// <summary>
        /// Die Infos für die Detailansicht werden erstellt
        /// Chapter, Events, Typobjekte, Seitenzahl, Wörter
        /// </summary>
        private void detailInfotexteErstellen() 
        {
            int n;
            NumberOfChapter_Zahl.Text = " " + tome_VM.getNumberOfChapters();
            NumberOfEvent_Zahl.Text = "" + tome_VM.getNumberOfEvents();
            NumberOfTypeObject_Zahl.Text = "" + tome_VM.getNumberOfTypeObjects();
            NumberOfPage_Zahl.Text = "" + getNumberOfPages();
            NumberOfWord_Zahl.Text = "" + tome_VM.getNumberOfWords();


            //Texte, Mehrzahl/Einzahl
            //Event/Events
            n = tome_VM.getNumberOfEvents();
            if (n == 1)
            {
                NumberOfEvent_Text.Text = "Event";
            }
            else
            {
                NumberOfEvent_Text.Text = "Events";
            }

            //Typobjekte
            n = tome_VM.getNumberOfTypeObjects();
            if (n == 1)
            {
                NumberOfTypeObject_Text.Text = "Typ Objekt";
            }
            else
            {
                NumberOfTypeObject_Text.Text = "Typ Objekte";
            }

            //Seiten
            n = getNumberOfPages();
            if (n == 1)
            {
                NumberOfPage_Text.Text = "Seite";
            }
            else
            {
                NumberOfPage_Text.Text = "Seiten";
            }

            //Wort/Wörter
            n = tome_VM.getNumberOfWords();
            if (n == 1)
            {
                NumberOfWord_Text.Text = "Wort";
            }
            else
            {
                NumberOfWord_Text.Text = "Wörter";
            }

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

                        if (oldImage != null && !oldImage.Equals(img))
                            oldImage.Source = new BitmapImage(new Uri("/icons/off.png", UriKind.RelativeOrAbsolute));
                        if (oldLlms != null && !oldLlms.Equals(llms))
                        {
                            oldLlms.IsSelectionEnabled = false;
                            oldLlms.EnforceIsSelectionEnabled = false;
                            oldLlms.Visibility = Visibility.Collapsed;
                    }
                            
                        oldChapter = parent.DataContext as datawrapper.Chapter;
                    }
                    else      //Ist die Ereignislist aufgeklappt,
                    {         //dann wird sie zusammenfeklappt.
                        img.Source = new BitmapImage(new Uri("/icons/off.png", UriKind.RelativeOrAbsolute));
                        llms.Visibility = Visibility.Collapsed;

                        if (oldImage != null && !oldImage.Equals(img))
                            oldImage.Source = new BitmapImage(new Uri("/icons/off.png", UriKind.RelativeOrAbsolute));
                        if (oldLlms != null && !oldLlms.Equals(llms))
                        {
                            oldLlms.IsSelectionEnabled = false;
                            oldLlms.EnforceIsSelectionEnabled = false;
                            oldLlms.Visibility = Visibility.Collapsed;
                    }
                }

                        oldImage = img;
                        oldLlms = llms;
            }
                if (isNewEventAndChapterButtonsRemoved)
                {
                    AddNewChapterAndEventButton();
                }
            }
            else
            {
                wasfocuslost = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {

            llstructure.IsSelectionEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chapter_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!newChapterMode)
            {
            llstructure.IsSelectionEnabled = true;

                llstructure.SelectedItems.Add(((Grid)sender).DataContext);
                if (!isSectionOpened)
                {
                    addSelectionApplicationBarButton();
                }
                int ID = (((LongListMultiSelector)sender).DataContext as datawrapper.Chapter).chapterID;
                if(tome_VM.isEventsInChapter(oldLlms, ID))
                {
                                    //Um alle events mitselektieren wenn die liste aufgeklappt ist.
                if (oldLlms != null && oldLlms.Visibility == Visibility.Visible)
                {
                    oldLlms.IsSelectionEnabled = true;
                    ObservableCollection<datawrapper.Event> l = new ObservableCollection<datawrapper.Event>(
                        (ObservableCollection<datawrapper.Event>)oldLlms.ItemsSource);
                    foreach (datawrapper.Event item in l)
                    {
                        if (!oldLlms.SelectedItems.Contains(item))
                        {
                            oldLlms.SelectedItems.Add(item);
                        }
                    }
                }
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void llmsEvent_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //l = sender as LongListMultiSelector;
            if (!newChapterMode)
            {
                datawrapper.Event _e = ((TextBlock)e.OriginalSource).DataContext as datawrapper.Event;

                //if (llstructure.SelectedItems.Count == 0)
                //{
                oldLlms.IsSelectionEnabled = true;
                oldLlms.SelectedItems.Add(_e);
                llstructure.EnforceIsSelectionEnabled = false;
                //}
                if (!isSectionOpened)
                {
                    addSelectionApplicationBarButton();
                }
        }

        }


        /// <summary>
        /// Tap auf ein Chapter (Textbox) 
        /// klappt Liste der Ereignisse auf/zu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChapterTextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (isNewEventAndChapterButtonsRemoved)
        {
                AddNewChapterAndEventButton();
            }
            singleTap = true;
            // verzögerung um "tap" von "doubleTap" zu unterscheiden
            
            Thread.Sleep(200);
            //await Task.Delay(200);

            //abfrage ob gesingle- oder gedoubletaped wurde, abfrage ob sich chapter in "bearbeitung" befindet
            if (singleTap && !doubleTap)
            {
                //geklickte Textbox(chapter) holen, entsprechende Eventliste aufklappen/zuklappen + IMG ändern
                b = sender as TextBox;
                Image img = ((b.Parent as Border).Parent as Grid).Children[0] as Image;
                
                if (!b.Text.Equals("Neues Kapitel") && !newChapterMode)
                {
                    Image_Tap(img, e);

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
        /// Die "Selection" applicationbar wird eingefügt
        /// </summary>
        private void addSelectionApplicationBarButton()
        {
            isSectionOpened = true;
            //Default Buttons von ApplicationBarButton löschen.
            if (isChapterControlOpened)
            removeAddChapterApplicationBarButton();

            //ApplicationBarButton mit ManagmentButtons erfüllen.

            delete = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));
            cancel_2 = new ApplicationBarIconButton(new Uri("/icons/cancel.png", UriKind.Relative));


            delete.Text = "Löschen";
            cancel_2.Text = "Abbrechen";
            

            //Events zu Buttons hinzufügen.


            //TODO
            cancel_2.Click += cancel_2Button_Click;
            delete.Click += deleteButton_Click;


            ApplicationBar.Buttons.Add(delete);
            ApplicationBar.Buttons.Add(cancel_2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_2Button_Click(object sender, EventArgs e)
        {
            llstructure.IsSelectionEnabled = false;
            
            if (oldLlms != null && oldLlms.Visibility == Visibility.Visible)
            {
                oldLlms.IsSelectionEnabled = false;
            }
            if (isSectionOpened)
            {
                removeSelectionApplicationBarButton();
            }

            AddNewChapterAndEventButton();
        }

        /// <summary>
        /// 
        /// </summary>
        private void removeAddChapterApplicationBarButton()
        {
            isChapterControlOpened = false;
            ApplicationBar.Buttons.Remove(save);
            ApplicationBar.Buttons.Remove(cancel);
        }

        /// <summary>
        /// 
        /// </summary>
        private void removeSelectionApplicationBarButton()
        {
            isSectionOpened = false;
            ApplicationBar.Buttons.Remove(delete);
            ApplicationBar.Buttons.Remove(cancel_2);
        }

        /// <summary>
        /// 
        /// </summary>
        private void addAddChapterApplicationBarButton()
        {
            isChapterControlOpened = true;
            //Default Buttons von ApplicationBarButton löschen.
            if (isSectionOpened)
            {
            removeSelectionApplicationBarButton();
            }

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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, EventArgs e) 
        {

            ObservableCollection<datawrapper.Chapter> ocl = new ObservableCollection<datawrapper.Chapter>();
            foreach (datawrapper.Chapter item in llstructure.SelectedItems)
            {
                ocl.Add(item);
            }
            tome_VM.deleteChapter(ocl);
                    
            if (oldLlms != null && oldLlms.Visibility == Visibility.Visible)
            {
                ObservableCollection<datawrapper.Event> ocl2 = new ObservableCollection<datawrapper.Event>();
                foreach (datawrapper.Event item in oldLlms.SelectedItems)
                {
                    ocl2.Add(item);
                }
                tome_VM.deleteEvent(ocl2);
            }

            AddNewChapterAndEventButton();

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
                if(isChapterControlOpened)
                removeAddChapterApplicationBarButton();
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
                    if(isChapterControlOpened)
                    removeAddChapterApplicationBarButton();
                    newChapterMode = false;
                    //b.IsReadOnly = true;
                    newChapterTextbox.IsEnabled = false;
                    newChapterTextbox.Visibility = Visibility.Collapsed;
                    newChapterRectangle.Visibility = Visibility.Collapsed;
                    newChapterTextTitle.Visibility = Visibility.Collapsed;
                    newChapterTextbox.Text = "";
                }
            } else{
                if (isChapterControlOpened)
                removeAddChapterApplicationBarButton();
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
                if (PhoneApplicationService.Current.State.ContainsKey("assignNote"))
                {
                    if (tome_VM.isExistNoteInEvent(_event.eventID, (PhoneApplicationService.Current.State["memoryNoteTitle"] as String)))
                    {
                        //Meldung
                        MessageBoxResult result = MessageBoxResult.OK;

                        result = MessageBox.Show(AppResources.MeldungVorhandeneNotizinEvent1 + " " + _event.title + " " 
                            + AppResources.MeldungVorhandeneNotizinEvent2 + System.Environment.NewLine + AppResources.MeldungVorhandeneNotizinEvent3,
                        AppResources.AppBarClose, MessageBoxButton.OKCancel);

                        if (result == MessageBoxResult.OK)
                        {
                            tome_VM.removeNote(_event.eventID, (PhoneApplicationService.Current.State["memoryNoteTitle"] as String));
                            PhoneApplicationService.Current.State.Remove("memoryNoteTitle");
                        }
                        else
                        {
                            PhoneApplicationService.Current.State.Remove("memoryNoteTitle");
                            return;
                        }

                    }
                    PhoneApplicationService.Current.State.Remove("memoryNoteTitle");
                    Title.Visibility = Visibility.Collapsed;
                    //Event ID zurückgeben
                    PhoneApplicationService.Current.State["eventID"] = _event.eventID;
                    NavigationService.GoBack();
                    return;
                }
                else
                {
                    
                    NavigationService.Navigate(new Uri("/views/EventDetail.xaml?chapterID=" + _event.chapter.chapterID + "&eventID=" + _event.eventID, UriKind.RelativeOrAbsolute));
                    var lastPage = NavigationService.BackStack.FirstOrDefault();
                    if (lastPage != null && lastPage.Source.ToString().Equals("/views/TomeDetails.xaml?tomeID=" + tomeID))
                    {
                        NavigationService.RemoveBackEntry();
                    }
                }
            }
            else
            {
                wasfocuslost = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadLlmsEvent(object sender, RoutedEventArgs e)
        {
            llmsEventListe.Add((LongListMultiSelector)sender);
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void llstructure_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.RemovedItems.Count > 0) || !((datawrapper.Chapter)e.AddedItems[0]).title.Equals("Neues Kapitel"))
            {
                if (!newChapterMode)
                {
                    if (!isNewEventAndChapterButtonsRemoved)
                    {
                        RemovceNewChapterAndEventButton();
                    }


                    if (!isSectionOpened)
                    {
                        addSelectionApplicationBarButton();
                    }


                    if (e.AddedItems.Count > 0)
                    {
                        if (!llstructure.SelectedItems.Contains((datawrapper.Chapter)e.AddedItems[0]))
                        {
                            llstructure.SelectedItems.Add(((datawrapper.Chapter)e.AddedItems[0]));
                        }

                        if (e.AddedItems[0].Equals(oldChapter))
                        {                //Um alle events mitselektieren wenn die liste aufgeklappt ist.
                            if (oldLlms != null && oldLlms.Visibility == Visibility.Visible)
                            {
                                oldLlms.IsSelectionEnabled = true;
                                ObservableCollection<datawrapper.Event> l = new ObservableCollection<datawrapper.Event>(
                                    (ObservableCollection<datawrapper.Event>)oldLlms.ItemsSource);
                                foreach (datawrapper.Event item in l)
                                {
                                    if (!oldLlms.SelectedItems.Contains(item))
                                    {
                                        oldLlms.SelectedItems.Add(item);
                                    }
                                }
                            }
                        }
                    }

                    if (e.RemovedItems.Count > 0)
                    {
                        if (llstructure.SelectedItems.Contains((datawrapper.Chapter)e.RemovedItems[0]))
                        {
                            llstructure.SelectedItems.Remove(((datawrapper.Chapter)e.RemovedItems[0]));
                        }
                        if (e.RemovedItems[0].Equals(oldChapter))
                        {
                            //Um alle events mitselektieren wenn die liste aufgeklappt ist.
                            if (oldLlms != null && oldLlms.Visibility == Visibility.Visible)
                            {
                                oldLlms.IsSelectionEnabled = false;
                                ObservableCollection<datawrapper.Event> l = new ObservableCollection<datawrapper.Event>(
                                    (ObservableCollection<datawrapper.Event>)oldLlms.ItemsSource);
                                foreach (datawrapper.Event item in l)
                                {
                                    if (oldLlms.SelectedItems.Contains(item))
                                    {
                                        oldLlms.SelectedItems.Remove(item);
                                    }
                                }
                            }
                        }
                    }

                    if (oldLlms == null)
                    {
                        if (llstructure.SelectedItems.Count == 0)
                        {
                            removeSelectionApplicationBarButton();
                            if (isNewEventAndChapterButtonsRemoved)
                            {
                                AddNewChapterAndEventButton();
                            }
                        }
                    }
                    else
                    {
                        if (llstructure.SelectedItems.Count == 0 && oldLlms.SelectedItems.Count == 0)
                        {
                            removeSelectionApplicationBarButton();
                            if (isNewEventAndChapterButtonsRemoved)
                            {
                                AddNewChapterAndEventButton();
                            }
                        }
                    }
                    
                }
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddNewChapterAndEventButton()
         {
            isNewEventAndChapterButtonsRemoved = false;
                this.tome_VM.addNewChapterEntry();
            if (oldLlms != null)
            {
                this.tome_VM.addNewEventEntry();
            }
            this.llstructure.ItemsSource = this.tome_VM.structur;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemovceNewChapterAndEventButton()
        {
            isNewEventAndChapterButtonsRemoved = true;
            this.tome_VM.removeNewChapterEntry();
            if (oldLlms != null)
            {
                this.tome_VM.removeNewEventEntry();
            }
            this.llstructure.ItemsSource = this.tome_VM.structur;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moveChapterDown(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image i = sender as Image;
            Grid g1 = i.Parent as Grid;
            datawrapper.Chapter c = (datawrapper.Chapter)g1.DataContext;
            if (!c.title.Equals("Neues Kapitel")) { 
                this.tome_VM.moveChapterDown((datawrapper.Chapter)g1.DataContext);
            }
            this.llstructure.ItemsSource = this.tome_VM.structur;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moveChapterUp(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image i = sender as Image;
            Grid g1 = i.Parent as Grid;
            datawrapper.Chapter c = (datawrapper.Chapter)g1.DataContext;
            if (!c.title.Equals("Neues Kapitel"))
            {
                this.tome_VM.moveChapterUp((datawrapper.Chapter)g1.DataContext);
            }

            this.llstructure.ItemsSource = this.tome_VM.structur;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moveEventDown(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image i = sender as Image;
            Grid g1 = i.Parent as Grid;
            datawrapper.Event ev = (datawrapper.Event)g1.DataContext;
            if (!ev.title.Equals("Neues Ereignis"))
            {
                this.tome_VM.moveEventDown(ev);
            }

            this.llstructure.ItemsSource = this.tome_VM.structur;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moveEventUp(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image i = sender as Image;
            Grid g1 = i.Parent as Grid;
            datawrapper.Event ev = (datawrapper.Event)g1.DataContext;
            if (!ev.title.Equals("Neues Ereignis"))
            {
                this.tome_VM.moveEventUp(ev);
            }

            this.llstructure.ItemsSource = this.tome_VM.structur;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void llmsEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((e.RemovedItems.Count > 0) || !((datawrapper.Event)e.AddedItems[0]).title.Equals("Ereignis hinzufügen"))
            {
                if (!newChapterMode)
                {
                    if (!isNewEventAndChapterButtonsRemoved)
                    {
                        RemovceNewChapterAndEventButton();
                    }

                    if (!isSectionOpened)
                    {
                        addSelectionApplicationBarButton();
                    }

                    //TODO selektieren

                    if (oldLlms.Visibility == Visibility.Visible)
                    {
                        if (e.AddedItems.Count > 0)
                        {
                            if (!oldLlms.SelectedItems.Contains((datawrapper.Event)e.AddedItems[0]))
                            {
                                oldLlms.SelectedItems.Add(((datawrapper.Event)e.AddedItems[0]));
                            }
                        }
                        if (e.RemovedItems.Count > 0)
                        {
                            if (oldLlms.SelectedItems.Contains((datawrapper.Event)e.RemovedItems[0]))
                            {
                                oldLlms.SelectedItems.Remove(((datawrapper.Event)e.RemovedItems[0]));
                            }
                        }
                    }

                    if (oldLlms == null)
                    {
                        if (llstructure.SelectedItems.Count == 0)
                        {
                            removeSelectionApplicationBarButton();
                            if (isNewEventAndChapterButtonsRemoved)
                            {
                                AddNewChapterAndEventButton();
                            }
                        }
                    }
                    else
                    {
                        if (llstructure.SelectedItems.Count == 0 && oldLlms.SelectedItems.Count == 0)
                        {
                            removeSelectionApplicationBarButton();
                            if (isNewEventAndChapterButtonsRemoved)
                            {
                                AddNewChapterAndEventButton();
                            }
                        }
                    }

                }
            }        
        }
        private void Image_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Search.xaml", UriKind.RelativeOrAbsolute));
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush _s = new SolidColorBrush(Colors.Transparent);
            this.bookTitle.Background = _s;
        }

        private void bookTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            this.tome_VM.changeTitle(bookTitle.Text);
        }
        

    }
}
