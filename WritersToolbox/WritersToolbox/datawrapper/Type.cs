using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<TypeObject> typeObjects { get; set; }

        public Type() 
        {
            typeObjects = new ObservableCollection<TypeObject>();
        }

        /// <summary>
        /// Prüft ob dieser Typ identisch mit einem anderen ist.
        /// </summary>
        /// <param name="obj">Zu prüfender Typ</param>
        /// <returns>Genau dann true, wenn die typeID bei beiden Objekten übereinstimmt.</returns>
        public override bool Equals(object obj)
        {
            return (obj.GetType().IsAssignableFrom(new Type().GetType())
                && this.typeID == ((Type)obj).typeID);
        }
    }
}
