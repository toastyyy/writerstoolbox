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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Phone.Controls;

namespace WritersToolbox.viewmodels
{
    class TypeDetailViewModel
    {
        private WritersToolboxDatebase wtb;
        private Table<TypeObject> tableTypeObject;
        private datawrapper.TypeObject typeObject;

        public datawrapper.TypeObject TypeObject {
            get { return typeObject; }
            set { typeObject = value; }
        }

        public TypeDetailViewModel(int id) 
        {
            wtb = WritersToolboxDatebase.getInstance();
            tableTypeObject = wtb.GetTable<TypeObject>();

            var v = from to in tableTypeObject
                    where to.typeObjectID == id
                    select to;

            var o = v.Single();

            typeObject = new datawrapper.TypeObject() 
            { 
                color = o.color,
                fk_typeID = o.fk_typeID,
                imageString = o.imageString,
                typeObjectID = o.typeObjectID,
                used = o.used,
                name = o.name
            };
        }
    }
}
