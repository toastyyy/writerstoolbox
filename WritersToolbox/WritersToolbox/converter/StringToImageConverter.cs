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
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String path = (String)value;
            BitmapImage bmp = this.loadImageFromStorage(path);
            if (bmp != null) return bmp;
            bmp = this.loadImageFromIsolatedStorage(path);
            if (bmp != null) return bmp;

            var resource = App.GetResourceStream(new Uri(String.Format(@"{0}", path), UriKind.Relative));
            //var buffer = new byte[resource.Stream.Length];
            //resource.Stream.Read(buffer, 0, buffer.Length);
            resource.Stream.Position = 0;
            bmp = new BitmapImage();
            bmp.SetSource(resource.Stream);
            return bmp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private BitmapImage loadImageFromIsolatedStorage(String path) {
            BitmapImage bi = new BitmapImage();

            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(path))
                {
                    using (var fileStream = isoStore.OpenFile(path, FileMode.Open))
                    {
                        bi.SetSource(fileStream);
                        return bi;
                    }
                }
            }
            return null;
        }

        private BitmapImage loadImageFromStorage(String path)
        {
            MediaLibrary ml = new MediaLibrary();
            BitmapImage bi = new BitmapImage();
            try
            {
                Picture pic = ml.Pictures.Where(p => p.GetPath().Equals(path)).Single();
                bi.SetSource(pic.GetThumbnail());
            }
            catch (InvalidOperationException e)
            {
                bi = null;
            }

            return bi;
        }
    }
}
