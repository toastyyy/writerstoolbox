using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.datawrapper
{
    public class Book
    {
        public int bookID { get; set; }
        public String name { get; set; }
        public DateTime addedDate { get; set; }
        public DateTime updatedDate { get; set; }
        public List<Tome> tomes { get; set; }
        public BookType bookType { get; set; }
    }
}
