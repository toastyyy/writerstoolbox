using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.datawrapper
{
    public class Event
    {
        public int eventID { get; set; }
        public string title { get; set; }
        public int orderInChapter { get; set; }
        public Chapter chapter { get; set; }
        public ObservableCollection<MemoryNote> notes { get; set; }
        public ObservableCollection<TypeObject> typeObjects { get; set; }
        public bool deleted { get; set; }
        public string finaltext { get; set; }
        public Event()
        {
            notes = new ObservableCollection<MemoryNote>();
            typeObjects = new ObservableCollection<TypeObject>();
        }
    }
}
