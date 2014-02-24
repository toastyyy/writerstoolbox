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
using WritersToolbox.Resources;
using System.Collections;

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
            trash.loadData();
            this.DataContext = trash;
        }


        private void trash_selectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count > 0)
            {

                if (!llms_trash.SelectedItems.Contains((object)e.AddedItems[0]))
                {
                    llms_trash.SelectedItems.Add(((object)e.AddedItems[0]));
                }
            }
            if (e.RemovedItems.Count > 0)
            {

                if (llms_trash.SelectedItems.Contains((object)e.RemovedItems[0]))
                {
                    llms_trash.SelectedItems.Remove(((object)e.RemovedItems[0]));
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
            IList list = llms_trash.SelectedItems;
            
            this.trash.deleteTrash(list);
        }

        private void restoreButton_Click(object sender, EventArgs e)
        {
            //ObservableCollection<object> collection = new ObservableCollection<object>();
            //ObservableCollection<object> collection2 = new ObservableCollection<object>(
            //    (ObservableCollection<object>)llms_trash.ItemsSource);
            //foreach (object item in collection2)
            //{
            //    if (llms_trash.SelectedItems.Contains(item))
            //    {
            //        llms_trash.ItemsSource.Remove(item);
            //        llms_trash.SelectedItems.Remove(item);
            //        collection.Add(item);
            //    }

            //}
            //trash.restoreTrash(collection);

            //if (llms_trash.ItemsSource.Count == 0)
            //{
            //    selectAllCheckBox.IsChecked = false;
            //}
        }

                //Fertig
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            ApplicationBarIconButton btn3 = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            btn1.Text = AppResources.AppBarDelete;
            btn2.Text = AppResources.AppBarBack;
            btn3.Text = AppResources.AppBarRestore;
            trash.loadData();
            //llms_trash.ItemsSource = trash.getObservableColletion();
        }

        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!isselected)
            {
                llms_trash.EnforceIsSelectionEnabled = true;
                llms_trash.SelectedItems.Clear();
                ObservableCollection<object> collection =
                    (ObservableCollection<object>)llms_trash.ItemsSource;
                foreach (object item in collection)
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
            //Grid selector = sender as Grid;
            //PhoneApplicationService.Current.State["memoryNoteID"] = ((TrashbinViewModel)selector.DataContext).memoryNoteID + "";
            //PhoneApplicationService.Current.State["edit"] = "true";
            //NavigationService.Navigate(new Uri("/views/Trashbin.xaml", UriKind.Relative));
            
        }

        private void Grid_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid g = (Grid)sender;

            llms_trash.SelectedItems.Add((g.DataContext));

            llms_trash.EnforceIsSelectionEnabled = true;
          
        }

        private void zurueckButton_Click(object sender, EventArgs e)
        {
            llms_trash.EnforceIsSelectionEnabled = false;
        }
    }
}