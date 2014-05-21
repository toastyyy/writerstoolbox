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
using System.Threading;

namespace WritersToolbox.views
{
    /// <summary>
    /// Die Klasse Trashbin verwaltet "gelöschte" Daten. Die Daten werden in einer Liste
    /// gezeigt und können hier endgültig gelöscht werden sowie wieder zur
    /// ursprünglichen Position zurückgesetzt werden.
    /// </summary>
    public partial class Trashbin : PhoneApplicationPage
    {
        //private bool loaded = false;
        //private int filterIndex;
        private TrashbinViewModel trash;
        private bool isselected;


        public Trashbin()
        {
            InitializeComponent();
            isselected = false;
            trash = new TrashbinViewModel();
            trash.loadData();
            this.DataContext = trash;
            if (this.llms_trash.ItemsSource.Count == 0)
            {
                selectAllCheckBox.IsEnabled = false;
                //filterMemoryNotes.IsEnabled = false;
            }
           
        }
        /// <summary>
        /// Sollte sich was an der Checkbox-Selection verändert haben wird das hier überprüft und sichtbar gemacht.
        /// </summary>
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


        /// <summary>
        /// Holt sich die Liste der Items aus dem Papierkorb und deaktiviert SelectAll 
        /// Checkboxen falls die Liste leer ist.
        /// </summary>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            IList list = llms_trash.SelectedItems;
            
            this.trash.deleteTrash(list);

            if (this.llms_trash.ItemsSource.Count == 0)
            {
                selectAllCheckBox.IsChecked = false;
                selectAllCheckBox.IsEnabled = false;
                //filterMemoryNotes.IsChecked = false;
                //filterMemoryNotes.IsEnabled = false;
            }
            deletePopup.IsOpen = false;
        }

        private void restoreButton_Click(object sender, EventArgs e)
        {
            IList list = llms_trash.SelectedItems;
            this.trash.restoreTrash(list);
            if (this.llms_trash.ItemsSource.Count == 0)
            {
                selectAllCheckBox.IsChecked = false;
                selectAllCheckBox.IsEnabled = false;
                //filterMemoryNotes.IsChecked = false;
                //filterMemoryNotes.IsEnabled = false;
            }

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
            //filterIndex = 0;
            trash.loadData();
            if (this.llms_trash.ItemsSource.Count == 0)
            {
                selectAllCheckBox.IsEnabled = false;
                //filterMemoryNotes.IsEnabled = false;
            }
            //llms_trash.ItemsSource = trash.getObservableColletion();
        }

        /// <summary>
        //Filter für Späteren gebrauch evt.
        /// </summary>       
        //private void filter_Checked(object sender, RoutedEventArgs e)
        //{
        //    this.trash.deleteList();
        //    this.trash.loadDeletedTomes();
        //}
        //private void filter_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    this.trash.deleteList();
        //    this.trash.loadData();
        //}

        /// <summary>
        // Selektiert alle Items in der Liste.
        /// </summary>     
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
        /// <summary>
        // Deselektiert alle Items in der Liste.
        /// </summary>   
        private void selectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (llms_trash.SelectedItems.Count == llms_trash.ItemsSource.Count)
            {
                llms_trash.EnforceIsSelectionEnabled = false;
                llms_trash.SelectedItems.Clear();
            }
        }
        /// <summary>
        // Bei Tap auf ein Item wird es markiert.
        /// </summary>   
        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid g = (Grid)sender;

            llms_trash.SelectedItems.Add((g.DataContext));

            llms_trash.EnforceIsSelectionEnabled = true;
            
        }
        /// <summary>
        // Bei Hold auf ein Item wird es markiert.
        /// </summary>  
        private void Grid_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid g = (Grid)sender;

            llms_trash.SelectedItems.Add((g.DataContext));

            llms_trash.EnforceIsSelectionEnabled = true;
          
        }

        private void zurueckButton_Click(object sender, EventArgs e)
        {
            llms_trash.EnforceIsSelectionEnabled = false;
            llms_trash.SelectedItems.Clear();
        }

        private void DoNotDelete(object sender, RoutedEventArgs e)
        {
            deletePopup.IsOpen = false;
        }

        private void deletePopupClick(object sender, EventArgs e)
        {
            deletePopup.IsOpen = true;
        }
        /// <summary>
        // Alte Filter Methode evt für späteren Gebrauch.
        /// </summary>  

        //private void FilterChanged(object sender, SelectionChangedEventArgs e)
        //{

        //    ListPicker lP = sender as ListPicker;
        //    //lP.SelectedIndex = 0;
        //    if (trash != null)
        //    {
        //        if (lP.SelectedIndex == 0)
        //        {
        //            this.trash.deleteList();
        //            this.trash.loadData();
        //            //NavigationService.Navigate(new Uri("/views/Trashbin.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
        //            //NavigationService.RemoveBackEntry();
        //        }
        //        else if (lP.SelectedIndex == 1)
        //        {
        //            this.trash.deleteList();
        //            this.trash.loadDeletedMemoryNotes();
        //            NavigationService.Navigate(new Uri("/views/Trashbin.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
        //            NavigationService.RemoveBackEntry();
        //        }
        //        else if (lP.SelectedIndex == 2)
        //        {
        //            this.trash.deleteList();
        //            this.trash.loadDeletedBooks();
        //            NavigationService.Navigate(new Uri("/views/Trashbin.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
        //            NavigationService.RemoveBackEntry();
        //        }
        //        else if (lP.SelectedIndex == 3)
        //        {
        //            this.trash.deleteList();
        //            this.trash.loadDeletedTomes();
        //            NavigationService.Navigate(new Uri("/views/Trashbin.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
        //            NavigationService.RemoveBackEntry();
        //        }
        //        else if (lP.SelectedIndex == 4)
        //        {
        //            this.trash.deleteList();
        //            this.trash.loadDeletedTypes();
        //        }
        //        else if (lP.SelectedIndex == 5)
        //        {
        //            this.trash.deleteList();
        //            this.trash.loadDeletedTypeObjects();
        //        }
        //        else if (lP.SelectedIndex == 6)
        //        {
        //            this.trash.deleteList();
        //            this.trash.loadDeletedEvents();
        //        }
        //    }
        //}

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            

        }
        /// <summary>
        // Reload der Liste bei Filter.
        /// </summary>  
        //private void PageLoaded(object sender, RoutedEventArgs e)
        //{
        //    //loaded = true;
        //    //FilterPicker.SelectedIndex = filterIndex;
        //}
       
    }
}