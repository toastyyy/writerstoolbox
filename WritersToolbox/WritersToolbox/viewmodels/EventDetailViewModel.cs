using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
namespace WritersToolbox.viewmodels
{
    public class EventDetailViewModel
    {
        private WritersToolboxDatebase wtb = null;
        private Table<Event> tableEvents = null;
        private Table<Chapter> tableChapter = null;
        private Table<MemoryNote> tableNotes = null;
        private int event_id = -1;

        public datawrapper.Event Event = null;
        public EventDetailViewModel(int event_id) {
            if (event_id > 0) {
                this.event_id = event_id;
            }
            this.tableEvents = this.wtb.GetTable<Event>();
            this.tableChapter = this.wtb.GetTable<Chapter>();
            this.tableNotes = this.wtb.GetTable<MemoryNote>();
        }

        public void LoadData() { 
            Event sqlEvent = (from e in this.tableEvents
                               where e.eventID == this.event_id
                               select e).Single();

            this.Event = new datawrapper.Event() { 
                eventID = sqlEvent.eventID,
                deleted = sqlEvent.deleted
            };
        }


    }
}
