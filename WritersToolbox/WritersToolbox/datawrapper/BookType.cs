using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.datawrapper
{
    public class BookType
    {
        public int bookTypeID { get; set; }
        public String name { get; set; }
        public int numberOfChapter { get; set; }
        public DateTime addedDate { get; set; }
        public DateTime updatedDate { get; set; }
    }
}
