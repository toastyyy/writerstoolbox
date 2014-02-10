﻿using Microsoft.Xna.Framework.Media;
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
    public class TypeObjectImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            datawrapper.TypeObject to = (datawrapper.TypeObject)value;

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
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private BitmapImage loadImageFromStorage(String path) {
            MediaLibrary ml = new MediaLibrary();
            Picture pic = ml.Pictures.Where(p => p.GetPath().Equals(path)).Single();
            BitmapImage bi = new BitmapImage();
            bi.SetSource(pic.GetThumbnail());
            return bi;
        }
    }
}
