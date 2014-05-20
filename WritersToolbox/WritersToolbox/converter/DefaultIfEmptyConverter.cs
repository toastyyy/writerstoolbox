using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WritersToolbox.converter
{
    /// <summary>
    /// Gibt, falls das zu konvertierende Objekt ein Leerstring oder null ist, einen Defaultwert aus dem angegebenen 
    /// Konverter-Parameter zurück. Ansonsten wird das zu konvertierende Objekt zurückgegeben.
    /// </summary>
    public class DefaultIfEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String val = (String)value;

            if (val == null || val.Equals("")) {
                val = (String)parameter;
            }

            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
