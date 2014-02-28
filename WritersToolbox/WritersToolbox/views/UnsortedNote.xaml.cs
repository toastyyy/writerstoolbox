﻿using System;
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

namespace WritersToolbox.views
{
    public partial class UnsortedNote : PhoneApplicationPage
    {
        //ViewModel zwischen UnsortedNote(View) und Note(Entity)
        private UnsortedNoteViewModel unsrotedNotes;
        //Ob alle in der List selektiert sind.
        private bool isAllUnsortedNoteSelected;

        /// <summary>
        /// Default Konstruktor.
        /// </summary>
        public UnsortedNote()
        {
            InitializeComponent();
            isAllUnsortedNoteSelected = false;
            unsrotedNotes = new UnsortedNoteViewModel();
            llms_unsortedNote.ItemsSource = unsrotedNotes.getUnsortedNotes();
        }

        /// <summary>
        /// Wenn neue Selektion in LongListMultiSelector der unsortierte Notizen stattfindet,
        /// wird diese event ausgeführt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void unsortedNote_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Ob eine Notiz von der List selektiert.
            if (e.AddedItems.Count > 0)
            {
                //wird es in SelectedItems des LongListMultiSelector hinzugefügt.
                if (!llms_unsortedNote.SelectedItems.Contains((datawrapper.UnsortedMemoryNote)e.AddedItems[0]))
                {
                    llms_unsortedNote.SelectedItems.Add(((datawrapper.UnsortedMemoryNote)e.AddedItems[0]));
                }
            }
            //Ob eine Selektion einer Notiz von der List ausgehoben.
            if (e.RemovedItems.Count > 0)
            {
                //wird es in SelectedItems des LongListMultiSelector gelöscht.
                if (llms_unsortedNote.SelectedItems.Contains((datawrapper.UnsortedMemoryNote)e.RemovedItems[0]))
                {
                    llms_unsortedNote.SelectedItems.Remove(((datawrapper.UnsortedMemoryNote)e.RemovedItems[0]));
                }

            }
            //Wenn Anzahl der selektierte unsortierter Notizen kleiner als Anzahl der unsortierten Notizen in der List.
            if (llms_unsortedNote.SelectedItems.Count < llms_unsortedNote.ItemsSource.Count)
            {
                isAllUnsortedNoteSelected = false;
                selectAllCheckBox.IsChecked = false;

            }
            //Wenn Anzahl der selektierte unsortierter Notizen gleich Anzahl der unsortierten Notizen in der List.
            else if (llms_unsortedNote.SelectedItems.Count == llms_unsortedNote.ItemsSource.Count)
            {
                isAllUnsortedNoteSelected = true;
                selectAllCheckBox.IsChecked = true;

            }

            //Wenn Anzahl der selektierten unsortierte Notizen gleich 0 ist.
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

        /// <summary>
        /// wird ausgeführt, wenn eine oder mehrere unsortierte Notiz gelöscht werden müssen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            //Gesamte List in der Collection2 kopieren, danach werden alle unsortierte Notizen durchgelaufen, 
            //die selektierte wird in der Collection gespeichert, damit sie danach gelöscht wird.
            ObservableCollection<datawrapper.UnsortedMemoryNote> collection = new ObservableCollection<datawrapper.UnsortedMemoryNote>();
            ObservableCollection<datawrapper.UnsortedMemoryNote> collection2 = new ObservableCollection<datawrapper.UnsortedMemoryNote>(
                (ObservableCollection<datawrapper.UnsortedMemoryNote>)llms_unsortedNote.ItemsSource);

            foreach (datawrapper.UnsortedMemoryNote item in collection2)
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
        
        /// <summary>
        /// wird ausgeführt wenn es zu diesem Screen navigiert wird.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            llms_unsortedNote.ItemsSource = unsrotedNotes.getUnsortedNotes();
            ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            btn1.Text = AppResources.AppBarDelete;
            btn2.Text = AppResources.AppBarBack;
        }

        /// <summary>
        /// Wenn alle unsortierten Notizen ausgewählt werden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!isAllUnsortedNoteSelected)
            {
                llms_unsortedNote.EnforceIsSelectionEnabled = true;
                llms_unsortedNote.SelectedItems.Clear();
                ObservableCollection<datawrapper.UnsortedMemoryNote> collection =
                    (ObservableCollection<datawrapper.UnsortedMemoryNote>)llms_unsortedNote.ItemsSource;
                foreach (datawrapper.UnsortedMemoryNote item in collection)
                {
                    llms_unsortedNote.SelectedItems.Add(item);
                }
            }
        }

        /// <summary>
        /// Wenn die option Alle auswählen aufgehoben wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (llms_unsortedNote.SelectedItems.Count == llms_unsortedNote.ItemsSource.Count)
            {
                llms_unsortedNote.EnforceIsSelectionEnabled = false;
                llms_unsortedNote.SelectedItems.Clear();
            }
        }

        /// <summary>
        /// Wenn eine unsortierte Notiz geöffnet wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid selector = sender as Grid;
            PhoneApplicationService.Current.State["memoryNoteID"] = ((datawrapper.UnsortedMemoryNote)selector.DataContext).memoryNoteID + "";
            PhoneApplicationService.Current.State["edit"] = "true";
            NavigationService.Navigate(new Uri("/views/AddNote.xaml", UriKind.Relative));
            
        }

        /// <summary>
        /// Wenn auf eine unsortierte Notiz festgedruckt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid g = (Grid)sender;

            llms_unsortedNote.SelectedItems.Add(((datawrapper.UnsortedMemoryNote)g.DataContext));

            llms_unsortedNote.EnforceIsSelectionEnabled = true;
          
        }

        /// <summary>
        /// Wenn zurückButton geclickt wird.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zurueckButton_Click(object sender, EventArgs e)
        {
            llms_unsortedNote.EnforceIsSelectionEnabled = false;
        }
    }
}