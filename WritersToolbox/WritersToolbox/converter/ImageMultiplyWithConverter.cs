using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WritersToolbox.converter
{
    /// <summary>
    /// Multipliziert das Bild aus dem angegebenen Pfad mit einer Farbe. Gibt das dadurch bearbeitete Bild zurueck.
    /// </summary>
    public class ImageMultiplyWithConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Color c = Color.FromArgb(255, 255, 0, 0);
            if (value != null) {
                try { 
                    String hex = ((String)value).Replace("#", "");
                    Byte a = System.Convert.ToByte(hex.Substring(0, 2), 16);
                    Byte r = System.Convert.ToByte(hex.Substring(2, 2), 16);
                    Byte g = System.Convert.ToByte(hex.Substring(4, 2), 16);
                    Byte b = System.Convert.ToByte(hex.Substring(6, 2), 16);
                    c = Color.FromArgb(a, r, g, b);                
                }
                catch (Exception e) {
                    c = Color.FromArgb(255, 255, 0, 0);
                }

            }

            WriteableBitmap wb = this.multiplicateImageWithColor((String) parameter, c);
            return wb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Wendet einen Multiplikationsfilter auf das angewendete Bild an. 
        /// ACHTUNG: Funktioniert nur hinreichend bei Bildern in Graustufen.
        /// </summary>
        /// <param name="fileName">Dateiname. Objekt muss als Datei im Projekt mit der Build Action 'Content' vorhanden sein.</param>
        /// <param name="c">Anzuwendende Farbe für die Überlagerung</param>
        /// <returns>Neues Bild mit angewendetem Filter</returns>
        private WriteableBitmap multiplicateImageWithColor(String fileName, Color c)
        {
            var file = System.Windows.Application.GetResourceStream(new Uri(fileName, UriKind.Relative));
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(file.Stream);
            WriteableBitmap wb = new WriteableBitmap(bmp);


            for (int x = 0; x < wb.PixelWidth; x++)
            {
                for (int y = 0; y < wb.PixelHeight; y++)
                {
                    Byte brightness = wb.GetBrightness(x, y);
                    Color newColor = new Color();
                    newColor.A = wb.GetPixel(x, y).A;
                    newColor.R = (byte)(c.R * (brightness / 255.0));
                    newColor.G = (byte)(c.G * (brightness / 255.0));
                    newColor.B = (byte)(c.B * (brightness / 255.0));
                    wb.SetPixel(x, y, newColor);
                }
            }
            return wb;
        }
    }
}
