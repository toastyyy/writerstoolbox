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
    public class TypeObjectTemplateSelector: TemplateSelector
    {
        public DataTemplate addTypeObject { get; set; }
        public DataTemplate viewTypeObject { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item.GetType().IsAssignableFrom((new datawrapper.TypeObject()).GetType()))
            {
                return this.SelectTypeObjectTemplate((datawrapper.TypeObject)item);
            }

            return null;
        }

        private DataTemplate SelectTypeObjectTemplate(datawrapper.TypeObject t)
        {
            if (t.type.typeID == -2)
            {
                return addTypeObject;
            }
            else
            {
                return viewTypeObject;
            }
        }
    }
}
