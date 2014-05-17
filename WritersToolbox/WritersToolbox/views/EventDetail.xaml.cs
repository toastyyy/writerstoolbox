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
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using WritersToolbox.Resources;
using System.Collections;
using System.Windows.Media;

namespace WritersToolbox.views
{
    public partial class EventDetail : PhoneApplicationPage
    {
        //Viewmodel für Eventdetails
        private EventDetailViewModel edvm = null;

        //Flag, ob ein neues Event erstellt wird
        private bool newEvent = false;
        private bool unattach = false;
        private int chapterID = 0;
        public EventDetail()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Beim Wegnavigieren werden gerade eingegebene Daten gesichert.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (this.edvm.Event != null) { 
                PhoneApplicationService.Current.State["RestoreData"] =
                new datawrapper.Event()
                {
                    finaltext = textBoxFinalText.Text,
                    eventID = this.edvm.Event.eventID
                };
            }




        }


        /// <summary>
        /// Beim Hernavigieren wird folgendes Überprüft: um welches Event es sich handelt, wenn ein exisiterendes Event, dann welchem Chapter zugehörig,
        /// ob dem Event gerade ein Typobjekt angehängt wurde,
        /// oder ob die App gerade erwacht ist und die gespeicherten Daten wiederhergestellt werden sollen.
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //welches Event
            if (NavigationContext.QueryString.ContainsKey("eventID"))
            {
                int eID = int.Parse(NavigationContext.QueryString["eventID"]);
                //neues Event
                if (eID == 0)
                {
                    //welchem Chapter angehörig
                    this.chapterID = int.Parse(NavigationContext.QueryString["chapterID"]);
                    newEvent = true;
                    this.edvm = new EventDetailViewModel(eID);
                }
                else
                {
                    this.edvm = new EventDetailViewModel(eID);

                    this.edvm.LoadData();
                    this.DataContext = this.edvm.Event;
                    try
                    {
                        this.tFinalText.Xaml = this.parseRichTextFormat(this.edvm.Event.finaltext);
                    }
                    catch (Exception ex)
                    {
                        this.tFinalText.Xaml = this.parsePlainText(this.edvm.Event.finaltext);
                    }
                }
            }

