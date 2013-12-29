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
using System.ComponentModel;
using WritersToolbox.models;
namespace WritersToolbox.views
{
    public partial class TypesOverview : PhoneApplicationPage
    {


        public TypesOverview()
        {
            InitializeComponent();
            DataContext = Types.Types_VM;
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

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            if (selector == null)
                return;
            models.Type t = selector.SelectedItem as models.Type;
            if (t == null)
                return;
            if (t.typeID == -1)
            {
                NavigationService.Navigate(new Uri("/views/AddType.xaml", UriKind.Relative));
            }
            else
                NavigationService.Navigate(new Uri("/views/Types.xaml?item=" + t.typeID, UriKind.Relative));

            selector.SelectedItem = null;
        }

       

        
    }
}