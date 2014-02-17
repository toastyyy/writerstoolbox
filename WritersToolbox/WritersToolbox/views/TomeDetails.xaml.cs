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
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace WritersToolbox.views
{
    public partial class TomeDetails : PhoneApplicationPage
    {
        //Verbindung zwischen View und Model des Bandes.
        private TomeDetailsViewModel tome_VM;
        //Primäreschlüßel des bandes.
        private int tomeID;
        //Ablage des InformationsCode.
        private int informationCode;

        public TomeDetails()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Beim Navigieren zu dieser Seite wird das ausgewählte Objekt aus
        /// dem Navigationskontext herausgefiltert und die Details dazu mit dem
        /// Viewmodel geladen.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("tomeID"))
            {
                tomeID = int.Parse(NavigationContext.QueryString["tomeID"]);
                tome_VM = new TomeDetailsViewModel(tomeID);
                DataContext = tome_VM;
                int code = tome_VM.getInformation();
                switch (code)
                {
                    case 1:
                        rbNumberOfChapter.IsChecked = true;
                        break;
                    case 2:
                        rbNumberOfEvent.IsChecked = true;
                        break;
                    case 3:
                        rbNumberOfTypeObject.IsChecked = true;
                        break;
                    case 400:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 0;
                        fontSizeList.SelectedIndex = 0;
                        break;
                    case 410:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 1;
                        fontSizeList.SelectedIndex = 0;
                        break;
                    case 420:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 2;
                        fontSizeList.SelectedIndex = 0;
                        break;
                    case 430:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 3;
                        fontSizeList.SelectedIndex = 0;
                        break;
                    case 440:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 4;
                        fontSizeList.SelectedIndex = 0;
                        break;
                    case 401:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 0;
                        fontSizeList.SelectedIndex = 1;
                        break;
                    case 402:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 0;
                        fontSizeList.SelectedIndex = 2;
                        break;
                    case 403:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 0;
                        fontSizeList.SelectedIndex = 3;
                        break;
                    case 411:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 1;
                        fontSizeList.SelectedIndex = 1;
                        break;
                    case 412:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 1;
                        fontSizeList.SelectedIndex = 2;
                        break;
                    case 413:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 1;
                        fontSizeList.SelectedIndex = 3;
                        break;
                    case 421:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 2;
                        fontSizeList.SelectedIndex = 1;
                        break;
                    case 422:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 2;
                        fontSizeList.SelectedIndex = 2;
                        break;
                    case 423:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 2;
                        fontSizeList.SelectedIndex = 3;
                        break;
                    case 431:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 3;
                        fontSizeList.SelectedIndex = 1;
                        break;
                    case 432:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 3;
                        fontSizeList.SelectedIndex = 2;
                        break;
                    case 433:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 3;
                        fontSizeList.SelectedIndex = 3;
                        break;
                    case 441:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 4;
                        fontSizeList.SelectedIndex = 1;
                        break;
                    case 442:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 4;
                        fontSizeList.SelectedIndex = 2;
                        break;
                    case 443:
                        rbNumberOfPage.IsChecked = true;
                        formatList.SelectedIndex = 4;
                        fontSizeList.SelectedIndex = 3;
                        break;
                    case 5:
                        rbNumberOfWord.IsChecked = true;
                        break;
                }
            }

            llstructure.ItemsSource = tome_VM.getStructure();
        }

        /// <summary>
        /// Wird unmittelbar aufgerufen, nachdem die Page entladen und nicht mehr 
        /// die aktuelle Quelle eines übergeordneten Frame ist.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //Ausgewählte Information in der Datenbank aktualisieren.
            tome_VM.updateInformation(informationCode);
        }

        /// <summary>
        /// Information Button wird geklickt
        ///     Bildquelle Information ändert sich -> checked
        ///     Bildquelle Typeobjekt und structure ändert sich -> unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void information_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            BitmapImage _bitmapimage = new BitmapImage(new Uri("/icons/info_checked.png", UriKind.RelativeOrAbsolute));
            information.Source = _bitmapimage;
            informationGrid.Visibility = Visibility.Visible;

            _bitmapimage = new BitmapImage(new Uri("/icons/struktur_unchecked.png", UriKind.RelativeOrAbsolute));
            structure.Source = _bitmapimage;
            structureGrid.Visibility = Visibility.Collapsed;

            _bitmapimage = new BitmapImage(new Uri("/icons/typen_unchecked.png", UriKind.RelativeOrAbsolute));
            typeObject.Source = _bitmapimage;
            TypeObjectGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// structure Button wird geklickt
        ///     Bildquelle structure ändert sich -> checked
        ///     Bildquelle Typobject und information ändert sich -> unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void structure_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BitmapImage _bitmapimage = new BitmapImage(new Uri("/icons/info_unchecked.png", UriKind.RelativeOrAbsolute));
            information.Source = _bitmapimage;
            informationGrid.Visibility = Visibility.Collapsed;

            _bitmapimage = new BitmapImage(new Uri("/icons/struktur_checked.png", UriKind.RelativeOrAbsolute));
            structure.Source = _bitmapimage;
            structureGrid.Visibility = Visibility.Visible;

            _bitmapimage = new BitmapImage(new Uri("/icons/typen_unchecked.png", UriKind.RelativeOrAbsolute));
            typeObject.Source = _bitmapimage;
            TypeObjectGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// typeObject Button wird geklickt
        ///     Bildquelle typeobjekt ändert sich -> checked
        ///     Bildquelle Information und structure ändert sich -> unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void typeObjekt_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BitmapImage _bitmapimage = new BitmapImage(new Uri("/icons/info_unchecked.png", UriKind.RelativeOrAbsolute));
            information.Source = _bitmapimage;
            informationGrid.Visibility = Visibility.Collapsed;

            _bitmapimage = new BitmapImage(new Uri("/icons/struktur_unchecked.png", UriKind.RelativeOrAbsolute));
            structure.Source = _bitmapimage;
            structureGrid.Visibility = Visibility.Collapsed;

            _bitmapimage = new BitmapImage(new Uri("/icons/typen_checked.png", UriKind.RelativeOrAbsolute));
            typeObject.Source = _bitmapimage;
            TypeObjectGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Öffnet Detailsansicht des Typeobjektes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void typeObject_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //typeObjektID von sender Object herauslesen.
            int typeObjectID = ((datawrapper.TypeObject)((Grid)sender).DataContext).typeObjectID;
            //TypeobjectID durch URL übergeben.
            NavigationService.Navigate(new Uri("/views/TypeObjectDetails2.xaml?typeObjectID=" + typeObjectID, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Auswahl Anzahl Kapitel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNumberOfChapter_Checked(object sender, RoutedEventArgs e)
        {
            informationCode = 1;
            numberInforamtionText.Text = "" + tome_VM.getNumberOfChapters();
            inforamtionText.Text = "Kapitel";
        }

        /// <summary>
        /// Auswahl Anzahl Ereignisse.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNumberOfEvent_Checked(object sender, RoutedEventArgs e)
        {
            informationCode = 2;
            numberInforamtionText.Text = "" + tome_VM.getNumberOfEvents();
            inforamtionText.Text = "Ereignis-se";
        }

        /// <summary>
        /// Auswahl Anzal Typeobjekte.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNumberOfTypeObject_Checked(object sender, RoutedEventArgs e)
        {
            informationCode = 3;
            numberInforamtionText.Text = "" + tome_VM.getNumberOfTypeObjects();
            inforamtionText.Text = "Typeobjekt-e";
        }

        /// <summary>
        /// Auswahl Anzahl Seiten.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNumberOfPage_Checked(object sender, RoutedEventArgs e)
        {
            if (informationCode > 399)
            {
                //Um die zweite Stelle herauszulesen.
                formatList.SelectedIndex = (int)(informationCode % 100 / 10);
                //Um die letzte Stelle herauszulesen.
                fontSizeList.SelectedIndex = (int)(informationCode % 10);
            }
            formatList.IsEnabled = true;
            fontSizeList.IsEnabled = true;
        }

        /// <summary>
        /// Auswahl Anzahl Seiten aufheben.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNumberOfPage_Unchecked(object sender, RoutedEventArgs e)
        {
            formatList.IsEnabled = false;
            fontSizeList.IsEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (formatList != null && fontSizeList != null)
            {
                informationCode = 400 + formatList.SelectedIndex * 10 + fontSizeList.SelectedIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fontSizeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (formatList != null && fontSizeList != null)
            {
                informationCode = 400 + formatList.SelectedIndex * 10 + fontSizeList.SelectedIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNumberOfWord_Checked(object sender, RoutedEventArgs e)
        {
            informationCode = 5;
        }

    }
}
