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
            if (NavigationContext.QueryString.ContainsKey("eventID "))
            {
                int eID = int.Parse(NavigationContext.QueryString["eventID "]);
                this.edvm = new EventDetailViewModel(eID);
                this.edvm.LoadData();
                this.DataContext = this.edvm.Event;
            }
        }
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/TomeDetails.xaml?tomeID=1", UriKind.RelativeOrAbsolute));
        }
    }
}