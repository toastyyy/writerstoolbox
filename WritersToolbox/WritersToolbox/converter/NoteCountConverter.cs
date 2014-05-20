using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WritersToolbox.Resources;

namespace WritersToolbox.converter
{
    /// <summary>
    /// Zählt die Anzahl der Notizen in der übergebenen Collection mit MemoryNote Elementen.
    /// </summary>
    public class NoteCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            string noteCount = AppResources.ConverterNumberOfNotes;

            int count = (value != null) ? ((ObservableCollection<datawrapper.MemoryNote>)value).Count : 0;

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
