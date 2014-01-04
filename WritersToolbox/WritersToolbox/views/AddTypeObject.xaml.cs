using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Diagnostics;

namespace WritersToolbox.views
{
    public partial class AddTypeObject : PhoneApplicationPage
    {
        private int typeID;

        public AddTypeObject()
        {
            InitializeComponent();
        }

        private void SaveTypeObject(object sender, EventArgs e)
        {
            Color c = slider.Color;

            String r = c.R.ToString("X2");
            String g = c.G.ToString("X2");
            String b = c.B.ToString("X2");

            String color = "#" + r + g + b;
            String name = toName.Text;

            try 
            {
                Types.types_VM.createTypeObject(name, color, "", typeID);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Objekt konnte nicht erstellt werden");
            }

            NavigationService.GoBack();

        }

        private void CancelTypeObject(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("typeID"))
            {
                var tID = NavigationContext.QueryString["typeID"];
                typeID = int.Parse(tID);
            }
        }

        //protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    base.OnNavigatedTo(e);
        //    PhoneApplicationService.Current.State["NewTypeObject"] = typeID;
        //}
    }
}