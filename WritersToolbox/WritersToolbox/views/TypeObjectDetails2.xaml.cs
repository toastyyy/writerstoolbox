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
using System.Windows.Media;
using WritersToolbox.Resources;
using System.Diagnostics;
namespace WritersToolbox.views
{
    public partial class TypeObjectDetails2 : PhoneApplicationPage
    {
        TypeDetailViewModel tdvm = null;
        public TypeObjectDetails2()
        {
            InitializeComponent();
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
            if (NavigationContext.QueryString.ContainsKey("typeObjectID"))
            {
                int toID = int.Parse(NavigationContext.QueryString["typeObjectID"]);
                tdvm = new TypeDetailViewModel(toID);
                this.DataContext = tdvm;
            }
            if (this.tdvm.TypeObject.notes.Count > 0)
            {
                selectAllCheckBox.Visibility = Visibility.Visible;
            }
            ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            btn1.Text = AppResources.AppBarEdit;
            btn2.Text = AppResources.AppBarDelete;
        }

        /// <summary>
        /// Eine zum TypObjekt zugehörige Notiz wurde ausgewählt und wird zum bearbeiten geöffnet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void noteSelected(object sender, System.Windows.Input.GestureEventArgs e)
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


        private void editTypeObjectClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/TypeObjectEdit.xaml?typeObjectID=" + this.tdvm.TypeObject.typeObjectID, UriKind.Relative));
        }

        //private void deleteTypeObject_Click(object sender, EventArgs e)
        //{
        //    MessageBoxResult mbr = MessageBox.Show("Möchtest du dieses Objekt wirklich löschen? Alle angehängten Notizen werden mitgelöscht. Du kannst das Objekt über den Papierkorb wiederherstellen.", "Löschen bestätigen", MessageBoxButton.OKCancel);
        //    if(mbr.Equals(MessageBoxResult.OK)) {
        //        TypesViewModel tvm = new TypesViewModel();
        //        tvm.deleteTypeObject(this.tdvm.TypeObject.typeObjectID);
        //        NavigationService.Navigate(new Uri("/views/Types.xaml", UriKind.Relative));
        //    }
        //}

        private void TryDeleteTypeObject(object sender, EventArgs e)
        {
            TypeObjectDeleteQuestion.Text = AppResources.TypeObjectDeleteQuestion1 + tdvm.TypeObject.name.ToString() + AppResources.TypeObjectDeleteQuestion2;
            deleteTypeObjectPopup.IsOpen = true;
        }

        private void DeleteTypeObject(object sender, EventArgs e)
        {
            tdvm.deleteTypeObject(tdvm.TypeObject.typeObjectID, keepNotes.IsChecked.Value);
            deleteTypeObjectPopup.IsOpen = false;
            NavigationService.GoBack();
        }

        private void DoNotDeleteTypeObject(object sender, EventArgs e)
        {
            deleteTypeObjectPopup.IsOpen = false;
        }

        /// <summary>
        /// Wenn alle unsortierten Notizen ausgewählt werden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            multiselector.EnforceIsSelectionEnabled = true;
            foreach (var item in multiselector.ItemsSource)
            {
                var container = multiselector.ContainerFromItem(item)
                                      as LongListMultiSelectorItem;
                if (container != null) container.IsSelected = true;
            }
        }

        /// <summary>
        /// Wenn die option Alle auswählen aufgehoben wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            multiselector.EnforceIsSelectionEnabled = false;
        }


        private void NoteSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (multiselector.SelectedItems.Count != 0)
            {
                deleteButton.Visibility = Visibility.Visible;
            }
            else
            {
                deleteButton.Visibility = Visibility.Collapsed;
            }
        }

        private void TryDeleteAssociation(object sender, RoutedEventArgs e)
        {
            NoteDeleteQuestion.Text = AppResources.NoteDeleteQuestionSingle;
            if (multiselector.SelectedItems.Count > 1)
            {
                NoteDeleteQuestion.Text = AppResources.NoteDeleteQuestion1 + multiselector.SelectedItems.Count + AppResources.NoteDeleteQuestion2;
            }
            
            deleteNotesPopup.IsOpen = true;
            
        }


        private void DeleteNotes(object sender, EventArgs e)
        {
            foreach (datawrapper.MemoryNote note in multiselector.SelectedItems)
            {
                this.tdvm.deleteNote(note.memoryNoteID, true);
            }
            deleteNotesPopup.IsOpen = false;
            deleteButton.Visibility = Visibility.Collapsed;
            this.tdvm.LoadData();
            if (this.tdvm.TypeObject.notes.Count == 0)
            {
                selectAllCheckBox.Visibility = Visibility.Collapsed;
            }
        }

        private void DoNotDeleteNotes(object sender, EventArgs e)
        {
            deleteNotesPopup.IsOpen = false;
        }

        private void multiselector_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListMultiSelector llms = (LongListMultiSelector)sender;
            FrameworkElement c = (FrameworkElement)e.OriginalSource;
            while (!llms.Parent.GetType().IsAssignableFrom((new Grid()).GetType())) { };
            Grid g = (Grid)c.Parent;
            llms.SelectedItems.Add(((datawrapper.MemoryNote)g.DataContext));
        }
    }
}