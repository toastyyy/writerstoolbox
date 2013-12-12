using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using System.Windows.Media;
using System.ComponentModel;

namespace WritersToolbox.entity
{
    [Table(Name = "Tomes")]
    class Tome 
    {
        [Column(IsPrimaryKey = true,
            AutoSync = AutoSync.OnInsert,
            DbType = "INT IDENTITY",
            IsDbGenerated = true)]
        public int tomeID {get; set;}

        [Column(CanBeNull = false)]
        public String title { get; set; }

        public Color color { get; set; }

        [Column(CanBeNull = false)]
        public int tomeNumber { get; set; }

        [Column(Name = "fk_bookID")]
        private int? fk_bookID;

        private EntityRef<Book> _books = new EntityRef<Book>();

        [Association(Name = "FK_Tome_Book",
            IsForeignKey = true,
            Storage = "_books",
            ThisKey = "fk_bookID")]
        public Book obj_book
        {
            get { return _books.Entity; }
            set { _books.Entity = value; }
        }

    }
}
