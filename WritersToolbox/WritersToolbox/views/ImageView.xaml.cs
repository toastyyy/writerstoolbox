﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using WritersToolbox.models;

namespace WritersToolbox.views
{
    public partial class ImageView : PhoneApplicationPage
    {
        Image im;
        public ImageView()
        {
            InitializeComponent();
            PhoneApplicationService.Current.State["OppendImageView"] = "true";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            try
            {
                im = PhoneApplicationService.Current.State["imageView"] as Image;
                imageView.Source = im.Source;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("deletedImages"))
            {
                string cachImages = (PhoneApplicationService.Current.State["deletedImages"] as string);
                cachImages += ((MyImage)im.DataContext).Name + "|";
                PhoneApplicationService.Current.State["deletedImages"] = cachImages;
            }
            else
            {
                string cachImages = ((MyImage)im.DataContext).Name + "|";
                PhoneApplicationService.Current.State["deletedImages"] = cachImages;
            }

            NavigationService.go(new Uri("/views/addnote_test.xaml", UriKind.Relative));
            NavigationService.GoBack();
        }

    }
}