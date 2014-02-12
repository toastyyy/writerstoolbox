using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.datawrapper
{
    public class UnsortedMemoryNote
    {
        //Information der Notiz für die List der unsortierten Notizen.
        public int memoryNoteID { get; set; }
        public string title { get; set; }
        public string contents { get; set; }
        public DateTime addedDate { get; set; }
    }
}
