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

        

        static uint[] uintColors =
{ 
	0xFFFFE135,0xFFFFFF66,0xFF008A00,0xFF32CD32,0xFF00FF7F,0xFF808000,0xFFFF0000,0xFFFF4500,
	0xFFFF8C00,
	0xFFFF7F50,0xFFDC143C,0xFFFF1493,0xFFB22222,0xFFC71585,
	0xFFDA70D6,0xFF000080,0xFF4B0082,
	0xFF800080,0xFF483D8B,0xFFADD8E6,0xFF20B2AA,0xFF008080
};

        private void ColorPickerPage_Loaded(object sender, RoutedEventArgs e)
        {
            List<ColorItem> item = new List<ColorItem>();
            for (int i = 0; i < uintColors.Length; i++)
            {
                item.Add(new ColorItem() {Color = ConvertColor(uintColors[i]) });
            };

            listBox.ItemsSource = item; //Fill ItemSource with all colors
        }

        private Color ConvertColor(uint uintCol)
        {
            byte A = (byte)((uintCol & 0xFF000000) >> 24);
            byte R = (byte)((uintCol & 0x00FF0000) >> 16);
            byte G = (byte)((uintCol & 0x0000FF00) >> 8);
            byte B = (byte)((uintCol & 0x000000FF) >> 0);

            return Color.FromArgb(A, R, G, B); ;
        }

        public class ColorItem
        {
            public Color Color { get; set; }
        }

        private void lstColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                //(Application.Current as App).CurrentColorItem = ((ColorItem)e.AddedItems[0]);
            }
        }

        public AddTypeObject()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ein neues Typobjekt wird erzeugt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveTypeObject(object sender, EventArgs e)
        {
            //Color c = slider.Color;

            //String r = c.R.ToString("X2");
            //String g = c.G.ToString("X2");
            //String b = c.B.ToString("X2");

            //String color = "#" + r + g + b;
            //String name = toName.Text;

            //try 
            //{
            //    Types.types_VM.createTypeObject(name, color, "", typeID);
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine("Objekt konnte nicht erstellt werden");
            //}

            NavigationService.GoBack();

        }

        /// <summary>
        /// Cancelt die Erstellungen eines TypObjekts und geht eine Seite zurück.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelTypeObject(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// Beim Navigieren zu dieser Seite wird der ausgewählte Typ aus
        /// dem Navigationskontext herausgefiltert um das zu erstellende neue
        /// Typobjekt dem Typ zuordnen zu können.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("typeID"))
            {
                var tID = NavigationContext.QueryString["typeID"];
                typeID = int.Parse(tID);
                Title.Text = "Neues Objekt";
            }
            else if (NavigationContext.QueryString.ContainsKey("item"))
            {
                var item = NavigationContext.QueryString["item"];
                var indexParsed = int.Parse(item);
                toName.Text = Types.Types_VM.getNameForTypeObject(indexParsed);
                //slider.Color = Types.Types_VM.getColorForTypeObject(indexParsed);
                Title.Text = "Objekt ändern";
            }
        }

    }
}