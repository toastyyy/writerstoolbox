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

        /// <summary>
        /// Ein neues Typobjekt wird erzeugt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                slider.Color = Types.Types_VM.getColorForTypeObject(indexParsed);
                Title.Text = "Objekt ändern";
            }
        }

    }
}