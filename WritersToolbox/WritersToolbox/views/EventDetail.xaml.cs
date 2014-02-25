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

namespace WritersToolbox.views
{
    public partial class EventDetail : PhoneApplicationPage
    {
        private EventDetailViewModel edvm = null;
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
                this.edvm = new EventDetailViewModel(eID);
                this.edvm.LoadData();
                this.DataContext = this.edvm.Event;
            }
            if (this.edvm.Event.notes.Count == 0)
            {
                selectAllCheckBox1.Visibility = Visibility.Collapsed;
            }
            if (this.edvm.Event.typeObjects.Count == 0)
            {
                selectAllCheckBox2.Visibility = Visibility.Collapsed;
            }
        }

        private void selectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            LongListMultiSelector l = null;
            CheckBox c = sender as CheckBox;
            if(c.Name.Equals("selectAllCheckBox1"))
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