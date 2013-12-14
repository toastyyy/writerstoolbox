using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.entity;
using System.Collections.ObjectModel;
using WritersToolbox.Database;
namespace WritersToolbox.viewmodels
{
    class MemoryNoteViewModel : ObservableCollection<MemoryNoteViewModel>
    {
        private WritersToolboxDatebase db;

        public MemoryNoteViewModel () 
        {
            try
            {
                db = new WritersToolboxDatebase();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        public void save(MemoryNote obj_memoryNote)
        {
            //obj_memoryNote in DataContext hinzufügen.
            db.getTableMemoryNote().InsertOnSubmit(obj_memoryNote);

            //Änderung in der Datenbank übertragen.
            db.SubmitChanges();
        }

        public void saveAs(MemoryNote obj_memoryNote)
        {

        }


    }
}
