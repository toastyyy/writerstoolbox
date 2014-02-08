using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WritersToolbox.converter
{
    public class TypeObjectImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            datawrapper.TypeObject to = (datawrapper.TypeObject)value;

            String imgPath = (to.imageString == null || to.imageString.Equals("")) ? to.type.imageString : to.imageString;
            if (imgPath != null && !imgPath.Equals(""))
            {
                BitmapImage bmp = this.loadImageFromIsolatedStorage(imgPath);
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

        private BitmapImage loadImageFromIsolatedStorage(String path) {
            Stream stream = null;
            BitmapImage logo = new BitmapImage();
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(path))
                {
                    stream = isoStore.OpenFile(path, System.IO.FileMode.Open, FileAccess.Read);
                    try
                    {
                        logo.SetSource(stream);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            return logo;
        }
    }
}
