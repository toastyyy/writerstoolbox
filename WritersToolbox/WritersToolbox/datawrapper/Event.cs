using System;
using System.Collections.Generic;
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
        public List<MemoryNote> notes { get; set; }
        public List<TypeObject> typeObjects { get; set; }
        public bool deleted { get; set; }

        public Event()
        {
            notes = new List<MemoryNote>();
            typeObjects = new List<TypeObject>();
        }
    }
}
