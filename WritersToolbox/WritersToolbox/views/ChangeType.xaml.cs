using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media;

namespace WritersToolbox.views
{
    public partial class ChangeType : PhoneApplicationPage
    {
        public ChangeType()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Ein neuer Typ wird erzeugt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveType(object sender, EventArgs e)
        {
            Color c = slider.Color;

            String r = c.R.ToString("X2");
            String g = c.G.ToString("X2");
            String b = c.B.ToString("X2");

            String color = "#" + r + g + b;
            String title = tTitle.Text;

            try
            {
                Types.types_VM.createType(title, color, "");
                NavigationService.GoBack();
            }
            catch (ArgumentException ae)
            {
                MessageBox.Show(ae.Message, "Fehler", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Cancelt die Erstellungen eines Typs und geht eine Seite zurück.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelType(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("item"))
            {
                var item = NavigationContext.QueryString["item"];
                var indexParsed = int.Parse(item);
                tTitle.Text = Types.Types_VM.getTitleForType(indexParsed);
                slider.Color = Types.Types_VM.getColorForType(indexParsed);
            }


        }
    }
}