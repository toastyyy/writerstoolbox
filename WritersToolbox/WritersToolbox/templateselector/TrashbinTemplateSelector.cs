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
        public DataTemplate tomeTemplate { get; set; }
        public DataTemplate memoryNoteTemplate { get; set; }
        public DataTemplate eventTemplate { get; set; }
        public DataTemplate typeObjectTemplate { get; set; }
        public DataTemplate typeTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item.GetType().IsAssignableFrom((new datawrapper.TypeObject()).GetType()))
            {
                return typeObjectTemplate;
            }
            else if (item.GetType().IsAssignableFrom((new datawrapper.MemoryNote()).GetType())) 
            {
                return memoryNoteTemplate;
            }
            else if (item.GetType().IsAssignableFrom((new datawrapper.Book()).GetType()))
            {
                return bookTemplate;
            }
            else if (item.GetType().IsAssignableFrom((new datawrapper.Tome()).GetType()))
            {
                return tomeTemplate;
            }
            else if (item.GetType().IsAssignableFrom((new datawrapper.Type()).GetType()))
            {
                return typeTemplate;
            }
            else if (item.GetType().IsAssignableFrom((new datawrapper.Event()).GetType()))
            {
                return eventTemplate;
            }
            
            


            return null;
        }
    }
}
