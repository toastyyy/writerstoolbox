using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
using WritersToolbox.Resources;
namespace WritersToolbox.viewmodels
{
    public class EventDetailViewModel : INotifyPropertyChanged
    {
        private WritersToolboxDatebase wtb = null;
        private Table<Event> tableEvents = null;
        private Table<Chapter> tableChapter = null;
        private Table<MemoryNote> tableNotes = null;
        private Table<TypeObject> tableTypeObjects = null;
        private Table<MemoryNote> tableMemoryNote;
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
            tableMemoryNote = wtb.GetTable<MemoryNote>();
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
                    datawrapper.Type wrappedType = new datawrapper.Type()
                    {
                        typeID = to.obj_Type.typeID,
                        color = to.obj_Type.color,
                        imageString = to.obj_Type.imageString,
                        title = to.obj_Type.title,
                    };
                    var notes = from n in tableMemoryNote
                                where n.obj_TypeObject.typeObjectID == to.typeObjectID
                                select n;
                    ObservableCollection<datawrapper.MemoryNote> listNotes = new ObservableCollection<datawrapper.MemoryNote>();
                    foreach(var n in notes)
                    {
                        datawrapper.MemoryNote wrappedNotes = new datawrapper.MemoryNote()
                        {
                            memoryNoteID = n.memoryNoteID
                        };
                        listNotes.Add(wrappedNotes);
                    }
                    
                    typeObjects.Add(
                        new datawrapper.TypeObject()
                        {
                            color = to.color,
                            imageString = to.imageString,
                            name = to.name,
                            notes = listNotes,
                            type = wrappedType,
                            typeObjectID = to.typeObjectID,
                            used = to.used
                        });
                }
                typeObjects.Add(new datawrapper.TypeObject()
                {
                    typeObjectID = -1,
                    type = new datawrapper.Type() { typeID = -2 },
                    name = AppResources.EventAssignTypObject
                });

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

        public void updateFinaltext(String newText)
        {
            if (this.Event != null)
            {
                this.Event.finaltext = newText;
            }

            this.wtb.SubmitChanges();
            this.NotifyPropertyChanged("Event");
        }

        public void attachTypeObject(int toID, int eID)
        {
            Event Event = (from e in this.tableEvents
                              where e.eventID == eID
                              select e).Single();

            var TypeObject = (from to in this.tableTypeObjects
                                  where to.typeObjectID == toID
                                  select to).Single();

        }

        public void unassignTypeObject(int toID)
        {
            var typeObject = (from to in tableTypeObjects
                              where to.typeObjectID == toID
                              select to).Single();
        }

        public void unassignNote(int nID)
        {
            var note = (from n in tableMemoryNote
                        where n.memoryNoteID == nID
                        select n).Single();
            note.obj_Event = null;
            this.wtb.SubmitChanges();
            this.LoadData();
        }

        public void deleteNote(int nID)
        {
            var note = (from n in tableMemoryNote
                        where n.memoryNoteID == nID
                        select n).Single();
            note.deleted = true;
            note.obj_Event = null;
            this.wtb.SubmitChanges();
            this.LoadData();
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
