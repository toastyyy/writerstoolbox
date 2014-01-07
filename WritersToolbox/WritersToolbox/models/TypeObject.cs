using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using System.Windows.Media;
using System.ComponentModel;

namespace WritersToolbox.models
{
    [Table(Name="TypeObjects")]
    class TypeObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        //um eine beschleunigte Ausführung der Datenänderung zu erreichen.
        [Column(IsVersion = true)]
        private Binary version;

        private int stg_typeObjectID;
        [Column(IsPrimaryKey = true,
            AutoSync = AutoSync.OnInsert,
            DbType = "INT IDENTITY",
            Storage = "stg_typeObjectID",
            IsDbGenerated = true)]
        public int typeObjectID
        {
            get { return stg_typeObjectID; }
            set
            {
                if (stg_typeObjectID != value)
                {
                    sendPropertyChanging("typeObjectID");
                    stg_typeObjectID = value;
                    sendPropertyChanged("typeObjectID");
                }
            }
        }

        [Column(Name = "fk_typeID")]
        public int fk_typeID;

        private EntityRef<Type> _type;

        [Association(Name = "FK_TypeObject_Type",
            Storage = "_type",         //Speicherort der Child-Instanzen.
            IsForeignKey = true,
            ThisKey = "fk_typeID",      //Name des Primärschlüssels.
            OtherKey = "typeID")] //Name des Fremdschlüssels.
        public Type obj_Type
        {
            get
            {
                return this._type.Entity;
            }
            set
            {
                sendPropertyChanging("type");
                this._type.Entity = value;
                sendPropertyChanged("type");
            }
        }

        //Datenbank optimierung
        //Benachrichtigt Clients, dass sich ein Eigenschaftswert ändert.
        public event PropertyChangingEventHandler PropertyChanging;
        protected void sendPropertyChanging(String propertyName)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        //Benachrichtigt Clients, dass ein Eigenschaftswert geändert wurde.
        public event PropertyChangedEventHandler PropertyChanged;
        protected void sendPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
