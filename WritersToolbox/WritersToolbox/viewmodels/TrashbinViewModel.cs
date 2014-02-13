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
        private Table<MemoryNote> memoryNote;
        private Table<Book> book;
        
        public TrashbinViewModel()
        {
            try
            {
                db = WritersToolboxDatebase.getInstance();
                memoryNote = db.GetTable<MemoryNote>();
                book = db.GetTable<Book>();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void loadData()
        {
        }

        public int getNumberOfTrash()
        {
            this.getObservableColletion();
            return count;
        }

        public void deleteTrash(ObservableCollection<TrashbinViewModel> list)
        {
            foreach (TrashbinViewModel item in list)
            {
                // TrashID sagt aus ob es eine MemoryID ist oder ein Book.
                if (item.trashID == 1)
                {
                    obj_memoryNote = db.GetTable<MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == item.memoryNoteID);
                    db.GetTable<MemoryNote>().DeleteOnSubmit(obj_memoryNote);
                }
                else if(item.trashID == 2)
                {
                   // obj_book = db.GetTable<Book>().Single(book => book.bookID == item.bookID);
                   // db.GetTable<Book>().DeleteOnSubmit(obj_book);
                }
            }

            db.SubmitChanges();
        }


        public void restoreTrash(ObservableCollection<TrashbinViewModel> list)
        {
            foreach (TrashbinViewModel item in list)
            {
                if (item.trashID == 1)
                {
                    obj_memoryNote = db.GetTable<MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == item.memoryNoteID);
                    obj_memoryNote.deleted = false;
                }
                else if (item.trashID == 2)
                {
                  //  obj_book = db.GetTable<Book>().Single(book => book.bookID == item.bookID);
                  //  obj_book.deleted = false;
                }
            }

            db.SubmitChanges();
        }

    }
}
