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
    /// Gibt das Bild, dass dem angegebenen Typobjekt zugehört, zurück.
    /// Hat das TypObjekt kein eigenes Bild, wird das des Typs genommen.
    /// </summary>
    public class TypeObjectImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage bmp = null;
            datawrapper.TypeObject to = (datawrapper.TypeObject)value;
            StringToImageConverter stic = new StringToImageConverter();
            object o = stic.Convert(to.imageString, targetType, parameter, culture);
            if (o != null) { 
                bmp = (BitmapImage) o;
            }

            if (bmp == null) {
                o = stic.Convert(to.type.imageString, targetType, parameter, culture);
                if(o == null) return null;
                bmp = (BitmapImage)o;
            }

            return bmp;
            /*
            String imgPath = (to.imageString == null || to.imageString.Equals("")) ? to.type.imageString : to.imageString;
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
            }*/
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
