using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WritersToolbox.templateselector
{
    public class TrashbinTemplateSelector : TemplateSelector
    {
        public DataTemplate bookTemplate { get; set; }
        public DataTemplate memoryNoteTemplate { get; set; }
        public DataTemplate eventTemplate { get; set; }
        public DataTemplate typeObjectTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item.GetType().IsAssignableFrom((new datawrapper.TypeObject()).GetType()))
            {
                return typeObjectTemplate;
            }
            else if (item.GetType().IsAssignableFrom((new datawrapper.MemoryNote()).GetType())) {
                return memoryNoteTemplate;
            }

            return null;
        }
    }
}
