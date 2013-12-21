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
//using Microsoft.Xna.Framework.Graphics.PackedVector;
namespace WritersToolbox.views
{
    public partial class PivotPage1 : PhoneApplicationPage
    {
        public PivotPage1()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double height = heightCalculator(textBox1.Text, textBox1.FontFamily, textBox1.FontSize,
                textBox1.FontStyle, textBox1.FontWeight);
            if (height > 526)
            {
                textBox1.Height = height;
            }

        }

        private double heightCalculator(String str, FontFamily font, double size, 
            FontStyle fontstyle, FontWeight fontweight )
        {
                TextBlock l = new TextBlock();
                l.FontFamily = font;
                l.FontSize = size;
                l.FontStyle = fontstyle;
                l.FontWeight = fontweight;
                l.Text = str;
                return l.ActualHeight + 70;
        }
    }


}