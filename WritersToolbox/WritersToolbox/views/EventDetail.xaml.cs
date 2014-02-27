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

namespace WritersToolbox.views
{
    public partial class EventDetail : PhoneApplicationPage
    {
        private EventDetailViewModel edvm = null;
        private PhotoChooserTask photoChooserTask;
        private int photochooserSelStart = -1;
        private int photochooserSelLength = 0;
        public EventDetail()
        {
            InitializeComponent();
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (PhoneApplicationService.Current.State.ContainsKey("preventUpdate") &&
                (Boolean)PhoneApplicationService.Current.State["preventUpdate"] == true)
            {
                PhoneApplicationService.Current.State["preventUpdate"] = false;
                return;
            }

            if (NavigationContext.QueryString.ContainsKey("eventID"))
            {
                int eID = int.Parse(NavigationContext.QueryString["eventID"]);
                this.edvm = new EventDetailViewModel(eID);
                this.edvm.LoadData();
                this.DataContext = this.edvm.Event;
                try
                {
                    this.tFinalText.Xaml = this.parseRichTextFormat(this.edvm.Event.finaltext);
                }
                catch (Exception ex) {
                    this.tFinalText.Xaml = this.parsePlainText(this.edvm.Event.finaltext);
                }
                
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void tFinalText_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.editFinaltextGrid.Visibility = Visibility.Visible;
            this.textBoxFinalText.Visibility = Visibility.Visible;

            //workaround fuer tastatur
            this.textBoxFinalText.Focus();
            this.WorkaroundButton.Focus();
            this.textBoxFinalText.Focus();
            this.createTextEditApplicationBar();
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

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                String curText = this.textBoxFinalText.Text;
                this.textBoxFinalText.Text =
                    curText.Substring(0, this.photochooserSelStart)
                    + "<p>" + e.OriginalFileName.Replace(@"\", "/") + "</p>" +
                    curText.Substring(this.photochooserSelStart + this.photochooserSelLength);
            }
        }

        private String parseRichTextFormat(String plain) {

            String retXaml = "";
            String text = plain;
            text = Regex.Replace(text, @"<b>(\w+)</b>", "<Bold>$1</Bold>");
            text = Regex.Replace(text, @"<i>(\w+)</i>", "<Italic>$1</Italic>");
            text = Regex.Replace(text, @"<u>(\w+)</u>", "<Underline>$1</Underline>");

            retXaml = 
                @"<Section xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">" +
                "<Paragraph>" + text + "</Paragraph>" + 
                "</Section>";
            return retXaml;
        }

        private String parsePlainText(String xaml) {
            String ret = xaml.Replace("<", "&lt;").Replace(">", "&gt;");
            return @"<Section xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">" +
                "<Paragraph>" + ret + "</Paragraph>" +
                "</Section>";
        }

        private void Notes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TypeObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SelectTypeObject(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid g = sender as Grid;
            if (g == null)
                return;
            datawrapper.TypeObject t = g.DataContext as datawrapper.TypeObject;
            if (t == null)
                return;
            NavigationService.Navigate(new Uri("/views/TypeObjectDetails2.xaml?typeObjectID=" + t.typeObjectID.ToString(), UriKind.Relative));
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
            NavigationService.Navigate(new Uri("/views/TomeDetails.xaml?tomeID=1", UriKind.RelativeOrAbsolute));
        }

        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            LongListMultiSelector l = null;
            CheckBox c = sender as CheckBox;
            if (c.Name.Equals("selectAllCheckBox1"))
            {
                l = NoteList;
            }
            else if (c.Name.Equals("selectAllCheckBox2"))
            {
                l = TypeObjectList;
            }

            if (l != null)
            {
                l.EnforceIsSelectionEnabled = true;
                foreach (var item in l.ItemsSource)
                {
                    var container = l.ContainerFromItem(item)
                                          as LongListMultiSelectorItem;
                    if (container != null) container.IsSelected = true;
                }
            }

        }

        private void selectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            LongListMultiSelector l = null;
            CheckBox c = sender as CheckBox;
            if (c.Name.Equals("selectAllCheckBox1"))
            {
                l = NoteList;
            }
            else if (c.Name.Equals("selectAllCheckBox2"))
            {
                l = TypeObjectList;
            }
            if (l != null)
            {
                l.EnforceIsSelectionEnabled = false;
            }
        }
    }
}