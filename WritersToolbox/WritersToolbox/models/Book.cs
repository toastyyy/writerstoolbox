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
    [Table(Name = "Books")]
    public class Book : INotifyPropertyChanging, INotifyPropertyChanged, ISearchable
    {
        //um eine beschleunigte Ausführung der Datenänderung zu erreichen.
        [Column(IsVersion = true)]
        private Binary version;

        private int stg_bookID;
        [Column(IsPrimaryKey = true,
            AutoSync = AutoSync.OnInsert,
            DbType = "INT IDENTITY",
            Storage = "stg_bookID",
            IsDbGenerated = true)]
        public int bookID
        {
            get { return stg_bookID; }
            set
            {
                if (stg_bookID != value)
                {
                    sendPropertyChanging("bookID");
                    stg_bookID = value;
                    sendPropertyChanged("bookID");
                }
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

        private EntitySet<Tome> _tomes;

        [Association(Name = "Books_Tomes",
            Storage = "_tomes",      //Speicherort der Child-Instanzen.
            ThisKey = "bookID",      //Name des Primärschlüssels.
            OtherKey = "fk_bookID")] //Name des Fremdschlüssels.
        public EntitySet<Tome> tomes
        {
            get
            {
                return this._tomes;
            }
            set
            {
                sendPropertyChanging("tomes");
                this._tomes.Assign(value);
                sendPropertyChanged("tomes");
            }
        }

        [Column(Name = "fk_bookTypeID")]
        private int fk_bookTypeID;

        private EntityRef<BookType> _booktypes;
        [Association(Name = "FK_Book_BookType",
            IsForeignKey = true,        //Fremdschlüssel.
            Storage = "_booktypes",     //Speicherort der Relation.
            OtherKey = "bookTypeID",    //Speicherort des Fremdschlüssels.
            ThisKey = "fk_bookTypeID")] //Definition der Beziehung des Primärschlussels des Objekts.
        public BookType obj_bookType
        {
            get { return _booktypes.Entity; }
            set 
            {
                sendPropertyChanging("_booktypes");
                _booktypes.Entity = value;
                sendPropertyChanged("_booktypes");
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

        public Book()
        {
            this._tomes = new EntitySet<Tome>();    //EntitySet muss immer instanziert.
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
            return this.name.ToLower().Contains(query.ToLower());
        }

        public string Title
        {
            get
            {
                return this.name;
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
                return this.obj_bookType.name;
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
                return new Uri("/views/Books.xaml?item=" + this.bookID, UriKind.RelativeOrAbsolute);
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
