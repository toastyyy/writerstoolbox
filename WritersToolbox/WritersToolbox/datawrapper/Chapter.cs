using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.datawrapper
{
    public class Chapter
    {
        public int chapterID { get; set; }
        public string title { get; set; }
        public int chapterNumber { get; set; }
        public DateTime addedDate { get; set; }
        public DateTime updatedDate { get; set; }
        public ObservableCollection<Event> events { get; set; }
        public Tome tome { get; set; }
        public bool deleted { get; set; }

        public Chapter()
        {
            events = new ObservableCollection<Event>();
        }

    }
}
