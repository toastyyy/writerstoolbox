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
    [Table(Name = "Tomes")]
    class Tome : INotifyPropertyChanging, INotifyPropertyChanged
    {

        //um eine beschleunigte Ausführung der Datenänderung zu erreichen.
        [Column(IsVersion = true)]
        private Binary version;

        private int stg_tomeID;
        [Column(IsPrimaryKey = true,
            AutoSync = AutoSync.OnInsert,
            DbType = "INT IDENTITY",
            Storage = "stg_tomeID",
            IsDbGenerated = true)]
        public int tomeID
        {
            get { return stg_tomeID; }
            set
            {
                if (stg_tomeID != value)
                {
                    sendPropertyChanging("tomeID");
                    stg_tomeID = value;
                    sendPropertyChanged("tomeID");
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

        private int stg_tomeNumber;
        [Column(CanBeNull = false,
            Storage = "stg_tomeNumber")]
        public int tomeNumber
        {
            get { return stg_tomeNumber; }
            set
            {
                if (stg_tomeNumber != value)
                {
                    sendPropertyChanging("tomeNumber");
                    stg_tomeNumber = value;
                    sendPropertyChanged("tomeNumber");
                }
            }
        }

        private DateTime stg_addedDate;
        [Column(CanBeNull=false,
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


        private EntitySet<Chapter> _chapters;

        [Association(Name = "Tomes_Chapters",
            Storage = "_chapters",      //Speicherort der Child-Instanzen.
            ThisKey = "tomeID",      //Name des Primärschlüssels.
            OtherKey = "fk_tomeID")] //Name des Fremdschlüssels.
        public EntitySet<Chapter> chapters
        {
            get
            {
                return this._chapters;
            }
            set
            {
                sendPropertyChanging("chapters");
                this._chapters.Assign(value);
                sendPropertyChanged("chapters");
            }
        }

        [Column(Name = "fk_bookID")]
        private int fk_bookID;

        private EntityRef<Book> _books;

        [Association(Name = "FK_Tome_Book",
            IsForeignKey = true,    //Fremdschlüssel.
            Storage = "_books",     //Speicherort der Relation.
            OtherKey = "bookID",    //Speicherort des Fremdschlüssels.
            ThisKey = "fk_bookID")] //Definition der Beziehung des Primärschlussels des Objekts.
        public Book obj_book
        {
            get { return _books.Entity; }
            set 
            {
                sendPropertyChanging("obj_book");
                _books.Entity = value;
                sendPropertyChanged("obj_book");
            }
        }

        public Tome()
        {
            this._chapters = new EntitySet<Chapter>();  //EntitySet muss immer instanziert.
        }

        //Datenbank optimierung
        //Benachrichtigt Clients, dass sich ein Eigenschaftswert ändert.
        public event PropertyChangingEventHandler propertyChanging;
        protected void sendPropertyChanging(String propertyName)
        {
            PropertyChangingEventHandler handler = propertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        //Benachrichtigt Clients, dass ein Eigenschaftswert geändert wurde.
        public event PropertyChangedEventHandler propertyChanged;
        protected void sendPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = propertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
