using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.datawrapper
{
    public class MemoryNote
    {
        public int memoryNoteID { get; set; }
        public DateTime addedDate { get; set; }
        public DateTime updatedDate { get; set; }
        public String title { get; set; }
        public String contentText { get; set; }
        public String location { get; set; }
        public Boolean associated { get; set; }
        public Boolean deleted { get; set; }
        public String tags { get; set; }
        public int fk_eventID { get; set; }
        public int fk_typeObjectID { get; set; }
        public String contentImageString { get; set; }
        public String contentAudioString { get; set; }
    }
}
