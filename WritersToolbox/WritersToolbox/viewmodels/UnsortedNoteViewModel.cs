using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;

namespace WritersToolbox.viewmodels
{
    public class UnsortedNoteViewModel
    {
        public int memoryNoteID { get; set; }
        public string title { get; set; }
        public string contents { get; set; }
        public string updatedNote { get; set; }
        private int count;
        private WritersToolboxDatebase db;
        private Table<MemoryNote> memoryNote;
        private MemoryNote obj_memoryNote;

        public UnsortedNoteViewModel()
        {
            try
            {
                db = WritersToolboxDatebase.getInstance();
                memoryNote = db.GetTable<MemoryNote>();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public ObservableCollection<UnsortedNoteViewModel> getObservableColletion()
        {
            count = 0;
            var t = memoryNote.Where(x => !x.associated && !x.deleted);
            ObservableCollection<UnsortedNoteViewModel> unsortedNote_List = new ObservableCollection<UnsortedNoteViewModel>();
            foreach (var item in t)
            {
                unsortedNote_List.Add(new UnsortedNoteViewModel()
                {
                    title = ((MemoryNote)item).title
                    ,
                    contents = ((MemoryNote)item).contentText.Substring(0, ((MemoryNote)item).contentText.Length > 15 ? 15 : ((MemoryNote)item).contentText.Length) + " ..."
                    ,
                    updatedNote = new DateTime(((MemoryNote)item).updatedDate.Year,((MemoryNote)item).updatedDate.Month,((MemoryNote)item).updatedDate.Day).ToShortDateString()
                    ,
                    memoryNoteID = ((MemoryNote)item).memoryNoteID
                });
                count++;
            }
            return unsortedNote_List;
        }

        public int getNumberOfUnsortedNote()
        {
            this.getObservableColletion();
            return count;
        }

        public void deleteUnsortedNote(ObservableCollection<UnsortedNoteViewModel> list)
        {
            foreach (UnsortedNoteViewModel item in list)
            {
                obj_memoryNote = db.GetTable<MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == item.memoryNoteID);
                obj_memoryNote.deleted = true;
            }

            db.SubmitChanges();
        }
    }
}
