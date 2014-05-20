using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WritersToolbox.converter
{
    /// <summary>
    /// Prüft, ob eine Liste (ICollection) Elemente enthält oder nicht. Gibt einen dementsprechenden Boolean-Wert zurück.
    /// </summary>
    public class CollectionHasElementsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ICollection c = (ICollection)value;
            return c.Count > 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
