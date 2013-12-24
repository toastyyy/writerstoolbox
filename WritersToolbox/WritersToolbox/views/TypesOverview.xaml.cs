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
using System.Collections.ObjectModel;
namespace WritersToolbox.views
{
    public partial class TypesOverview : PhoneApplicationPage
    {
        public TypesOverview()
        {
            InitializeComponent();
            
        }
        


        private void pinch_out(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                if (e.PinchManipulation.CumulativeScale < 1d)
                {
                    System.Diagnostics.Debug.WriteLine("Zoomin");
                    NavigationService.Navigate(new Uri("/views/Types.xaml", UriKind.Relative));
                }
                else
                    System.Diagnostics.Debug.WriteLine("Zoomout");

            }
        }

        
    }
}