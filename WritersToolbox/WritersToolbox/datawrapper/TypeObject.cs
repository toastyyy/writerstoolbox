using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WritersToolbox.datawrapper
{
    public class TypeObject
    {
        public int typeObjectID { get; set; }
        public int fk_typeID { get; set; }
        public String name { get; set; }
        public String color { get; set;  }
        public String imageString { get; set; }
        public Boolean used { get; set; }
    }
}
