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

// TODO: Tags, contentImage, contentAudio
namespace WritersToolbox.models
{
    [Table(Name="Notes")]
    class MemoryNote : INotifyPropertyChanging, INotifyPropertyChanged
    {
        //um eine beschleunigte Ausführung der Datenänderung zu erreichen.
        [Column(IsVersion = true)]
        private Binary version;

        private int stg_memoryNoteID;
        [Column(IsPrimaryKey = true,
            AutoSync = AutoSync.OnInsert,
            DbType = "INT IDENTITY",
            Storage = "stg_memoryNoteID",
            IsDbGenerated = true)]
        public int memoryNoteID
        {
            get { return stg_memoryNoteID; }
            set
            {
                if (stg_memoryNoteID != value)
                {
                    sendPropertyChanging("memoryNoteID");
                    stg_memoryNoteID = value;
                    sendPropertyChanged("memoryNoteID");
                }
            }
        }

        private DateTime stg_addedDate;
        [Column(CanBeNull = false,
            Storage = "stg_addedDate")]
        public DateTime addedDate
        {
            get { return stg_addedDate; }
            set
            {
                if (stg_addedDate != value)
                {
                    sendPropertyChanging("addedDate");
                    stg_addedDate = value;
                    sendPropertyChanged("addedDate");
                }
            }
        }

        private DateTime stg_updatedDate;
        [Column(CanBeNull = false,
            Storage = "stg_addedDate")]
        public DateTime updatedDate
        {
            get { return stg_updatedDate; }
            set
            {
                if (stg_updatedDate != value)
                {
                    sendPropertyChanging("updatedDate");
                    stg_updatedDate = value;
                    sendPropertyChanged("updatedDate");
                }
            }
        }

        private String stg_title;
        [Column(CanBeNull = false,
            Storage = "stg_title")]
        public String title
        {
            get { return stg_title; }
            set
            {
                if (stg_title != value)
                {
                    sendPropertyChanging("title");
                    stg_title = value;
                    sendPropertyChanged("title");
                }
            }
        }

        private String stg_contentText;
        [Column(CanBeNull = false,
            Storage = "stg_contentText")]
        public String contentText
        {
            get { return stg_contentText; }
            set
            {
                if (stg_contentText != value)
                {
                    sendPropertyChanging("contentText");
                    stg_contentText = value;
                    sendPropertyChanged("contentText");
                }
            }
        }

        private String stg_location;
        [Column(CanBeNull = false,
            Storage = "stg_location")]
        public String location
        {
            get { return stg_location; }
            set
            {
                if (stg_location != value)
                {
                    sendPropertyChanging("location");
                    stg_location = value;
                    sendPropertyChanged("location");
                }
            }
        }

        private Boolean stg_associated;
        [Column(Storage = "stg_associated")]
        public Boolean associated
        {
            get { return stg_associated; }
            set 
            {
                if (stg_associated != value)
                {
                    sendPropertyChanging("associated");
                    stg_associated = value;
                    sendPropertyChanged("associated");
                }
            }
        }

        [Column(Name = "fk_eventID")]
        public int fk_eventID;

        private EntityRef<Event> _event;

        [Association(Name = "FK_Note_Event",
            Storage = "_event",         //Speicherort der Child-Instanzen.
            IsForeignKey = true,
            ThisKey = "fk_eventID",      //Name des Primärschlüssels.
            OtherKey = "eventID")] //Name des Fremdschlüssels.
        public Event obj_Event
        {
            get
            {
                return this._event.Entity;
            }
            set
            {
                sendPropertyChanging("event");
                this._event.Entity = value;
                sendPropertyChanged("event");
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
