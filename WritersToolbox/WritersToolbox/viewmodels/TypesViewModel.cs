using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
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
        public List<TypeObject> getTypeObjectsForType(models.Type t)
        {
            var result = from to in tableTypeObject
                         where to.obj_Type.Equals(t)
                         select to;
            List<TypeObject> l = new List<TypeObject>();
            foreach (var row in result) 
            {
                l.Add(row);
            }
            return l;
        }

        public List<models.Type> getAllTypes()
        {
            return this.tableType.ToList<models.Type>();
        }

        public models.Type getType(int id) 
        {
            var result = from t in tableType
                         where t.typeID == id
                         select t;
            return result.First();
        }

        public TypeObject getTypeObject(int id)
        {
            var result = from t in tableTypeObject
                         where t.typeObjectID == id
                         select t;
            return result.First();
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

        public void createTypeObject(String name, String color, String image, models.Type type)
        {
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
    }
}
