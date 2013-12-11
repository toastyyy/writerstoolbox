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
    [Table(Name = "Books")]
    class Book
    {
        [Column(IsPrimaryKey = true,
            AutoSync = AutoSync.OnInsert,
            DbType = "INT IDENTITY",
            IsDbGenerated = true)]
        public int bookID { get; set; }

        [Column(CanBeNull = false)]
        public String name { get; set; }

        public Color color { get; set; }

        [Column(Name = "fk_bookTypeID")]
        private int? fk_bookTypeID;

        private EntityRef<BookType> _booktypes = new EntityRef<BookType>();

        [Association(Name = "FK_Book_BookType",
            IsForeignKey = true,
            Storage = "_booktypes",
            ThisKey = "fk_bookTypeID")]
        public BookType obj_bookType
        {
            get { return _booktypes.Entity; }
            set { _booktypes.Entity = value; }
        }

    }
}
