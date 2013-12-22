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
            return null;
        }

        public List<models.Type> getAllTypes()
        {
            return this.tableType.ToList<models.Type>();
        }

        public void createType(String title, String color, String image)
        {
            models.Type t = new models.Type();
            t.title = title;
            t.color = color;
            t.imageString = image;
            this.tableType.InsertOnSubmit(t);
            this.db.SubmitChanges();
        }

        public void createTypeObject(String name, String color, String image)
        {
        }
    }
}
