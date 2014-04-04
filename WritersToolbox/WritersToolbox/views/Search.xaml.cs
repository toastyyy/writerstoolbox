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
    public partial class Search : PhoneApplicationPage
    {
        SearchViewModel svm;
        public Search()
        {
            InitializeComponent();
        }

        private void searchQuery_TextChanged(object sender, TextChangedEventArgs e)
        {
            svm.loadByQuery(searchQuery.Text);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            svm = new SearchViewModel();
            this.DataContext = svm;
        }
    }
}