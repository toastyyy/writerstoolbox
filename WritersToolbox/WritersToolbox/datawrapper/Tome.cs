using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.datawrapper
{
    public class Tome
    {
        public int tomeID { get; set; }
        public String title { get; set; }
        public int tomeNumber { get; set; }
        public DateTime addedDate { get; set; }
        public DateTime updatedDate { get; set; }
        public List<Chapter> chapters { get; set; }
        public Book book { get; set; }
        public Boolean deleted { get; set; }
    }
}
