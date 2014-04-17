﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WritersToolbox.viewmodels;
using WritersToolbox.models;

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
            if (!searchQuery.Text.Equals("")) {
                svm.loadByQuery(searchQuery.Text);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            svm = new SearchViewModel();
            this.DataContext = svm;
        }

        private void LongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ISearchable clicked = (ISearchable)e.AddedItems[0];
            NavigationService.Navigate(clicked.Link);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NavigationService.RemoveBackEntry();
        }
    }
}