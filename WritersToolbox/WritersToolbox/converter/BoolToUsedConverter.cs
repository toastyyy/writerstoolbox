using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WritersToolbox.Resources;

namespace WritersToolbox.converter
{
    /// <summary>
    /// Konverter-Klasse, die den Boolean Wert für verwendete Notizen in einen kurzen Text konvertiert.
    /// </summary>
    public class BoolToUsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            string used = "";
            if ((bool)value)
            {
                used = AppResources.ConverterUsed;
            }
            else if(parameter != null && parameter.Equals("showUnused"))
            {
                used = AppResources.ConverterUnused;
            }  
            return used;
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            
            throw new NotImplementedException();
        }
    }
}
