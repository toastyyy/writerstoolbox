using System;
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

namespace WritersToolbox.views
{
    public partial class ImageView : PhoneApplicationPage
    {
        public ImageView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string strCodeTiers = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("path",out strCodeTiers))
            {
                 using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                 {
                     using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(NavigationContext.QueryString["path"], FileMode.Open, FileAccess.Read))
                     {
                         BitmapImage bi = new BitmapImage();
                         bi.SetSource(fileStream);
                         
                         imageView.Source = bi;

                     }
                 }
                 
            }
        }
    }
}