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
namespace WritersToolbox.views
{
    public partial class AddType : PhoneApplicationPage
    {
        public AddType()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SaveType(object sender, EventArgs e)
        {
            Color c = slider.Color;

            String r = c.R.ToString("X2");
            String g = c.G.ToString("X2");
            String b = c.B.ToString("X2");

            String color = "#" + r + g + b;
            String title = tTitle.Text;

            Types.types_VM.createType(title, color, "");
        }


        private void CancelType(object sender, EventArgs e)
        {

        }
    }
}