using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
namespace WritersToolbox.viewmodels
{
    class TypesViewModel
    {
        private WritersToolboxDatebase db;
        private Table<models.Type> tableType;
        private Table<TypeObject> tableTypeObject;
        public TypesViewModel()
        {
            this.db = WritersToolboxDatebase.getInstance();
            this.tableType = this.db.GetTable<models.Type>();
            this.tableTypeObject = this.db.GetTable<TypeObject>();
        }

        public int[] getAllTypeIDs() 
        {
            var result = from t in tableType select t.typeID;
            return result.ToArray();
        }

        public int[] getAllTypeObjectIDsForTypeID(int typeID) 
        {
            var result = from t in tableTypeObject
                         where t.fk_typeID == typeID
                         select t.typeObjectID;
            
            return result.ToArray();
        }

        public Color getColorForType(int typeID) 
        {
            var result = from t in tableType
                         where t.typeID == typeID
                         select t.color;

            return fromHexToColor(result.FirstOrDefault());
        }

        public String getTitleForType(int typeID) 
        {
            var result = from t in tableType
                         where t.typeID == typeID
                         select t.title;

            return result.First();
        }

        public Image getImageForType(int typeID) 
        {
            String imagePath = (from t in tableType
                         where t.typeID == typeID
                         select t.imageString).FirstOrDefault();

            if(imagePath.Equals("")) 
            {
                return null;
            } else {
                BitmapImage bi = new BitmapImage(new Uri(imagePath));
                Image img = new Image();
                img.Source = bi;
                return img;
            }
            
        }

        public String getNameForTypeObject(int typeObjectID)
        {
            return (from to in tableTypeObject
                    where to.typeObjectID == typeObjectID
                    select to.name).FirstOrDefault();
        }

        public int[] getNoteIDsForTypeObject(int typeObjectID)
        {
            var result = (from to in tableTypeObject
                    where to.typeObjectID == typeObjectID
                    select to.notes).FirstOrDefault();

            int[] retVar = new int[result.Count()];
            int index = 0;
            foreach (var row in result)
            {
                retVar[index] = ((MemoryNote)row).memoryNoteID;
                index++;
            }

            return retVar;
        }

        public Image getImageForTypeObject(int typeObjectID)
        {
            String imagePath = (from to in tableTypeObject
                                where to.typeObjectID == typeObjectID
                                select to.imageString).FirstOrDefault();

            if (imagePath.Equals(""))
            {
                return null;
            }
            else
            {
                BitmapImage bi = new BitmapImage(new Uri(imagePath));
                Image img = new Image();
                img.Source = bi;
                return img;
            }
        }

        public void createType(String title, String color, String image)
        {
            models.Type t = new models.Type();
            if (title.Equals(""))
                throw new ArgumentException("Titel muss ausgefüllt sein", "Title");
            t.title = title;
            t.color = color;
            t.imageString = image;
            this.tableType.InsertOnSubmit(t);
            this.db.SubmitChanges();
        }

        public void createTypeObject(String name, String color, String image, int typeID)
        {
            models.Type type = (from t in tableType
                         where t.typeID == typeID
                         select t).FirstOrDefault();

            if (name.Equals("")) 
            {
                throw new ArgumentException("Name muss angegeben werden", "name");
            }
            if (type == null) 
            {
                throw new ArgumentException("TypeObject muss einem Typ angehören", "type");
            }
            TypeObject to = new TypeObject();
            to.name = name;
            if (color.Equals(""))
            {
                to.color = type.color;
            }
            else 
            {
                to.color = color;
            }
            to.obj_Type = type;

            this.tableTypeObject.InsertOnSubmit(to);
            this.db.SubmitChanges();
        }

        private Color fromHexToColor(String hex)
        {
            Byte colorR = Convert.ToByte(hex.Substring(0, 2));
            Byte colorG = Convert.ToByte(hex.Substring(2, 2));
            Byte colorB = Convert.ToByte(hex.Substring(4, 2));

            return Color.FromArgb(255, colorR, colorG, colorB);
        }
    }
}
