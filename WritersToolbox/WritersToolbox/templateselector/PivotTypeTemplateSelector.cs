using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace WritersToolbox.templateselector
{
    /// <summary>
    /// Verwendet fuer mehrere ItemTemplates. z.B. bei Typen, um zu unterscheiden, ob es sich um ein Typ
    /// handelt, oder ob ein neuer Typ hinzugefuegt werden soll.
    /// </summary>
    public class PivotTypeTemplateSelector: TemplateSelector
    {
        public DataTemplate addType { get; set; }
        public DataTemplate viewTypes { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item.GetType().IsAssignableFrom((new datawrapper.Type()).GetType()))
            {
                return this.SelectTypeTemplate((datawrapper.Type)item);
            }

            return null;
        }

        private DataTemplate SelectTypeTemplate(datawrapper.Type t)
        {
            if (t.typeID == -1)
            {
                return addType;
            }
            else
            {
                return viewTypes;
            }
        }
    }
}