            //Event wurde gerade Typobjekt angehängt
            if (PhoneApplicationService.Current.State.ContainsKey("attachEvent"))
            {
                if (PhoneApplicationService.Current.State.ContainsKey("typeObjectID"))
                {
                    int toID = (int)PhoneApplicationService.Current.State["typeObjectID"];
                    PhoneApplicationService.Current.State.Remove("typeObjectID");
                    PhoneApplicationService.Current.State.Remove("attachEvent");
                    this.edvm.attachTypeObject(toID, this.edvm.Event.eventID);
                    this.DataContext = null;
                    this.DataContext = this.edvm.Event;
                }
                else
                {
                    PhoneApplicationService.Current.State.Remove("attachEvent");
                }

            }
            //App erwacht gerade, gesichert Daten werden wiederhergestellt
            if (PhoneApplicationService.Current.State.ContainsKey("tombstoned"))
            {
                if (PhoneApplicationService.Current.State.ContainsKey("RestoreData"))
                {

                    datawrapper.Event ev = (datawrapper.Event)PhoneApplicationService.Current.State["RestoreData"];
                    this.edvm = new EventDetailViewModel(ev.eventID);
                    this.edvm.LoadData();
                    this.DataContext = this.edvm.Event;
                    try
                    {
                        this.tFinalText.Xaml = this.parseRichTextFormat(ev.finaltext);
                        textBoxFinalText.Text = ev.finaltext;
                    }
                    catch (Exception ex)
                    {
                        this.tFinalText.Xaml = this.parsePlainText(ev.finaltext);
                        textBoxFinalText.Text = ev.finaltext;
                    }
                    newEvent = false;
                    PhoneApplicationService.Current.State.Remove("RestoreData");
                }
                PhoneApplicationService.Current.State.Remove("tombstoned");
            }
        }


        /// <summary>
        /// Das Pivotitem wird geändert und die Appbar angepasst.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!newEvent)
            {
                Pivot p = sender as Pivot;
                if (p.SelectedIndex == 0)
                {
                    ApplicationBar.Buttons.Clear();
                }
                else if (p.SelectedIndex == 1)
                {
                    if (this.edvm.Event.notes.Count > 0)
                    {
                        /*ApplicationBar.Buttons.Clear();
                        ApplicationBarIconButton unattach = new ApplicationBarIconButton(new Uri("/icons/edit.png", UriKind.Relative));
                        unattach.Text = AppResources.AppBarUnattached;
                        unattach.Click += unattachNotes;
                        ApplicationBar.Buttons.Add(unattach);
                        ApplicationBarIconButton delete = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));
                        delete.Text = AppResources.AppBarDelete;
                        delete.Click += deleteNotes;
                        ApplicationBar.Buttons.Add(delete);*/
                    }
                }
                else if (p.SelectedIndex == 2)
                {
                    if (this.edvm.Event.typeObjects.Count > 1)
                    {
                        /*ApplicationBar.Buttons.Clear();
                        ApplicationBarIconButton unattach = new ApplicationBarIconButton(new Uri("/icons/edit.png", UriKind.Relative));
                        unattach.Text = AppResources.AppBarUnattached;
                        unattach.Click += unattachTypeObjects;
                        ApplicationBar.Buttons.Add(unattach);*/
                    }
                }
            }

        }

        /// <summary>
        /// Finaltext soll bearbeitet werden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tFinalText_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!newEvent)
            {
                this.editFinaltextGrid.Visibility = Visibility.Visible;
                this.textBoxFinalText.Visibility = Visibility.Visible;

                //workaround fuer tastatur
                this.textBoxFinalText.Focus();
                this.WorkaroundButton.Focus();
                this.textBoxFinalText.Focus();
                this.createTextEditApplicationBar();
            }

        }

        /// <summary>
        /// Methode zum Einfügen des Bold-Tags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void insertBold(object sender, System.EventArgs e)
        {
            int selStart = this.textBoxFinalText.SelectionStart;
            int selLength = this.textBoxFinalText.SelectionLength;
            String text = this.textBoxFinalText.Text;
            if (selLength == 0)
            {
                String newText = text.Substring(0, selStart)
                                + "<b></b>"
                                + text.Substring(selStart);
                this.textBoxFinalText.Text = newText;
                this.textBoxFinalText.SelectionStart = selStart + 3;
            }
            else
            {
                String newText = text.Substring(0, selStart)
                                + "<b>" + text.Substring(selStart, selLength) + "</b>"
                                + text.Substring(selStart + selLength);
                this.textBoxFinalText.Text = newText;
                this.textBoxFinalText.SelectionStart = selStart + 3;
                this.textBoxFinalText.SelectionLength = selLength;
            }
        }

        /// <summary>
        /// Methode zum Einfügen des Italic-Tags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void insertItalic(object sender, System.EventArgs e)
        {
            int selStart = this.textBoxFinalText.SelectionStart;
            int selLength = this.textBoxFinalText.SelectionLength;
            String text = this.textBoxFinalText.Text;
            if (selLength == 0)
            {
                String newText = text.Substring(0, selStart)
                                + "<i></i>"
                                + text.Substring(selStart);
                this.textBoxFinalText.Text = newText;
                this.textBoxFinalText.SelectionStart = selStart + 3;
            }
            else
            {
                String newText = text.Substring(0, selStart)
                                + "<i>" + text.Substring(selStart, selLength) + "</i>"
                                + text.Substring(selStart + selLength);
                this.textBoxFinalText.Text = newText;
                this.textBoxFinalText.SelectionStart = selStart + 3;
                this.textBoxFinalText.SelectionLength = selLength;
            }
        }

        /// <summary>
        /// Methode zum Einfügen des Underline-Tags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void insertUnderline(object sender, System.EventArgs e)
        {
            int selStart = this.textBoxFinalText.SelectionStart;
            int selLength = this.textBoxFinalText.SelectionLength;
            String text = this.textBoxFinalText.Text;
            if (selLength == 0)
            {
                String newText = text.Substring(0, selStart)
                                + "<u></u>"
                                + text.Substring(selStart);
                this.textBoxFinalText.Text = newText;
                this.textBoxFinalText.SelectionStart = selStart + 3;
            }
            else
            {
                String newText = text.Substring(0, selStart)
                                + "<u>" + text.Substring(selStart, selLength) + "</u>"
                                + text.Substring(selStart + selLength);
                this.textBoxFinalText.Text = newText;
                this.textBoxFinalText.SelectionStart = selStart + 3;
                this.textBoxFinalText.SelectionLength = selLength;
            }
        }

        /// <summary>
        /// Die Appbar zum bearbeiten des Finaltextes mit allen Buttons wird geladen
        /// </summary>
        private void createTextEditApplicationBar()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBarIconButton boldButton = new ApplicationBarIconButton(new Uri("/icons/bold.png", UriKind.RelativeOrAbsolute));
            boldButton.Text = AppResources.AppBarBold;
            boldButton.Click += insertBold;
            ApplicationBar.Buttons.Add(boldButton);
            ApplicationBarIconButton italicButton = new ApplicationBarIconButton(new Uri("/icons/italic.png", UriKind.RelativeOrAbsolute));
            italicButton.Text = AppResources.AppBarItalic;
            italicButton.Click += insertItalic;
            ApplicationBar.Buttons.Add(italicButton);
            ApplicationBarIconButton underlineButton = new ApplicationBarIconButton(new Uri("/icons/underline.png", UriKind.RelativeOrAbsolute));
            underlineButton.Text = AppResources.AppBarUnderline;
            underlineButton.Click += insertUnderline;
            ApplicationBar.Buttons.Add(underlineButton);
            ApplicationBarIconButton saveButton = new ApplicationBarIconButton(new Uri("/icons/save.png", UriKind.RelativeOrAbsolute));
            saveButton.Text = AppResources.AppBarSave;
            saveButton.Click += saveFinaltextChanges;
            ApplicationBar.Buttons.Add(saveButton);
        }

        /// <summary>
        /// Die Standard Appbar wird wiederhergestellt.
        /// </summary>
        private void restoreStandardApplicationBar()
        {
            ApplicationBar.Buttons.Clear();
            /*ApplicationBarIconButton saveButton = new ApplicationBarIconButton(new Uri("/icons/save.png", UriKind.RelativeOrAbsolute));
            saveButton.Text = AppResources.AppBarSave;
            ApplicationBar.Buttons.Add(saveButton);
            ApplicationBarIconButton cancelButton = new ApplicationBarIconButton(new Uri("/icons/cancel.png", UriKind.RelativeOrAbsolute));
            cancelButton.Text = AppResources.AppBarCancel;
            ApplicationBar.Buttons.Add(cancelButton);*/
        }


        /// <summary>
        /// Die Änderungen des Finaltextes werden gespeichert. Der Text der Textbox wird in Richtext geparst.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFinaltextChanges(object sender, System.EventArgs e)
        {
            this.restoreStandardApplicationBar();
            this.edvm.updateFinaltext(this.textBoxFinalText.Text);
            this.DataContext = null;
            this.DataContext = this.edvm.Event;
            this.editFinaltextGrid.Visibility = Visibility.Collapsed;
            this.textBoxFinalText.Visibility = Visibility.Collapsed;
            try
            {
                this.tFinalText.Xaml = this.parseRichTextFormat(this.edvm.Event.finaltext);
            }
            catch (Exception ex)
            {
                this.tFinalText.Xaml = this.parsePlainText(this.edvm.Event.finaltext);
            }
        }


        /// <summary>
        /// Text mit Tags wird in Richtext geparst um eine Darstellung in einer Richtext-Box zu ermöglichen.
        /// Damit sind einfache Formatierungen möglich.
        /// </summary>
        /// <param name="plain"></param>
        /// <returns></returns>
        private String parseRichTextFormat(String plain)
        {

            String retXaml = "";
            String text = plain;
            text = text.Replace("<b>", "<Bold>").Replace("</b>", "</Bold>");
            text = text.Replace("<i>", "<Italic>").Replace("</i>", "</Italic>");
            text = text.Replace("<u>", "<Underline>").Replace("</u>", "</Underline>");

            text = text.Replace(@"\n", "</Paragraph><Paragraph>");
            retXaml =
                @"<Section xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">" +
                @"<Paragraph TextAlignment=""Justify"">" + text + "</Paragraph>" +
                "</Section>";
            return retXaml;
        }

        /// <summary>
        /// Richttext kann nicht korrekt geparst werden, weil Tags fehelrhaft sind.
        /// </summary>
        /// <param name="xaml"></param>
        /// <returns></returns>
        private String parsePlainText(String xaml)
        {
            String ret = xaml.Replace("<", "&lt;").Replace(">", "&gt;");
            ret = ret.Replace("\n", "</Paragraph><Paragraph>");
            return @"<Section xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">" +
                @"<Paragraph TextAlignment=""Justify"">" + ret + "</Paragraph>" +
                "</Section>";
        }


        /// <summary>
        /// Ein Typobjekt wurde ausgewählt und die Möglichkeit, ein neues zuzufügen verschwindet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TypeObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!unattach)
            {
                if (TypeObjectList.SelectedItems.Count > 0)
                {
                    this.edvm.removeAddTypeObject();
                    this.addAppBarTypeObjects();
                }
                else
                {
                    this.edvm.addAddTypeObject();
                    this.removeAppBarTypeObjects();
                }
            }
            this.selectAllCheckBox2.Unchecked -= selectAllCheckBox_Unchecked;
            this.selectAllCheckBox2.Checked -= selectAllCheckBox_Checked;
            this.selectAllCheckBox2.IsChecked = TypeObjectList.SelectedItems.Count == this.edvm.Event.typeObjects.Count;
            this.TypeObjectList.EnforceIsSelectionEnabled = TypeObjectList.SelectedItems.Count > 0;
            this.eventPivot.IsLocked = TypeObjectList.SelectedItems.Count > 0;
            this.selectAllCheckBox2.Unchecked += selectAllCheckBox_Unchecked;
            this.selectAllCheckBox2.Checked += selectAllCheckBox_Checked;


        }

        /// <summary>
        /// Wurde auf ein Typobjekt geklickt, wird zu diesem navigiert oder es wird ein neues angehängt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectTypeObject(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid g = sender as Grid;
            if (g == null)
                return;
            datawrapper.TypeObject t = g.DataContext as datawrapper.TypeObject;
            if (t == null)
                return;
            if (t.typeObjectID != -1)
            {
                NavigationService.Navigate(new Uri("/views/TypeObjectDetails2.xaml?typeObjectID=" + t.typeObjectID.ToString(), UriKind.Relative));
            }
            else if (t.typeObjectID == -1)
            {
                PhoneApplicationService.Current.State["attachEvent"] = true;
                NavigationService.Navigate(new Uri("/views/Types.xaml", UriKind.Relative));
            }
        }

        /// <summary>
        /// Wird eine Notiz angeklickt, wird zu ihr navigiert zum bearbeiten.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectNote(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid g = sender as Grid;
            if (g == null)
                return;
            datawrapper.MemoryNote n = g.DataContext as datawrapper.MemoryNote;
            if (n == null)
                return;
            PhoneApplicationService.Current.State["memoryNoteID"] = n.memoryNoteID.ToString();
            PhoneApplicationService.Current.State["eventID"] = n.fk_eventID;
            PhoneApplicationService.Current.State["edit"] = "true";
            PhoneApplicationService.Current.State["assignedNote"] = "true";
            NavigationService.Navigate(new Uri("/views/AddNote.xaml", UriKind.Relative));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //NavigationService.GoBack();
            if (this.edvm.Event != null)
            {
                NavigationService.Navigate(new Uri("/views/TomeDetails.xaml?tomeID=" + this.edvm.Event.chapter.tome.tomeID, UriKind.RelativeOrAbsolute));
            }
            else
            {
                int tomeId = this.edvm.getTomeIDForChapter(this.chapterID);
                NavigationService.Navigate(new Uri("/views/TomeDetails.xaml?tomeID=" + tomeId, UriKind.RelativeOrAbsolute));
            }

            var lastPage = NavigationService.BackStack.FirstOrDefault();
            if (lastPage != null && lastPage.Source.ToString().Substring(0, 23).Equals("/views/EventDetail.xaml"))
            {
                NavigationService.RemoveBackEntry();
            }

        }

        /// <summary>
        /// Beim selektiern der Checkbox "Select All" werden alle Typobjekte markiert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.TypeObjectList.SelectionChanged -= TypeObjects_SelectionChanged;
            this.edvm.removeAddTypeObject();

            LongListMultiSelector l = null;
            CheckBox c = sender as CheckBox;
            if (c.Name.Equals("selectAllCheckBox1"))
            {
                l = NoteList;
            }
            else if (c.Name.Equals("selectAllCheckBox2"))
            {
                this.eventPivot.IsLocked = true;
                l = TypeObjectList;
            }

            if (l != null)
            {
                l.EnforceIsSelectionEnabled = true;
                IEnumerator enumerator = l.ItemsSource.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    l.SelectedItems.Add(enumerator.Current);
                }
            }
            this.TypeObjectList.SelectionChanged += TypeObjects_SelectionChanged;

        }


        /// <summary>
        /// Beim Deselektieren der Checkbox "Select All" werden alle Typobjekte deselektiert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.TypeObjectList.SelectionChanged -= TypeObjects_SelectionChanged;
            this.edvm.addAddTypeObject();
            //this.DataContext = null;
            //this.DataContext = this.edvm.Event;
            LongListMultiSelector l = null;
            CheckBox c = sender as CheckBox;
            if (c.Name.Equals("selectAllCheckBox1"))
            {
                l = NoteList;
            }
            else if (c.Name.Equals("selectAllCheckBox2"))
            {
                this.eventPivot.IsLocked = false;
                l = TypeObjectList;
            }
            if (l != null)
            {
                l.EnforceIsSelectionEnabled = false;
            }
            this.TypeObjectList.SelectionChanged += TypeObjects_SelectionChanged;
        }

        /// <summary>
        /// Die Zuordnung zu einem Typobjekt wird aufgelöst.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void unattachTypeObjects(object sender, EventArgs e)
        {
            foreach (datawrapper.TypeObject to in TypeObjectList.SelectedItems)
            {
                this.edvm.unassignTypeObject(to.typeObjectID);
            }
            unattach = true;
            this.DataContext = null;
            unattach = false;
            this.DataContext = this.edvm.Event;
            this.removeAppBarTypeObjects();
        }

        /// <summary>
        /// Die Zuordnung einer Notiz wird aufgelöst.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void unattachNotes(object sender, EventArgs e)
        {
            foreach (datawrapper.MemoryNote n in NoteList.SelectedItems)
            {
                this.edvm.unassignNote(n.memoryNoteID);
            }
            this.DataContext = null;
            this.DataContext = this.edvm.Event;
            this.removeAppBarNotes();
        }

        /// <summary>
        /// Eine Notiz wird gelöscht.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteNotes(object sender, EventArgs e)
        {
            foreach (datawrapper.MemoryNote n in NoteList.SelectedItems)
            {
                this.edvm.deleteNote(n.memoryNoteID);
            }
            this.DataContext = null;
            this.DataContext = this.edvm.Event;
        }

        private void TypeObjectList_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListMultiSelector llms = (LongListMultiSelector)sender;
            FrameworkElement c = (FrameworkElement)e.OriginalSource;
            while (!llms.Parent.GetType().IsAssignableFrom((new Grid()).GetType())) { };
            Grid g = (Grid)c.Parent;
            llms.SelectedItems.Add(((datawrapper.TypeObject)g.DataContext));
        }

        private void NoteList_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListMultiSelector llms = (LongListMultiSelector)sender;
            FrameworkElement c = (FrameworkElement)e.OriginalSource;
            while (!llms.Parent.GetType().IsAssignableFrom((new Grid()).GetType())) { };
            Grid g = (Grid)c.Parent;
            llms.SelectedItems.Add(((datawrapper.MemoryNote)g.DataContext));
        }


        /// <summary>
        /// Nach dem Laden der Seite wird überprüft, ob ein neues Event erstellt wird oder nicht und die Appbar angepasst.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (newEvent)
            {
                ApplicationBar.Buttons.Clear();
                ApplicationBarIconButton save = new ApplicationBarIconButton(new Uri("/icons/save.png", UriKind.Relative));
                save.Text = AppResources.AppBarSave;
                save.Click += saveNewEvent;
                ApplicationBar.Buttons.Add(save);
                ApplicationBarIconButton cancel = new ApplicationBarIconButton(new Uri("/icons/cancel.png", UriKind.Relative));
                cancel.Text = AppResources.AppBarCancel;
                cancel.Click += cancelNewEvent;
                ApplicationBar.Buttons.Add(cancel);
                newEventTextbox.Focus();
                eventPivot.IsLocked = true;
                newEventTitle.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Das erstellen eines neuen Events wird abgebrochen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelNewEvent(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }


        /// <summary>
        /// Ein neues event wird gespeichert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveNewEvent(object sender, EventArgs e)
        {
            if (newEventTextbox.Text.Equals("")) {
                MessageBox.Show("Sie müssen einen Titel für das Ereignis angeben");
                return;
            }
            this.newEventTextbox.LostFocus -= newEventTextbox_LostFocus;
            this.edvm.newEvent(newEventTextbox.Text, this.chapterID);
            this.edvm.LoadData();
            this.DataContext = null;
            this.DataContext = this.edvm.Event;
            eventPivot.IsLocked = false;
            newEventTitle.Visibility = Visibility.Collapsed;
            ApplicationBar.Buttons.Clear();
            newEvent = false;
            NavigationContext.QueryString.Clear();
        }


        /// <summary>
        /// Die Searchview wird geladen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Search.xaml", UriKind.RelativeOrAbsolute));
        }

        private void addAppBarTypeObjects()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBarIconButton unattach = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));
            unattach.Text = AppResources.AppBarUnattached;
            unattach.Click += unattachTypeObjects;
            ApplicationBar.Buttons.Add(unattach);
        }

        private void removeAppBarTypeObjects()
        {
            ApplicationBar.Buttons.Clear();
        }

        private void addAppBarNotes()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBarIconButton unattach = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));
            unattach.Text = AppResources.AppBarUnattached;
            unattach.Click += unattachNotes;
            ApplicationBar.Buttons.Add(unattach);
        }

        private void removeAppBarNotes() 
        {
            ApplicationBar.Buttons.Clear();
        }

        private void NoteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NoteList.SelectedItems.Count > 0)
            {
                this.addAppBarNotes();
            }
            else
            {
                this.removeAppBarNotes();
            }
        }

        private void tTitle_GotFocus(object sender, RoutedEventArgs e)
        {
            SolidColorBrush _s = new SolidColorBrush(Colors.Transparent);
            this.tTitle.Background = _s;
        }

        private void tTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!tTitle.Text.Equals(""))
            {
                this.edvm.updateTitle(tTitle.Text);
            }
        }

        private void newEventTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.newEventTextbox.Focus();
        }
    }
}