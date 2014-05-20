using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WritersToolbox.converter
{
    /// <summary>
    /// Kürzt den Inhalt einer Notiz auf eine Länge, die bei wenig Platz darstellbar ist. (40 Zeichen)
    /// Fügt am Ende "..." hinzu.
    /// </summary>
    public class NoteShortContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            string content = (String) value;
            if (content.Length > 40)
            {
                content = content.Substring(0, 40) + "...";
            }
            
                
            return content;
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            
            throw new NotImplementedException();
        }
    }
}
