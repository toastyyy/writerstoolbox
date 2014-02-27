using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WritersToolbox.Resources;
namespace WritersToolbox.converter
{
    public class TypeObjectSubtitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            datawrapper.TypeObject to = (datawrapper.TypeObject)value;

            return to.type.title.ToUpper() + ", " + to.notes.Count + AppResources.ConverterSubtitleNotes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
