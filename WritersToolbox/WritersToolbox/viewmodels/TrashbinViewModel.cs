using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using WritersToolbox.views;
using System.IO.IsolatedStorage;

namespace WritersToolbox.viewmodels
{
    /// <summary>
    /// Die TrashbinViewModel Klasse bzw. Präsentations-Logik ist eine aggregierte Datenquelle,
    /// die verschiedene Daten bereithält und Methoden zur Löschung und Wiederherstellung bietet.
    /// </summary>
    public class TrashbinViewModel : INotifyPropertyChanged
    {
        private WritersToolboxDatebase db;
        private Table<MemoryNote> tableMemoryNote;
        private Table<Book> tableBook;
        private Table<Tome> tableTome;
        private Table<WritersToolbox.models.TypeObject> tableTypeObject;
        private Table<WritersToolbox.models.Type> tableType;
        private Table<Event> tableEvent;
        private Table<Chapter> tableChapter;
        private Table<EventTypeObjects> tableEventTypeObject;
        

        public ObservableCollection<Object> DeletedObjects { get; set; }
        /// <summary>
        /// Konstruktur die Tabellen aus der Datenbank vorinitialisiert.
        /// </summary>
        public TrashbinViewModel()
        {
            try
            {
                db = WritersToolboxDatebase.getInstance();
                tableMemoryNote = db.GetTable<MemoryNote>();
                tableBook = db.GetTable<Book>();
                tableTome = db.GetTable<Tome>();
                tableTypeObject = db.GetTable<WritersToolbox.models.TypeObject>();
                tableType = db.GetTable<WritersToolbox.models.Type>();
                tableEvent = db.GetTable<Event>();
                tableEventTypeObject = db.GetTable<EventTypeObjects>();
                tableChapter = db.GetTable<Chapter>();

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void MN_AlreadyExists()
        {

        }
        /// <summary>
        /// Methode die die relevante Daten aus der Datenbank holt und in eine Liste speichert.
        /// </summary>
        public void loadDeletedMemoryNotes()
        {
            // notizen, die geloescht sind, aber nicht zugeordnet sind
            var sqlNotes = from n in tableMemoryNote
                           where n.deleted == true
                           select n;
            foreach (var note in sqlNotes)
            {
                datawrapper.MemoryNote mn = new datawrapper.MemoryNote()
                {
                    addedDate = note.addedDate,
                    associated = false,
                    contentAudioString = note.contentAudioString,
                    contentImageString = note.ContentImageString,
                    contentText = note.contentText,
                    deleted = true,
                    location = note.location,
                    tags = note.tags,
                    title = note.title,
                    updatedDate = note.updatedDate,
                    memoryNoteID = note.memoryNoteID
                };
                this.DeletedObjects.Add(mn);
            }
        }
        /// <summary>
        /// Methode die die relevante Daten aus der Datenbank holt und in eine Liste speichert.
        /// </summary>
        public void loadDeletedBooks()
        {
            // buecher, die geloescht sind
            var sqlBooks = from b in tableBook
                           where b.deleted == true
                           select b;
            foreach (var book in sqlBooks)
            {
                datawrapper.Book b = new datawrapper.Book()
                {

                    updatedDate = book.updatedDate,
                    addedDate = book.addedDate,
                    name = book.name,
                    bookID = book.bookID
                };
                this.DeletedObjects.Add(b);
            }
        }
        /// <summary>
        /// Methode die die relevante Daten aus der Datenbank holt und in eine Liste speichert.
        /// </summary>
        public void loadDeletedTomes()
        {
            // Tomes, die geloescht sind.
            var sqlTomes = from t in tableTome
                           where t.deleted == true
                           select t;
            foreach (var tome in sqlTomes)
            {
                datawrapper.Tome t = new datawrapper.Tome()
                {
                    // book = tome.book,
                    // chapters =  tome.chapters,
                    tomeNumber = tome.tomeNumber,
                    updatedDate = tome.updatedDate,
                    tomeID = tome.tomeID,
                    title = tome.title,
                    addedDate = tome.addedDate
                };
                this.DeletedObjects.Add(t);
            }
        }
        /// <summary>
        /// Methode die die relevante Daten aus der Datenbank holt und in eine Liste speichert.
        /// </summary>
        public void loadDeletedTypeObjects()
        {
            //typeObjecte die geloescht sind
            var sqlTypeObject = from tO in tableTypeObject
                                where tO.deleted == true
                                select tO;
            foreach (var typeObject in sqlTypeObject)
            {
                datawrapper.TypeObject tO = new datawrapper.TypeObject()
                {

                    typeObjectID = typeObject.typeObjectID,
                    //type = typeObject.type,
                    //notes = typeObject.notes,
                    color = typeObject.color,
                    used = typeObject.used,
                    name = typeObject.name,

                    imageString = typeObject.imageString

                };
                this.DeletedObjects.Add(tO);
            }
        }
        /// <summary>
        /// Methode die die relevante Daten aus der Datenbank holt und in eine Liste speichert.
        /// </summary>
        public void loadDeletedTypes()
        {
            //Komplette Typen die gelöscht sind.
            var sqlType = from ty in tableType
                          where ty.deleted == true
                          select ty;
            foreach (var type in sqlType)
            {
                datawrapper.Type ty = new datawrapper.Type()
                {

                    typeID = type.typeID,
                    title = type.title,
                    color = type.color,
                    imageString = type.imageString,
                    //   typeObjects = type.typeObjects

                };
                this.DeletedObjects.Add(ty);
            }
            this.NotifyPropertyChanged("DeletedObjects");
        }

        /// <summary>
        /// Methode die die relevante Daten aus der Datenbank holt und in eine Liste speichert.
        /// </summary>
        public void loadDeletedEvents()
        {
             //Komplette Events die gelöscht sind.
            var sqlEvent = from te in tableEvent
                                where te.deleted == true
                                select te;
            foreach (var eevent in sqlEvent)
            {
                datawrapper.Event te = new datawrapper.Event()
                {
                   
                    eventID = eevent.eventID,
                    title = eevent.title,

                };
                this.DeletedObjects.Add(te);
            }
        }

        /// <summary>
        /// Ruft die obigen Methoden auf.
        /// </summary>
        public void loadData()
        {

            this.DeletedObjects = new ObservableCollection<Object>(); // O || o

            loadDeletedMemoryNotes();
            loadDeletedBooks();
            loadDeletedTomes();
            loadDeletedTypeObjects();
            loadDeletedTypes();
            loadDeletedEvents();

        }
        /// <summary>
        /// Löscht die Liste.
        /// </summary>
        public void deleteList()
        {
            this.DeletedObjects.Clear();
        }
        /// <summary>
        /// Löscht die ausgewählten und alle darunterliegenden Daten aus der Datenbank.
        /// </summary>
        public void deleteTrash(IList list)
        {
            IEnumerator enumerator = list.GetEnumerator();
            while (enumerator.MoveNext()) {
                object entry = enumerator.Current;
                // loeschen start
                if (entry.GetType().IsAssignableFrom((new datawrapper.MemoryNote()).GetType()))
                {
                    
                    datawrapper.MemoryNote mn = (datawrapper.MemoryNote)entry;
                    var entries = (from m in this.tableMemoryNote
                                   where m.memoryNoteID == mn.memoryNoteID
                                   select m).Single();
                    Debug.WriteLine(mn.fk_eventID);
                    Debug.WriteLine(mn.fk_typeObjectID);
                    if ((mn.fk_eventID == 0) && (mn.fk_typeObjectID == 0))
                    {
                        Debug.WriteLine("Notiz war in unsortierte Notizen drin");
                    }
                    if(mn.contentAudioString != null)
                    {
                        //models.MemoryNote _m = wtb.GetTable<models.MemoryNote>().Single(_mn => _mn.memoryNoteID == id);
                        string[] tokens = mn.contentAudioString.Split('|');

                        using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            foreach (string item in tokens)
                            {
                                if (isoStore.FileExists(item))
                                {
                                    isoStore.DeleteFile(item);
                                }
                            }
                        }
                    }
                    
                    this.tableMemoryNote.DeleteOnSubmit(entries);
                }

                if (entry.GetType().IsAssignableFrom((new datawrapper.Book()).GetType()))
                {
                    datawrapper.Book bk = (datawrapper.Book)entry;
                    var entries = (from b in this.tableBook
                                   where b.bookID == bk.bookID
                                   select b).Single();



                    this.tableBook.DeleteOnSubmit(entries);
                }

                if (entry.GetType().IsAssignableFrom((new datawrapper.Tome()).GetType()))
                {
                    datawrapper.Tome to = (datawrapper.Tome)entry;
                    var tome = (from t in this.tableTome
                                   where t.tomeID == to.tomeID
                                   select t).Single();

                    var chapter = (from c in this.tableChapter
                                   where c.obj_tome.tomeID == tome.tomeID
                                   select c).ToList();

                    foreach (var ch in chapter)
                    {
                        var even = (from e in this.tableEvent
                                       where e.fk_chapterID == ch.chapterID
                                       select e).ToList();
                        foreach (var eve in even)
                        {
                            var notes = (from n in this.tableMemoryNote
                                       where n.obj_Event.eventID == eve.eventID
                                       select n).ToList();
                            foreach (var note in notes)
                            {
                                note.obj_Event = null;
                            }
                            var eto = (from t in this.tableEventTypeObject
                                      where t.fk_eventID == eve.eventID
                                      select t).ToList();
                            foreach (var evento in eto)
                            {
                                this.tableEventTypeObject.DeleteOnSubmit(evento);
                            }
                            this.tableEvent.DeleteOnSubmit(eve);
                        }
                        this.tableChapter.DeleteOnSubmit(ch);
                    }

                    this.tableTome.DeleteOnSubmit(tome);
                }
                if (entry.GetType().IsAssignableFrom((new datawrapper.Event()).GetType()))
                {
                    datawrapper.Event ev = (datawrapper.Event)entry;
                    var entries = (from e in this.tableEvent
                                   where e.eventID == ev.eventID
                                   select e).Single();
                    foreach (var n in entries.notes)
                    {
                        tableMemoryNote.DeleteOnSubmit(n);
                    }
                    var tos = from to in this.tableEventTypeObject
                              where to.fk_eventID == entries.eventID
                              select to;
                    foreach (var t in tos)
                    {
                        tableEventTypeObject.DeleteOnSubmit(t);
                    }
                    entries.typeObjects.Clear();
                    this.tableEvent.DeleteOnSubmit(entries);
                }
                if (entry.GetType().IsAssignableFrom((new datawrapper.Type()).GetType()))
                {
                    datawrapper.Type ty = (datawrapper.Type)entry;
                    var entries = (from t in this.tableType
                                   where t.typeID == ty.typeID
                                   select t).Single();
                    this.tableType.DeleteOnSubmit(entries);
                }
                if (entry.GetType().IsAssignableFrom((new datawrapper.TypeObject()).GetType()))
                {
                    datawrapper.TypeObject tyo = (datawrapper.TypeObject)entry;

                    var to = (from t in this.tableTypeObject
                                   where t.typeObjectID == tyo.typeObjectID
                                   select t).Single();
                    var entries = (from t in this.tableMemoryNote
                                   where t.obj_TypeObject.typeObjectID == to.typeObjectID
                                   select t).ToList();
                    
                    foreach (var n in entries)
                    {
                        tableMemoryNote.DeleteOnSubmit(n);
                    }
                    var tos = from x in this.tableEventTypeObject
                              where x.obj_typeObject.typeObjectID == to.typeObjectID
                              select x;
                    foreach (var t in tos)
                    {
                        tableEventTypeObject.DeleteOnSubmit(t);
                    }
                    
                   
                    this.tableTypeObject.DeleteOnSubmit(to);
                }
                if (entry.GetType().IsAssignableFrom((new datawrapper.Chapter()).GetType()))
                {
                    datawrapper.Chapter ca = (datawrapper.Chapter)entry;
                    var entries = (from c in this.tableChapter
                                   where c.chapterID == ca.chapterID
                                   select c).Single();
                    this.tableChapter.DeleteOnSubmit(entries);
                }
                

                // loeschen ende
            }
            this.db.SubmitChanges();
            this.loadData();
        }

        /// <summary>
        /// Bei den ausgewählten Daten wird der delete-Flag wieder entfernt. 
        /// </summary>
        public void restoreTrash(IList list)
        {
            IEnumerator enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object entry = enumerator.Current;
                if (entry.GetType().IsAssignableFrom((new datawrapper.MemoryNote()).GetType()))
                {
                    datawrapper.MemoryNote mn = (datawrapper.MemoryNote)entry;
                    var entries = (from m in this.tableMemoryNote
                                   where m.memoryNoteID == mn.memoryNoteID
                                   select m).Single();
                    entries.deleted = false;
                }
                if (entry.GetType().IsAssignableFrom((new datawrapper.Type()).GetType()))
                {
                    datawrapper.Type ty = (datawrapper.Type)entry;
                    var entries = (from t in this.tableType
                                   where t.typeID == ty.typeID
                                   select t).Single();
                    entries.deleted = false;
                }
                if (entry.GetType().IsAssignableFrom((new datawrapper.TypeObject()).GetType()))
                {
                    datawrapper.TypeObject to = (datawrapper.TypeObject)entry;
                    var entries = (from t in this.tableTypeObject
                                   where t.typeObjectID == to.typeObjectID
                                   select t).Single();
                    entries.deleted = false;
                }
                if (entry.GetType().IsAssignableFrom((new datawrapper.Event()).GetType()))
                {
                    datawrapper.Event ev = (datawrapper.Event)entry;
                    var entries = (from t in this.tableEvent
                                   where t.eventID == ev.eventID
                                   select t).Single();
                    //int index = db.GetTable<models.Event>().Count(_e => _e.obj_Chapter.chapterID == ev.chapter.chapterID && !_e.deleted);
                    //entries.orderInChapter = index + 1;
                    entries.deleted = false;
                }
                if (entry.GetType().IsAssignableFrom((new datawrapper.Book()).GetType()))
                {
                    datawrapper.Book bo = (datawrapper.Book)entry;
                    var entries = (from b in this.tableBook
                                   where b.bookID == bo.bookID
                                   select b).Single();
                    entries.deleted = false;
                }
                if (entry.GetType().IsAssignableFrom((new datawrapper.Tome()).GetType()))
                {
                    datawrapper.Tome to = (datawrapper.Tome)entry;
                    var entries = (from t in this.tableTome
                                   where t.tomeID == to.tomeID
                                   select t).Single();
                    entries.deleted = false;
                }
                if (entry.GetType().IsAssignableFrom((new datawrapper.Chapter()).GetType()))
                {
                    datawrapper.Chapter ca = (datawrapper.Chapter)entry;
                    var entries = (from c in this.tableChapter
                                   where c.chapterID == ca.chapterID
                                   select c).Single();
                    //int index = db.GetTable<models.Chapter>().Count(_c => _c.obj_tome.tomeID == ca.tome.tomeID && !_c.deleted);
                    //entries.chapterNumber = index + 1;
                    entries.deleted = false;
                    
                }
                
            }
                this.db.SubmitChanges();
                this.loadData();
            
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
