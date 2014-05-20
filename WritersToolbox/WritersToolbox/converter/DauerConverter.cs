using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WritersToolbox.Resources;

namespace WritersToolbox.converter
{
    /// <summary>
    /// Konvertiert eine TimeSpan in einen darstellbaren String, der eine Dauer angibt (Minuten:Sekunden).
    /// </summary>
    public class DauerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan val = (TimeSpan)value;
            String pre = "";
            return AppResources.ConverterDuration + val.ToString(@"mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
