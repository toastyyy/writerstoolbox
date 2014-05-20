using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using WritersToolbox.Resources;

namespace WritersToolbox.converter
{
    /// <summary>
    /// Konvertiert ein DateTime - Objekt in einen String, der das Datum im Format der derzeit festgelegten Nationalität
    /// zurückgibt.
    /// </summary>
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime val = (DateTime)value;

            return AppResources.ConverterBookCreationDate + val.ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
