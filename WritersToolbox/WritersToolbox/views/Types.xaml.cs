using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;


namespace WritersToolbox.views
{
    public partial class Types : PhoneApplicationPage
    {
        
        
        

        public Types()
        {
            InitializeComponent();
           
        }

        

        private void pinch_out(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            
            if(e.PinchManipulation != null)
            {
                if (e.PinchManipulation.CumulativeScale > 1d)
                {
                    System.Diagnostics.Debug.WriteLine("Zoomout");
                    NavigationService.Navigate(new Uri("/views/TypesOverview.xaml", UriKind.Relative));
                } else
                    System.Diagnostics.Debug.WriteLine("Zoomin");
                                
            }
            
            
        }

        private void navUeberblick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/TypesOverview.xaml", UriKind.Relative));
        }

        
    }
}