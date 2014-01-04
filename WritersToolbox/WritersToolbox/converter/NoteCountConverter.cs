using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WritersToolbox.converter
{
    public class NoteCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            string noteCount = "Anzahl Notizen: ";
            int count = ((List<datawrapper.MemoryNote>)value).Count;

            noteCount += count;
            return noteCount;
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {

            throw new NotImplementedException();
        }
    }
}
