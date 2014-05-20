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
    /// Konvertiert aus den Daten eines TypeObject s einen kurzen Untertitel, der den Titel des Typs und die Anzahl der Notizen enthält.
    /// Beispiel: "Charakter, 10 Notizen"
    /// </summary>
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
