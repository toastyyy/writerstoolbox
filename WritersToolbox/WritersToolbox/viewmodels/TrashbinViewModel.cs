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

        public ObservableCollection<Object> DeletedObjects { get; set; }
        public TrashbinViewModel()
        {
            try
            {
                db = WritersToolboxDatebase.getInstance();
                tableMemoryNote = db.GetTable<MemoryNote>();
                tableBook = db.GetTable<Book>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void loadData()
        {
            this.DeletedObjects = new ObservableCollection<object>();
            
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
        }

        public void deleteTrash(ObservableCollection<TrashbinViewModel> list)
        {
           
        }


        public void restoreTrash(ObservableCollection<TrashbinViewModel> list)
        {
        }

    }
}
