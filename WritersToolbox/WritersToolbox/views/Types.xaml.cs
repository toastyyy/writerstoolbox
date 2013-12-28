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
    public partial class Types : PhoneApplicationPage
    {

        public static TypesViewModel types_VM = null;

        public static TypesViewModel Types_VM
        {
            get
                {
                    if (types_VM == null)
                    {
                        types_VM = new TypesViewModel();
                        if (!types_VM.IsDataLoaded)
                            types_VM.LoadData();
                    }
                    return types_VM;
                }
        }

        //public static sampleData.SampleDataTyp sdt = null;

        //public static sampleData.SampleDataTyp SDT
        //{
        //    get
        //    {
        //        if (sdt == null)
        //        {
        //            sdt = new sampleData.SampleDataTyp();
                    
        //        }
        //        return sdt;
        //    }
        //}
        

        public Types()
        {
            InitializeComponent();
            DataContext = Types_VM;
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