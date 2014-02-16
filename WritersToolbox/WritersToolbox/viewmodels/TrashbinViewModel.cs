using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
using System.Diagnostics;

namespace WritersToolbox.viewmodels
{
    public class TrashbinViewModel
    {
        private WritersToolboxDatebase db;
        private Table<MemoryNote> tableMemoryNote;
        private Table<Book> tableBook;
        private Table<Tome> tableTome;
        private Table<WritersToolbox.models.TypeObject> tableTypeObject;
        private Table<Event> tableEvent;

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
                tableEvent = db.GetTable<Event>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Collection<Object> loadData()
        {
            this.DeletedObjects = new ObservableCollection<Object>(); // O || o
            
            // notizen, die geloescht sind, aber nicht zugeordnet sind
            var sqlNotes = from n in tableMemoryNote
                           where n.associated == false && n.deleted == true
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
                    updatedDate = note.updatedDate
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

            var sqlType = from ty in tableTypeObject
                          where ty.deleted == true
                          select ty;
            foreach (var type in sqlType)
            {
                datawrapper.TypeObject ty = new datawrapper.TypeObject()
                {
                    typeObjectID = type.typeObjectID,
                //    notes = type.notes,
                    color = type.color,
                    name = type.name,
                  //  obj_Type = type.obj_Type;
                    imageString = type.imageString
                  //  ObservableCollection<MemoryNote> notes = type.note

                };
                this.DeletedObjects.Add(ty);
            }

            return DeletedObjects;
        }

        public void deleteTrash(ObservableCollection<object> list)
        {
           // foreach(
           //item.GetType().IsAssignableFrom((new datawrapper.TypeObject()).GetType()))
           //     obj_memoryNote = db.GetTable<MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == item.memoryNoteID);
           //     obj_memoryNote.deleted = true;
            
           // db.SubmitChanges();
            
        }


        public void restoreTrash(ObservableCollection<object> list)
        {

        }

    }
}
