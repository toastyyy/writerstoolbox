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
    [Table(Name = "Booktypes")]
    class BookType 
    {

        [Column(IsPrimaryKey = true,
            AutoSync = AutoSync.OnInsert,
            DbType = "INT IDENTITY",
            IsDbGenerated = true)]
        public int bookTypeID { get; set; }

        [Column(CanBeNull = false)]
        public String name { get; set; }

        [Column(CanBeNull = false)]
        public int numberOfChapter { get; set; }

        
    }
}
