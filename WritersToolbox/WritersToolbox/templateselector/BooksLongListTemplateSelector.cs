using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WritersToolbox.datawrapper;

namespace WritersToolbox.templateselector
{
    /// <summary>
    /// Verwendet fuer mehrere ItemTemplates. z.B. bei Bände, um zu unterscheiden, ob es sich um ein Band
    /// handelt, oder ob ein neuer Band hinzugefuegt werden soll.
    /// </summary>
    public class BooksLongListTemplateSelector : TemplateSelector
    {
        public DataTemplate existingTome { get; set; }
        public DataTemplate newTome { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item.GetType().IsAssignableFrom((new Tome()).GetType())) {
                return this.SelectTomeTemplate((Tome)item);
            }

            return null;
        }

        private DataTemplate SelectTomeTemplate(Tome t) {
            if (t.tomeID == -1)
            {
                return newTome;
            }
            else {
                return existingTome;
            }
        }
    }
}
