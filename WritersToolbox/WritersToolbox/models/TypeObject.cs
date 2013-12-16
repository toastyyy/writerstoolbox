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
        public TypeObject() 
        {
            _notes = new EntitySet<MemoryNote>();
            color = "0000ff"; // default farbe blau
        }

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

        private String stg_name;
        [Column(CanBeNull = false,
            Storage = "stg_name")]
        public String name
        {
            get { return stg_name; }
            set
            {
                if (stg_name != value)
                {
                    sendPropertyChanging("name");
                    stg_name = value;
                    sendPropertyChanged("name");
                }
            }
        }

        private String stg_color;
        [Column(CanBeNull = false,
            Storage = "stg_color")]
        public String color
        {
            get { return stg_color; }
            set
            {
                if (stg_color != value)
                {
                    sendPropertyChanging("color");
                    stg_color = value;
                    sendPropertyChanged("color");
                }
            }
        }

        private String stg_imageString;
        [Column(CanBeNull = true,
            Storage = "stg_imageString")]
        public String imageString
        {
            get { return stg_imageString; }
            set
            {
                if (stg_imageString != value)
                {
                    sendPropertyChanging("imageString");
                    stg_imageString = value;
                    sendPropertyChanged("imageString");
                }
            }
        }

        private Boolean stg_used;
        [Column(CanBeNull = false,
            Storage = "stg_used")]
        public Boolean used
        {
            get { return stg_used; }
            set
            {
                if (stg_used != value)
                {
                    sendPropertyChanging("used");
                    stg_used = value;
                    sendPropertyChanged("used");
                }
            }
        }

        private EntitySet<MemoryNote> _notes;

        [Association(Name = "TypeObject_Notes",
            Storage = "_notes",         //Speicherort der Child-Instanzen.
            ThisKey = "typeObjectID",      //Name des Primärschlüssels.
            OtherKey = "fk_typeObjectID")] //Name des Fremdschlüssels.
        public EntitySet<MemoryNote> notes
        {
            get
            {
                return this._notes;
            }
            set
            {
                sendPropertyChanging("notes");
                this._notes.Assign(value);
                sendPropertyChanged("notes");
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
