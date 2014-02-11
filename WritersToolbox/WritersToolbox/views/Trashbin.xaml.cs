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
    public partial class Trashbin : PhoneApplicationPage
    {
        private TrashbinViewModel trash;
        private bool isselected;
        public Trashbin()
        {
            InitializeComponent();
            isselected = false;
            trash = new TrashbinViewModel();
            llms_trash.ItemsSource = trash.getObservableColletion();
        }

        private void trash_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {

                if (!llms_trash.SelectedItems.Contains((TrashbinViewModel)e.AddedItems[0]))
                {
                    llms_trash.SelectedItems.Add(((TrashbinViewModel)e.AddedItems[0]));
                }
            }
            if (e.RemovedItems.Count > 0)
            {

                if (llms_trash.SelectedItems.Contains((TrashbinViewModel)e.RemovedItems[0]))
                {
                    llms_trash.SelectedItems.Remove(((TrashbinViewModel)e.RemovedItems[0]));
                }

            }

            if (llms_trash.SelectedItems.Count < llms_trash.ItemsSource.Count)
            {
                isselected = false;
                selectAllCheckBox.IsChecked = false;

            }
            else if (llms_trash.SelectedItems.Count == llms_trash.ItemsSource.Count)
            {
                isselected = true;
                selectAllCheckBox.IsChecked = true;

            }
            if (llms_trash.SelectedItems.Count == 0)
            {
                llms_trash.EnforceIsSelectionEnabled = false;
                this.ApplicationBar.IsVisible = false;
            }
            else
            {
                this.ApplicationBar.IsVisible = true;

            }  
            
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            ObservableCollection<TrashbinViewModel> collection = new ObservableCollection<TrashbinViewModel>();
            ObservableCollection<TrashbinViewModel> collection2 = new ObservableCollection<TrashbinViewModel>(
                (ObservableCollection<TrashbinViewModel>)llms_trash.ItemsSource);
            foreach (TrashbinViewModel item in collection2)
            {
                if (llms_trash.SelectedItems.Contains(item))
                {
                    llms_trash.ItemsSource.Remove(item);
                    llms_trash.SelectedItems.Remove(item);
                    collection.Add(item);
                }

            }
            trash.deleteTrash(collection);
            if (llms_trash.ItemsSource.Count == 0)
            {
                selectAllCheckBox.IsChecked = false;
            }
        }

        private void restoreButton_Click(object sender, EventArgs e)
        {
            ObservableCollection<TrashbinViewModel> collection = new ObservableCollection<TrashbinViewModel>();
            ObservableCollection<TrashbinViewModel> collection2 = new ObservableCollection<TrashbinViewModel>(
                (ObservableCollection<TrashbinViewModel>)llms_trash.ItemsSource);
            foreach (TrashbinViewModel item in collection2)
            {
                if (llms_trash.SelectedItems.Contains(item))
                {
                    llms_trash.ItemsSource.Remove(item);
                    llms_trash.SelectedItems.Remove(item);
                    collection.Add(item);
                }

            }
            trash.restoreTrash(collection);

            if (llms_trash.ItemsSource.Count == 0)
            {
                selectAllCheckBox.IsChecked = false;
            }
        }

                //Fertig
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            llms_trash.ItemsSource = trash.getObservableColletion();
        }

        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!isselected)
            {
                llms_trash.EnforceIsSelectionEnabled = true;
                llms_trash.SelectedItems.Clear();
                ObservableCollection<TrashbinViewModel> collection =
                    (ObservableCollection<TrashbinViewModel>)llms_trash.ItemsSource;
                foreach (TrashbinViewModel item in collection)
                {
                    llms_trash.SelectedItems.Add(item);
                }
            }
        }

        private void selectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (llms_trash.SelectedItems.Count == llms_trash.ItemsSource.Count)
            {
                llms_trash.EnforceIsSelectionEnabled = false;
                llms_trash.SelectedItems.Clear();
            }
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid selector = sender as Grid;
            PhoneApplicationService.Current.State["memoryNoteID"] = ((TrashbinViewModel)selector.DataContext).memoryNoteID + "";
            PhoneApplicationService.Current.State["edit"] = "true";
            NavigationService.Navigate(new Uri("/views/Trashbin.xaml", UriKind.Relative));
            
        }

        private void Grid_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid g = (Grid)sender;

            llms_trash.SelectedItems.Add(((TrashbinViewModel)g.DataContext));

            llms_trash.EnforceIsSelectionEnabled = true;
          
        }

        private void zurueckButton_Click(object sender, EventArgs e)
        {
            llms_trash.EnforceIsSelectionEnabled = false;
        }
    }
}