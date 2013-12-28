﻿using System;
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
    [Table(Name="Types")]
    public class Type : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public Type() {
            _typeObjects = new EntitySet<TypeObject>();
        }
        //um eine beschleunigte Ausführung der Datenänderung zu erreichen.
        [Column(IsVersion = true)]
        private Binary version;

        private int stg_typeID;
        [Column(IsPrimaryKey = true,
            AutoSync = AutoSync.OnInsert,
            DbType = "INT IDENTITY",
            Storage = "stg_typeID",
            IsDbGenerated = true)]
        public int typeID
        {
            get { return stg_typeID; }
            set
            {
                if (stg_typeID != value)
                {
                    sendPropertyChanging("typeID");
                    stg_typeID = value;
                    sendPropertyChanged("typeID");
                }
            }
        }

        private String stg_title;
        [Column(Name = "title",
            Storage = "stg_title")]
        public String title
        {
            get { return stg_title; }
            set 
            {
                sendPropertyChanging("title");
                stg_title = value;
                sendPropertyChanged("title");   
            }

        }

        private String stg_color;
        [Column(Name = "color",
            Storage = "stg_color")]
        public String color
        {
            get { return stg_color; }
            set
            {
                sendPropertyChanging("color");
                stg_color = value;
                sendPropertyChanged("color");
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

        private EntitySet<TypeObject> _typeObjects;

        [Association(Name = "Type_TypeObjects",
            Storage = "_typeObjects",         //Speicherort der Child-Instanzen.
            ThisKey = "typeID",      //Name des Primärschlüssels.
            OtherKey = "fk_typeID")] //Name des Fremdschlüssels.
        public EntitySet<TypeObject> typeObjects
        {
            get
            {
                return this._typeObjects;
            }
            set
            {
                sendPropertyChanging("typeObjects");
                this._typeObjects.Assign(value);
                sendPropertyChanged("typeObjects");
            }
        }

        private Boolean stg_deleted;
        [Column(Storage = "stg_deleted")]
        public Boolean deleted
        {
            get { return stg_deleted; }
            set
            {
                if (stg_deleted != value)
                {
                    sendPropertyChanging("deleted");
                    stg_deleted = value;
                    sendPropertyChanged("deleted");
                }
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
