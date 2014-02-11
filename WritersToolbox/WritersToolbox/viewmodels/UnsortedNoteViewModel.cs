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

        //Instanzvariablen.
        private WritersToolboxDatebase db;
        private Table<MemoryNote> memoryNote;
        private MemoryNote obj_memoryNote;

        /// <summary>
        /// Defaultkonstruktor.
        /// </summary>
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

        /// <summary>
        /// Liefert die unsortierten Notizen zurück.
        /// </summary>
        /// <returns>Unsortierte Notizen als ObservableCollection</returns>
        public ObservableCollection<datawrapper.UnsortedMemoryNote> getUnsortedNotes()
        {
            //Liste der Unsortierte Notizen und die Noch nicht gelöscht sind.
            var notizen = memoryNote.Where(x => !x.associated && !x.deleted);
            ObservableCollection<datawrapper.UnsortedMemoryNote> unsortedNote_List = new ObservableCollection<datawrapper.UnsortedMemoryNote>();
            foreach (var item in notizen)
            {
                unsortedNote_List.Add(new datawrapper.UnsortedMemoryNote()
                {
                    title = ((MemoryNote)item).title.Substring(0, ((MemoryNote)item).title.Length > 15 ? 15 : ((MemoryNote)item).title.Length)
                    ,
                    contents = ((MemoryNote)item).contentText.Substring(0, ((MemoryNote)item).contentText.Length > 27 ? 27 : ((MemoryNote)item).contentText.Length) 
                    ,
                    updatedNote = ((MemoryNote)item).updatedDate
                    ,
                    memoryNoteID = ((MemoryNote)item).memoryNoteID
                });

            }
            return unsortedNote_List;
        }

        /// <summary>
        /// Anzahl der unsortierten Notizen.
        /// </summary>
        /// <returns></returns>
        public int getNumberOfUnsortedNote()
        {
            return memoryNote.Where(x => !x.associated && !x.deleted).Count(); 
        }

        /// <summary>
        /// unsortierte Notizen löschen.
        /// </summary>
        /// <param name="list">List der unsortierten Notizen, die gelöscht werden müssen</param>
        public void deleteUnsortedNote(ObservableCollection<datawrapper.UnsortedMemoryNote> list)
        {
            foreach (datawrapper.UnsortedMemoryNote item in list)
            {
                obj_memoryNote = db.GetTable<MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == item.memoryNoteID);
                obj_memoryNote.deleted = true;
            }
            db.SubmitChanges();
        }
    }
}
