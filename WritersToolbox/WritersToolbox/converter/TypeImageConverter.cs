using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media.PhoneExtensions;

namespace WritersToolbox.converter
{
    /// <summary>
    /// Gibt das Bild, dass dem angegebenen Typ zugeordnet wurde, zurück.
    /// </summary>
    public class TypeImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            datawrapper.Type t = (datawrapper.Type)value;

            String imgPath = (t.imageString == null || t.imageString.Equals("")) ? "" : t.imageString;
            if (imgPath != null && !imgPath.Equals(""))
            {
                BitmapImage bmp = this.loadImageFromStorage(imgPath);
                if (bmp != null)
                {
                    return bmp;
                }
                else {
                    return "";
                }
            }
            else {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private BitmapImage loadImageFromStorage(String path) {
            MediaLibrary ml = new MediaLibrary();
            BitmapImage bi = new BitmapImage();
            try
            {            
                Picture pic = ml.Pictures.Where(p => p.GetPath().Equals(path)).Single();
                bi.SetSource(pic.GetThumbnail());
            }
            catch (InvalidOperationException e) {
                bi = new BitmapImage(new Uri(path, UriKind.Relative));
            }

            return bi;
        }
    }
}
