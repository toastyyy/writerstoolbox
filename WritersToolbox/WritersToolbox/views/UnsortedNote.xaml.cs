using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using WritersToolbox.viewmodels;

namespace WritersToolbox.views
{
    public partial class UnsortedNote : PhoneApplicationPage
    {
        private UnsortedNoteViewModel unsrotedNotes;
        private bool isselected;
        public UnsortedNote()
        {
            InitializeComponent();
            isselected = false;
            unsrotedNotes = new UnsortedNoteViewModel();
            llms_unsortedNote.ItemsSource = unsrotedNotes.getObservableColletion();
        }

        private void unsortedNote_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {

                if (!llms_unsortedNote.SelectedItems.Contains((UnsortedNoteViewModel)e.AddedItems[0]))
                {
                    llms_unsortedNote.SelectedItems.Add(((UnsortedNoteViewModel)e.AddedItems[0]));
                }
            }
            if (e.RemovedItems.Count > 0)
            {

                if (llms_unsortedNote.SelectedItems.Contains((UnsortedNoteViewModel)e.RemovedItems[0]))
                {
                    llms_unsortedNote.SelectedItems.Remove(((UnsortedNoteViewModel)e.RemovedItems[0]));
                }

            }

            if (llms_unsortedNote.SelectedItems.Count < llms_unsortedNote.ItemsSource.Count)
            {
                isselected = false;
                selectAllCheckBox.IsChecked = false;

            }
            else if (llms_unsortedNote.SelectedItems.Count == llms_unsortedNote.ItemsSource.Count)
            {
                isselected = true;
                selectAllCheckBox.IsChecked = true;

            }
            if (llms_unsortedNote.SelectedItems.Count == 0)
            {
                llms_unsortedNote.EnforceIsSelectionEnabled = false;
                this.ApplicationBar.IsVisible = false;
            }
            else
            {
                this.ApplicationBar.IsVisible = true;

            }  
            
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            ObservableCollection<UnsortedNoteViewModel> collection = new ObservableCollection<UnsortedNoteViewModel>();
            ObservableCollection<UnsortedNoteViewModel> collection2 = new ObservableCollection<UnsortedNoteViewModel>(
                (ObservableCollection<UnsortedNoteViewModel>)llms_unsortedNote.ItemsSource);
            foreach (UnsortedNoteViewModel item in collection2)
            {
                if(llms_unsortedNote.SelectedItems.Contains(item))
                {
                    llms_unsortedNote.ItemsSource.Remove(item);
                    llms_unsortedNote.SelectedItems.Remove(item);
                    collection.Add(item);
                }

            }
            unsrotedNotes.deleteUnsortedNote(collection);
            if (llms_unsortedNote.ItemsSource.Count == 0)
            {
                selectAllCheckBox.IsChecked = false;
            }
        }
                //Fertig
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            llms_unsortedNote.ItemsSource = unsrotedNotes.getObservableColletion();
        }

        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!isselected)
            {
                llms_unsortedNote.EnforceIsSelectionEnabled = true;
                llms_unsortedNote.SelectedItems.Clear();
                ObservableCollection<UnsortedNoteViewModel> collection =
                    (ObservableCollection<UnsortedNoteViewModel>)llms_unsortedNote.ItemsSource;
                foreach (UnsortedNoteViewModel item in collection)
                {
                    llms_unsortedNote.SelectedItems.Add(item);
                }
            }
        }

        private void selectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (llms_unsortedNote.SelectedItems.Count == llms_unsortedNote.ItemsSource.Count)
            {
                llms_unsortedNote.EnforceIsSelectionEnabled = false;
                llms_unsortedNote.SelectedItems.Clear();
            }
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid selector = sender as Grid;
            PhoneApplicationService.Current.State["memoryNoteID"] = ((UnsortedNoteViewModel)selector.DataContext).memoryNoteID + "";
            PhoneApplicationService.Current.State["edit"] = "true";
            NavigationService.Navigate(new Uri("/views/AddNote.xaml", UriKind.Relative));
            
        }

        private void Grid_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid g = (Grid)sender;

            llms_unsortedNote.SelectedItems.Add(((UnsortedNoteViewModel)g.DataContext));

            llms_unsortedNote.EnforceIsSelectionEnabled = true;
          
        }

        private void zurueckButton_Click(object sender, EventArgs e)
        {
            llms_unsortedNote.EnforceIsSelectionEnabled = false;
        }
    }
}