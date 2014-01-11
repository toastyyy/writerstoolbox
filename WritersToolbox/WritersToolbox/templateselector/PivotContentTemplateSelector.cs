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
    /// Verwendet fuer mehrere ItemTemplates. z.B. bei Buch, um zu unterscheiden, ob es sich um ein Buch
    /// handelt, oder ob ein neues Buch hinzugefuegt werden soll.
    /// </summary>
    public class PivotContentTemplateSelector : TemplateSelector
    {
        public DataTemplate addBook { get; set; }
        public DataTemplate viewBook { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item.GetType().IsAssignableFrom((new Book()).GetType())) {
                return this.SelectBookTemplate((Book)item);
            }

            return null;
        }

        private DataTemplate SelectBookTemplate(Book b) {
            if (b.bookID == -1)
            {
                return addBook;
            }
            else {
                return viewBook;
            }
        }
    }
}
