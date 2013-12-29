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
using System.Diagnostics;
using WritersToolbox.models;


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

        

        
        private void LongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;
            if (selector == null)
                return;
            TypeObject to = selector.SelectedItem as TypeObject;
            if (to == null)
                return;
            if (to.fk_typeID == -1)
            {
                NavigationService.Navigate(new Uri("/views/AddType.xaml", UriKind.Relative));
            }
            else if (to.fk_typeID == -2)
            {
                NavigationService.Navigate(new Uri("/views/AddTypeObject.xaml", UriKind.Relative));
            } 
            selector.SelectedItem = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("item"))
            {
                var item = NavigationContext.QueryString["item"];
                var indexParsed = int.Parse(item);
                PivotMain.SelectedIndex = indexParsed - 1;
            }
        }
        
        
    }
}