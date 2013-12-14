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

namespace WritersToolbox.entity
{
    [Table(Name = "Booktypes")]
    class BookType : INotifyPropertyChanging, INotifyPropertyChanged
    {

        //um eine beschleunigte Ausführung der Datenänderung zu erreichen.
        [Column(IsVersion = true)]
        private Binary version;

        private int stg_bookTypeID;
        [Column(IsPrimaryKey = true,
            AutoSync = AutoSync.OnInsert,
            DbType = "INT IDENTITY",
            Storage = "stg_bookTypeID",
            IsDbGenerated = true)]
        public int bookTypeID
        {
            get { return stg_bookTypeID; }
            set
            {
                if (stg_bookTypeID != value)
                {
                    sendPropertyChanging("bookTypeID");
                    stg_bookTypeID = value;
                    sendPropertyChanged("bookTypeID");
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

        private int stg_numberOfChapter;
        [Column(CanBeNull = false,
            Storage = "stg_numberOfChapter")]
        public int numberOfChapter
        {
            get { return stg_numberOfChapter; }
            set
            {
                if (stg_numberOfChapter != value)
                {
                    sendPropertyChanging("numberOfChapter");
                    stg_numberOfChapter = value;
                    sendPropertyChanged("numberOfChapter");
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

        private EntitySet<Book> _books;

        [Association(Name="Booktypes_Books",
            Storage = "_books",          //Speicherort der Child-Instanzen.
            ThisKey = "bookTypeID",      //Name des Primärschlüssels.
            OtherKey = "fk_bookTypeID")] //Name des Fremdschlüssels.
        public EntitySet<Book> books
        {
            get
            {
                return this._books;
            }
            set
            {
                sendPropertyChanging("books");
                this._books.Assign(value);
                sendPropertyChanging("books");
            }
        }

        public BookType()
        {
            this._books = new EntitySet<Book>();    //EntitySet muss immer instanziert.
        }

        //Datenbank optimierung
        //Benachrichtigt Clients, dass sich ein Eigenschaftswert ändert.
        public event PropertyChangingEventHandler propertyChanging;
        protected void sendPropertyChanging(String propertyName)
        {
            PropertyChangingEventHandler handler = propertyChanging;
            if(handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        //Benachrichtigt Clients, dass ein Eigenschaftswert geändert wurde.
        public event PropertyChangedEventHandler propertyChanged;
        protected void sendPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = propertyChanged;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
