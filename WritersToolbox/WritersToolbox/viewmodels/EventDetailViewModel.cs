using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
namespace WritersToolbox.viewmodels
{
    public class EventDetailViewModel : INotifyPropertyChanged
    {
        private WritersToolboxDatebase wtb = null;
        private Table<Event> tableEvents = null;
        private Table<Chapter> tableChapter = null;
        private Table<MemoryNote> tableNotes = null;
        private Table<TypeObject> tableTypeObjects = null;
        private int event_id = -1;

        public datawrapper.Event Event = null;
        public EventDetailViewModel(int event_id) {
            this.wtb = WritersToolboxDatebase.getInstance();
            if (event_id > 0) {
                this.event_id = event_id;
            }
            this.tableEvents = this.wtb.GetTable<Event>();
            this.tableChapter = this.wtb.GetTable<Chapter>();
            this.tableNotes = this.wtb.GetTable<MemoryNote>();
            this.tableTypeObjects = this.wtb.GetTable<TypeObject>();
        }

        public void LoadData() {
            if (this.event_id == -1)
            {
                this.Event = new datawrapper.Event();
            }
            else {
                Event sqlEvent = (from e in this.tableEvents
                                  where e.eventID == this.event_id
                                  select e).Single();

                this.Event = new datawrapper.Event()
                {
                    eventID = sqlEvent.eventID,
                    deleted = sqlEvent.deleted,
                    orderInChapter = sqlEvent.orderInChapter,
                    title = sqlEvent.title,
                    finaltext = sqlEvent.finaltext
                };

                // kapitel besorgen
                var sqlChapter = (from c in this.tableChapter
                                  where c.chapterID == sqlEvent.fk_chapterID
                                  select c).Single();

                this.Event.chapter = new datawrapper.Chapter()
                {
                    addedDate = sqlChapter.addedDate,
                    chapterID = sqlChapter.chapterID,
                    chapterNumber = sqlChapter.chapterNumber,
                    deleted = sqlChapter.deleted,
                    title = sqlChapter.title,
                    tome = null,
                    updatedDate = sqlChapter.updatedDate
                };

                // typobjekte holen

                var sqlTypeObjects = (from to in this.tableTypeObjects
                                      where to.events.Any(e => e.fk_eventID == this.event_id)
                                      select to);

                ObservableCollection<datawrapper.TypeObject> typeObjects = new ObservableCollection<datawrapper.TypeObject>();

                foreach (var to in sqlTypeObjects)
                {
                    typeObjects.Add(
                        new datawrapper.TypeObject()
                        {
                            color = to.color,
                            imageString = to.imageString,
                            name = to.name,
                            notes = null,
                            type = null,
                            typeObjectID = to.typeObjectID,
                            used = to.used
                        });
                }

                this.Event.typeObjects = typeObjects;

                // notizen besorgen

                var sqlMemoryNotes = (from mn in this.tableNotes
                                      where mn.obj_Event.eventID == this.event_id
                                      select mn);

                ObservableCollection<datawrapper.MemoryNote> memoryNotes = new ObservableCollection<datawrapper.MemoryNote>();

                foreach (var mn in sqlMemoryNotes)
                {
                    memoryNotes.Add(new datawrapper.MemoryNote()
                    {
                        addedDate = mn.addedDate,
                        associated = true,
                        contentAudioString = mn.contentAudioString,
                        contentImageString = mn.ContentImageString,
                        contentText = mn.contentText,
                        deleted = mn.deleted,
                        location = mn.location,
                        tags = mn.tags,
                        title = mn.title,
                        updatedDate = mn.updatedDate,
                        memoryNoteID = mn.memoryNoteID
                    });
                }

                this.Event.notes = memoryNotes;
            }
            

            this.NotifyPropertyChanged("Event");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
