using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WritersToolbox.datawrapper
{
    public class Type
    {
        public String title { get; set; }
        public String color { get; set; }
        public String imageString { get; set; }
        public int typeID { get; set; }
        public List<TypeObject> typeObjects { get; set; }

        public Type() 
        {
            typeObjects = new List<TypeObject>();
        }
    }
}
