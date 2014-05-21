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
using System.Collections;
using Microsoft.Phone.Shell;

namespace WritersToolbox.models
{
    /// <summary>
    /// Repräsentiert eine Notiz auf Datenbankebene.
    /// </summary>
    [Table(Name="Notes")]
    public class MemoryNote : INotifyPropertyChanging, INotifyPropertyChanged, ISearchable
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
            Storage = "stg_updatedDate")]
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
        [Column(CanBeNull = true,
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

        private String stg_tags;
        [Column(Storage = "stg_tags")]
        public String tags
        {
            get 
            {
                return stg_tags;
            }
            set
            {

                if (stg_tags != value)
                {
                    sendPropertyChanging("tags");
                    stg_tags = value;
                    sendPropertyChanged("tags");
            }
        }
        }

        [Column(Name = "fk_eventID", CanBeNull=true)]
        private int? fk_eventID; // ? = nullable type

        private EntityRef<Event> _event;

        [Association(Name = "FK_Note_Event",
            Storage = "_event",         //Speicherort der Child-Instanzen.
            IsForeignKey = true,
            ThisKey = "fk_eventID",      //Name des Primärschlüssels.
            OtherKey = "eventID" //Name des Fremdschlüssels.
            )] 
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


        [Column(Name = "fk_typeObjectID", CanBeNull = true)]
        private int? fk_typeObjectID; // ? = nullable type

        private EntityRef<TypeObject> _typeObject;

        [Association(Name = "FK_Note_TypeObject",
            Storage = "_typeObject",         //Speicherort der Child-Instanzen.
            IsForeignKey = true,
            ThisKey = "fk_typeObjectID",      //Name des Primärschlüssels.
            OtherKey = "typeObjectID" //Name des Fremdschlüssels.
            )]
        public TypeObject obj_TypeObject
        {
            get
            {
                return this._typeObject.Entity;
            }
            set
            {
                sendPropertyChanging("typeObject");
                this._typeObject.Entity = value;
                sendPropertyChanged("typeObject");
            }
        }

        private String stg_ContentImageString;
        [Column(CanBeNull = true,
            Storage = "stg_ContentImageString")]
        public String ContentImageString
        {
            get { return stg_ContentImageString; }
            set
            {
                if (stg_ContentImageString != value)
                {
                    sendPropertyChanging("contentImageString");
                    stg_ContentImageString = value;
                    sendPropertyChanged("contentImageString");
                }
            }
        }

        private String stg_ContentAudioString;
        [Column(CanBeNull = true,
            Storage = "stg_ContentAudioString")]
        public String contentAudioString
        {
            get { return stg_ContentAudioString; }
            set
            {
                if (stg_ContentAudioString != value)
                {
                    sendPropertyChanging("contentAudioString");
                    stg_ContentAudioString = value;
                    sendPropertyChanged("contentAudioString");
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

        public bool matchesQuery(string query)
        {
            return this.title.ToLower().Contains(query.ToLower()) || this.contentText.ToLower().Contains(query.ToLower());
        }

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Subtitle
        {
            get
            {
                if (this.contentText.Length > 50)
                {
                    return this.contentText.Substring(0, 50) + "...";
                }
                else {
                    return this.contentText;
                }
               
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Uri Link
        {
            get
            {
                PhoneApplicationService.Current.State["memoryNoteID"] = "" + this.memoryNoteID;
                PhoneApplicationService.Current.State["edit"] = "true";
                return new Uri("/views/AddNote.xaml", UriKind.Relative);
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
