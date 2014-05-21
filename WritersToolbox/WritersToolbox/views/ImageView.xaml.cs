using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using WritersToolbox.models;
using WritersToolbox.datawrapper;
namespace WritersToolbox.views
{
    public partial class ImageView : PhoneApplicationPage
    {
        //Um das Bild zu speichern.
        private Image tempImage;

        /// <summary>
        /// Default Konstruktor, wo Control Image das geöffnete Bild bekommt.
        /// </summary>
        public ImageView()
        {
            InitializeComponent();
            //Fullscrenn Modus im Cach speichern.
            PhoneApplicationService.Current.State["OppendImageView"] = "true";
            tempImage = PhoneApplicationService.Current.State["imageView"] as Image;
            imageView.Source = tempImage.Source;
        }

        /// <summary>
        /// um gezeigtes Bild zu löschen, und zurück zu der Notiz.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            //Speichern im Cach, dass das Bild gelöscht ist.
            if (PhoneApplicationService.Current.State.ContainsKey("deletedImages"))
            {
                string cachImages = (PhoneApplicationService.Current.State["deletedImages"] as string);
                cachImages += ((MyImage)tempImage.DataContext).path + "|";
                PhoneApplicationService.Current.State["deletedImages"] = cachImages;
            }
            else
            {
                string cachImages = ((MyImage)tempImage.DataContext).path + "|";
                PhoneApplicationService.Current.State["deletedImages"] = cachImages;
            }

            PhoneApplicationService.Current.State.Remove("imageView");
            NavigationService.GoBack();
        }

        /// <summary>
        /// ZurückButton des Handys überschreiben.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            PhoneApplicationService.Current.State.Remove("imageView");
            
            NavigationService.GoBack();
        }
    }
}