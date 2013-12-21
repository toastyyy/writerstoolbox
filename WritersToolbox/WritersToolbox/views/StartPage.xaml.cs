﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WritersToolbox.gui
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
            models.WritersToolboxDatebase db = models.WritersToolboxDatebase.getInstance();
            try
            {
                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            
        }

        private void newNote(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/addnote_test.xaml", UriKind.Relative));
        }

        private void navigateToTypes(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/Types.xaml", UriKind.Relative));
        }

        
    }
}