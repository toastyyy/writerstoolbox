﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
using System.Collections.ObjectModel;
namespace WritersToolbox.viewmodels
{
    class MemoryNoteViewModel : ObservableCollection<MemoryNoteViewModel>
    {
        private WritersToolboxDatebase db;

        public MemoryNoteViewModel () 
        {
            try
            {
                db = WritersToolboxDatebase.getInstance();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        public void save(MemoryNote obj_memoryNote)
        {
            //obj_memoryNote in DataContext hinzufügen.
            //db.GetTable<MemoryNote>.InsertOnSubmit(obj_memoryNote);

            //Änderung in der Datenbank übertragen.
            db.SubmitChanges();
        }

        public void saveAs(MemoryNote obj_memoryNote)
        {

        }


    }
}
