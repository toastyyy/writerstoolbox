using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.IO;
using Windows.Storage;
namespace WritersToolbox.Database
{
    [Database(Name = "WTDB")]
    class WritersToolboxDatebase : DataContext
    {
        static readonly string AppPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "ToDo.sdf");
        //static readonly string DbPath = Path.Combine(AppPath, "Data", "database.accdb");
        //static readonly string STR_CONNECTION = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + DbPath + "';Persist Security Info=False;";
        static readonly String tt = "Data Source=isostore:/ToDo.sdf";
        //Deklaration der Tabellen.
        private Table<entity.BookType> t_bookType;
        private Table<entity.Book> t_book;
        private Table<entity.Tome> t_tome;
        private Table<entity.Chapter> t_chapter;
        private Table<entity.Event> t_Event;
        private Table<entity.TypeObject> t_typeObject;
        private Table<entity.Type> t_type;
        private Table<entity.MemoryNote> t_memoryNote;

        public Table<entity.MemoryNote>  getTableMemoryNote()
        {
            return t_memoryNote;
        }
        public WritersToolboxDatebase() : base(tt) { }

        
    }
}
