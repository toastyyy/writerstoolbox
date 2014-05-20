using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WritersToolbox.converter
{
    /// <summary>
    /// Kürzt einen String (hier Titel eines Events) so, dass dieser in der Oberfläche dargestellt werden kann. (20 Zeichen)
    /// </summary>
    public class EventCutTitel: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String titel = (String) value;
            if (titel.Length > 21)
            {
                titel = titel.Substring(0,19);
                titel += "...";
            }
            return titel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return value;
           // throw new NotImplementedException();
        }
    }

}

