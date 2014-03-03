﻿using System;
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

namespace WritersToolbox.viewmodels
{
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
        

        public ObservableCollection<Object> DeletedObjects { get; set; }
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
                tableChapter = db.GetTable<Chapter>();

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void loadData()
        {
            
            this.DeletedObjects = new ObservableCollection<Object>(); // O || o
            
            // notizen, die geloescht sind, aber nicht zugeordnet sind
            var sqlNotes = from n in tableMemoryNote
                           where n.deleted == true
                           select n;
            foreach(var note in sqlNotes) {
                datawrapper.MemoryNote mn = new datawrapper.MemoryNote() {
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
            // buecher, die geloescht sind
            var sqlBooks = from b in tableBook
                          where b.deleted == true
                          select b;
            foreach(var book in sqlBooks) {
                datawrapper.Book b = new datawrapper.Book()
                {     
                    
                    updatedDate = book.updatedDate,
                    addedDate = book.addedDate,
                    name = book.name,
                    bookID = book.bookID
                };
                this.DeletedObjects.Add(b);
            }


            // Tomes, die geloescht sind.
            var sqlTomes = from t in tableTome
                           where t.deleted == true
                           select t;
            foreach (var tome in sqlTomes)
            {
                datawrapper.Tome t = new datawrapper.Tome()
                {
                    //     book = tome.book,
                    //     chapters =  tome.chapters,
                    tomeNumber = tome.tomeNumber,
                    updatedDate = tome.updatedDate,
                    tomeID = tome.tomeID,
                    title = tome.title,
                    addedDate = tome.addedDate
                };
                this.DeletedObjects.Add(t);
            }
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
                    // notes = typeObject.notes,
                    color = typeObject.color,
                    used = typeObject.used,
                    name = typeObject.name,
                  
                    imageString = typeObject.imageString

                };
                this.DeletedObjects.Add(tO);
            }
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
                    var entries = (from t in this.tableTome
                                   where t.tomeID == to.tomeID
                                   select t).Single();
                    this.tableTome.DeleteOnSubmit(entries);
                }
                if (entry.GetType().IsAssignableFrom((new datawrapper.Event()).GetType()))
                {
                    datawrapper.Event ev = (datawrapper.Event)entry;
                    var entries = (from e in this.tableEvent
                                   where e.eventID == ev.eventID
                                   select e).Single();
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
                    var entries = (from t in this.tableTypeObject
                                   where t.typeObjectID == tyo.typeObjectID
                                   select t).Single();
                    this.tableTypeObject.DeleteOnSubmit(entries);
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
