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

namespace WritersToolbox.views
{
    public partial class EventDetail : PhoneApplicationPage
    {
        private EventDetailViewModel edvm = null;
        private bool newEvent = false;
        public EventDetail()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("eventID"))
            {
                int eID = int.Parse(NavigationContext.QueryString["eventID"]);
                if (eID == 0)
                {
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
        }

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
                        ApplicationBar.Buttons.Clear();
                        ApplicationBarIconButton unattach = new ApplicationBarIconButton(new Uri("/icons/edit.png", UriKind.Relative));
                        unattach.Text = AppResources.AppBarUnattached;
                        unattach.Click += unattachNotes;
                        ApplicationBar.Buttons.Add(unattach);
                        ApplicationBarIconButton delete = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));
                        delete.Text = AppResources.AppBarDelete;
                        delete.Click += deleteNotes;
                        ApplicationBar.Buttons.Add(delete);
                    }
                }
                else if (p.SelectedIndex == 2)
                {
                    if (this.edvm.Event.typeObjects.Count > 1)
                    {
                        ApplicationBar.Buttons.Clear();
                        ApplicationBarIconButton unattach = new ApplicationBarIconButton(new Uri("/icons/edit.png", UriKind.Relative));
                        unattach.Text = AppResources.AppBarUnattached;
                        unattach.Click += unattachTypeObjects;
                        ApplicationBar.Buttons.Add(unattach);
                    }
                }
            }
            
        }

        
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
            else {
                String newText = text.Substring(0, selStart)
                                + "<b>" + text.Substring(selStart, selLength) + "</b>"
                                + text.Substring(selStart + selLength);
                this.textBoxFinalText.Text = newText;
                this.textBoxFinalText.SelectionStart = selStart + 3;
                this.textBoxFinalText.SelectionLength = selLength; 
            }
        }

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

        private void insertUnderline(object sender, System.EventArgs e) {
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

        private void createTextEditApplicationBar() {
            ApplicationBar.Buttons.Clear();
            ApplicationBarIconButton boldButton = new ApplicationBarIconButton(new Uri("/icons/bold.png", UriKind.RelativeOrAbsolute));
            boldButton.Text = "fett";
            boldButton.Click += insertBold;
            ApplicationBar.Buttons.Add(boldButton);
            ApplicationBarIconButton italicButton = new ApplicationBarIconButton(new Uri("/icons/italic.png", UriKind.RelativeOrAbsolute));
            italicButton.Text = "kursiv";
            italicButton.Click += insertItalic;
            ApplicationBar.Buttons.Add(italicButton);
            ApplicationBarIconButton underlineButton = new ApplicationBarIconButton(new Uri("/icons/underline.png", UriKind.RelativeOrAbsolute));
            underlineButton.Text = "unterstrichen";
            underlineButton.Click += insertUnderline;
            ApplicationBar.Buttons.Add(underlineButton);
            ApplicationBarIconButton saveButton = new ApplicationBarIconButton(new Uri("/icons/save.png", UriKind.RelativeOrAbsolute));
            saveButton.Text = "speichern";
            saveButton.Click += saveFinaltextChanges;
            ApplicationBar.Buttons.Add(saveButton);
        }

        private void restoreStandardApplicationBar() {
            ApplicationBar.Buttons.Clear();
            ApplicationBarIconButton saveButton = new ApplicationBarIconButton(new Uri("/icons/save.png", UriKind.RelativeOrAbsolute));
            saveButton.Text = "speichern";
            ApplicationBar.Buttons.Add(saveButton);
            ApplicationBarIconButton cancelButton = new ApplicationBarIconButton(new Uri("/icons/cancel.png", UriKind.RelativeOrAbsolute));
            cancelButton.Text = "abbrechen";
            ApplicationBar.Buttons.Add(cancelButton);
        }

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

        private String parseRichTextFormat(String plain) {

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

        private String parsePlainText(String xaml) {
            String ret = xaml.Replace("<", "&lt;").Replace(">", "&gt;");
            ret = ret.Replace("\n", "</Paragraph><Paragraph>");
            return @"<Section xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">" +
                @"<Paragraph TextAlignment=""Justify"">" + ret + "</Paragraph>" +
                "</Section>";
        }

        private void Notes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TypeObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeObjectList.SelectedItems.Count > 0)
            {
                this.edvm.removeAddTypeObject();
            }
            else {
                this.edvm.addAddTypeObject();
            }

            this.selectAllCheckBox2.Unchecked -= selectAllCheckBox_Unchecked;
            this.selectAllCheckBox2.Checked -= selectAllCheckBox_Checked;
            this.selectAllCheckBox2.IsChecked = TypeObjectList.SelectedItems.Count == TypeObjectList.ItemsSource.Count;
            this.TypeObjectList.EnforceIsSelectionEnabled = TypeObjectList.SelectedItems.Count > 0;
            this.eventPivot.IsLocked = TypeObjectList.SelectedItems.Count > 0;
            this.selectAllCheckBox2.Unchecked += selectAllCheckBox_Unchecked;
            this.selectAllCheckBox2.Checked += selectAllCheckBox_Checked;
        }

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
            

        private void SelectNote(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid g = sender as Grid;
            if (g == null)
                return;
            datawrapper.MemoryNote n = g.DataContext as datawrapper.MemoryNote;
            if (n == null)
                return;
            PhoneApplicationService.Current.State["memoryNoteID"] = n.memoryNoteID.ToString();
            PhoneApplicationService.Current.State["typeObjectID"] = n.fk_typeObjectID;
            PhoneApplicationService.Current.State["edit"] = "true";
            PhoneApplicationService.Current.State["assignedNote"] = "true";
            NavigationService.Navigate(new Uri("/views/AddNote.xaml", UriKind.Relative));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //NavigationService.GoBack();

            NavigationService.Navigate(new Uri("/views/TomeDetails.xaml?tomeID=" + this.edvm.Event.chapter.tome.tomeID , UriKind.RelativeOrAbsolute));

            var lastPage = NavigationService.BackStack.FirstOrDefault();
            if (lastPage != null && lastPage.Source.ToString().Substring(0,23).Equals("/views/EventDetail.xaml"))
            {
                NavigationService.RemoveBackEntry();
            }

        }

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

        private void unattachTypeObjects(object sender, EventArgs e)
        {
            foreach (datawrapper.TypeObject to in TypeObjectList.SelectedItems)
            {
                this.edvm.unassignTypeObject(to.typeObjectID);
            }
            this.DataContext = null;
            this.DataContext = this.edvm.Event;
        }

        private void unattachNotes(object sender, EventArgs e)
        {
            foreach (datawrapper.MemoryNote n in NoteList.SelectedItems)
            {
                this.edvm.unassignNote(n.memoryNoteID);
            }
            this.DataContext = null;
            this.DataContext = this.edvm.Event;
        }

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

        private void cancelNewEvent(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void saveNewEvent(object sender, EventArgs e)
        {
            this.edvm.newEvent(newEventTextbox.Text, 1);
            this.edvm.LoadData();
            this.DataContext = null;
            this.DataContext = this.edvm.Event;
            eventPivot.IsLocked = false;
            newEventTitle.Visibility = Visibility.Collapsed;
            ApplicationBar.Buttons.Clear();
            newEvent = false;
            NavigationContext.QueryString.Clear();
        }

    }
}