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

        //private static TestOverview vm = null;

        //public static TestOverview VM
        //{
        //    get
        //    {
        //        if (vm == null)
        //        {
        //            vm = new TestOverview();
        //            vm.LoadData();
        //        }


        //        return vm;
        //    }
        //}

        public static TypesViewModel types_VM = null;

        public static TypesViewModel Types_VM
        {
            get
            {
                if (types_VM == null)
                {
                    types_VM = new TypesViewModel();
                    types_VM.LoadData();
                }
                return types_VM;
            }
        }

        public TypesOverview()
        {
            InitializeComponent();
            //DataContext = VM;
            DataContext = Types_VM;
            
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        
    }
}