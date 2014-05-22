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
using System.Collections.ObjectModel;

namespace WritersToolbox.models
{
    /// <summary>
    /// Repräsentiert ein Kapitel auf Datenbankebene.
    /// </summary>
    [Table(Name = "Chapters")]
    public class Chapter : INotifyPropertyChanging, INotifyPropertyChanged, ISearchable
    {

        //um eine beschleunigte Ausführung der Datenänderung zu erreichen.
        [Column(IsVersion = true)]
        private Binary version;

        private int stg_chapterID;
        [Column(IsPrimaryKey = true,
            AutoSync = AutoSync.OnInsert,
            DbType = "INT IDENTITY",
            Storage = "stg_chapterID",
            IsDbGenerated = true)]
        public int chapterID
        {
            get { return stg_chapterID; }
            set
            {
                if (stg_chapterID != value)
                {
                    sendPropertyChanging("chapterID");
                    stg_chapterID = value;
                    sendPropertyChanged("chapterID");
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

        private int stg_chapterNumber;
        [Column(CanBeNull = false,
            Storage = "stg_chapterNumber")]
        public int chapterNumber
        {
            get { return stg_chapterNumber; }
            set
            {
                if (stg_chapterNumber != value)
                {
                    sendPropertyChanging("chapterNumber");
                    stg_chapterNumber = value;
                    sendPropertyChanged("chapterNumber");
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

        [Column(Name = "fk_tomeID")]
        private int fk_tomeID;

        private EntitySet<Event> _event;

        [Association(Name = "Chapters_Events",
            Storage = "_event",         //Speicherort der Child-Instanzen.
            ThisKey = "chapterID",      //Name des Primärschlüssels.
            OtherKey = "fk_chapterID")] //Name des Fremdschlüssels.
        public EntitySet<Event> events
        {
            get
            {
                return this._event;
            }
            set
            {
                sendPropertyChanging("events");
                this._event.Assign(value);
                sendPropertyChanged("events");
            }
        }

        private EntityRef<Tome> _tomes;

        [Association(Name = "FK_Chapter_Tome",
            IsForeignKey = true,    //Fremdschlüssel.
            Storage = "_tomes",     //Speicherort der Relation.
            OtherKey = "tomeID",    //Speicherort des Fremdschlüssels.
            ThisKey = "fk_tomeID")] //Definition der Beziehung des Primärschlussels des Objekts.
        public Tome obj_tome
        {
            get { return _tomes.Entity; }
            set 
            {
                sendPropertyChanging("obj_tome");
                _tomes.Entity = value;
                sendPropertyChanged("obj_tome");
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

        public Chapter()
        {
            this._event = new EntitySet<Event>();   //EntitySet muss immer instanziert.
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


        public static explicit operator datawrapper.Chapter(models.Chapter _chapter)
        {
            datawrapper.Chapter tempChapter = new datawrapper.Chapter()
            {
                chapterID = _chapter.chapterID,
                title = _chapter.title,
                addedDate = _chapter.addedDate,
                chapterNumber = _chapter.chapterNumber,
                deleted = _chapter.deleted,
                events = null,
                tome = null,
                updatedDate = _chapter.updatedDate
            };
            return tempChapter;
        }

        public bool matchesQuery(string query)
        {
            return this.title.ToLower().Contains(query.ToLower());
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
                return this.obj_tome.title;
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
                return new Uri("/views/TomeDetails.xaml?tomeID=" + this.obj_tome.tomeID, UriKind.RelativeOrAbsolute);
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
