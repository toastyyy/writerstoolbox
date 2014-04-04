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

    [Table(Name = "Events")]
    public class Event : INotifyPropertyChanging, INotifyPropertyChanged, ISearchable
    {

        public Event() 
        {
            _notes = new EntitySet<MemoryNote>();
            _event_typeObjects = new EntitySet<EventTypeObjects>();
        }

        //um eine beschleunigte Ausführung der Datenänderung zu erreichen.
        [Column(IsVersion = true)]
        private Binary version;

        private int stg_eventID;
        [Column(IsPrimaryKey = true,
            AutoSync = AutoSync.OnInsert,
            DbType = "INT IDENTITY",
            Storage = "stg_eventID",
            IsDbGenerated = true)]
        public int eventID
        {
            get { return stg_eventID; }
            set
            {
                if (stg_eventID != value)
                {
                    sendPropertyChanging("eventID");
                    stg_eventID = value;
                    sendPropertyChanged("eventID");
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

        private String stg_finaltext;
        [Column(CanBeNull = false,
            Storage = "stg_finaltext",
            DbType = "ntext", 
            UpdateCheck = UpdateCheck.Never
            )]
        public String finaltext
        {
            get { return stg_finaltext; }
            set
            {
                if (stg_finaltext != value)
                {
                    sendPropertyChanging("finaltext");
                    stg_finaltext = value;
                    sendPropertyChanged("finaltext");
                }
            }
        }
        private int stg_orderInChapter;
        [Column(CanBeNull = false,
            Storage = "stg_orderInChapter")]
        public int orderInChapter
        {
            get { return stg_orderInChapter; }
            set
            {
                if (stg_orderInChapter != value)
                {
                    sendPropertyChanging("orderInChapter");
                    stg_orderInChapter = value;
                    sendPropertyChanged("orderInChapter");
                }
            }
        }

        
        [Column(Name = "fk_chapterID")]
        public int? fk_chapterID;

        private EntityRef<Chapter> _chapter;

        [Association(Name = "FK_Event_Chapter",
            Storage = "_chapter",         //Speicherort der Child-Instanzen.
            IsForeignKey=true,
            ThisKey = "fk_chapterID",      //Name des Primärschlüssels.
            OtherKey = "chapterID")] //Name des Fremdschlüssels.
        public Chapter obj_Chapter
        {
            get
            {
                return this._chapter.Entity;
            }
            set
            {
                sendPropertyChanging("chapter");
                this._chapter.Entity = value;
                sendPropertyChanged("chapter");
            }
        }

        private EntitySet<MemoryNote> _notes;

        [Association(Name = "Events_Notes",
            Storage = "_notes",         //Speicherort der Child-Instanzen.
            ThisKey = "eventID",      //Name des Primärschlüssels.
            OtherKey = "fk_eventID")] //Name des Fremdschlüssels.
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

        /// <summary>
        /// Explicit Konvertierung von Models.Event to Datawrapper.Event
        /// </summary>
        /// <param name="_event">Models.Event</param>
        /// <returns>Datawrapper.Event</returns>
        public static explicit operator datawrapper.Event(models.Event _event)
        {

            datawrapper.Event tempEvent = new datawrapper.Event()
                {
                    chapter = new datawrapper.Chapter() { chapterID = (int)_event.fk_chapterID},
                    deleted = _event.deleted,
                    eventID = _event.eventID,
                    notes = null,
                    orderInChapter = _event.orderInChapter,
                    title = _event.title,
                    typeObjects = null
                };

            return tempEvent;
        }

        private EntitySet<EventTypeObjects> _event_typeObjects;

        [Association(Name = "Event_TypeObjects",
            Storage = "_event_typeObjects",         //Speicherort der Child-Instanzen.
            ThisKey = "eventID",      //Name des Primärschlüssels.
            OtherKey = "fk_eventID")] //Name des Fremdschlüssels.
        public EntitySet<EventTypeObjects> typeObjects
        {
            get
            {
                return this._event_typeObjects;
            }
            set
            {
                sendPropertyChanging("typeObjects");
                this._event_typeObjects.Assign(value);
                sendPropertyChanged("typeObjects");
            }
        }

        public bool matchesQuery(string query)
        {
            return this.title.Contains(query) || this.finaltext.Contains(query);
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
                return "Kapitel " + this.obj_Chapter.title;
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
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
