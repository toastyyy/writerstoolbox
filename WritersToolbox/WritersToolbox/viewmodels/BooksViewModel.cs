﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Phone.Controls;
using WritersToolbox.Resources;
using Microsoft.Phone.Shell;
using System.Windows;

namespace WritersToolbox.viewmodels
{
    /// <summary>
    /// Die BooksViewModel Klasse bzw. Präsentations-Logik ist eine aggregierte Datenquelle,
    /// die verschiedene Daten von Book und ihre entsprechende Eigenschaften bereitstellt.
    /// </summary>
    public class BooksViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<datawrapper.Book> books;

        /// <summary>
        /// Enthält alle Werke, nachdem die Methode LoadData ausgeführt wurde.
        /// </summary>
        public ObservableCollection<datawrapper.Book> Books
        {
            get { return books; }
            set
            {
                books = value;
                NotifyPropertyChanged("Books");
            }
        }
        private ObservableCollection<datawrapper.BookType> booktypes;

        /// <summary>
        /// Enthält alle Buchtypen, nachdem die Methode LoadBookTypes ausgeführt wurde.
        /// </summary>
        public ObservableCollection<datawrapper.BookType> BookTypes
        {
            get { return booktypes; }
            set
            {
                booktypes = value;
                NotifyPropertyChanged("BookTypes");
            }
        }
        private Boolean dataLoaded { get; set; }
        private WritersToolboxDatebase wtb;
        private Table<Book> tableBook;
        private Table<Tome> tableTome;
        private Table<Chapter> tableChapter;
        private Table<BookType> tableBookType;
        private Table<Event> tableEvents = null;
        private Book obj_book;

        /// <summary>
        /// Erzeugt eine neue Instanz des Viewmodels und läd die Datenbanktabellen.
        /// </summary>
        public BooksViewModel() {
            dataLoaded = false;
            wtb = WritersToolboxDatebase.getInstance();
            tableBook = wtb.GetTable<Book>();
            tableTome = wtb.GetTable<Tome>();
            tableChapter = wtb.GetTable<Chapter>();
            tableBookType = wtb.GetTable<BookType>();
            this.tableEvents = this.wtb.GetTable<Event>();
            
        }

        /// <summary>
        /// Prüft, ob die Methode LoadData bereits ausgeführt wurde.
        /// </summary>
        /// <returns>True, wenn LoadData ausgeführt wurde</returns>
        public Boolean isDataLoaded() 
        {
            return dataLoaded;
        }

        /// <summary>
        /// Gibt die Anzahl der Werke zurück.
        /// </summary>
        /// <returns>Anzahl der Werke</returns>
        public int getBookCount()
        {
            return this.Books.Count;
        }

        /// <summary>
        /// Fügt ein neues Werk mit den angegebenen Daten in die Datenbank ein.
        /// </summary>
        /// <param name="name">Name des neuen Werkes</param>
        /// <param name="bookType">Buchtyp des Werkes</param>
        public void addBook(String name, datawrapper.BookType bookType) {
            if (name.Trim().Equals("")) {
                throw new ArgumentException("Das Werk muss einen Titel haben.");
            }
            Boolean existsBookWithName = (from bo in tableBook
                                          where bo.name.Equals(name)
                                          select bo).Count() > 0;

            if (existsBookWithName) {
                throw new ArgumentException("Ein Werk mit dem angegebenen Titel existiert bereits.");
            } 

            Book b = new Book();
            b.name = name;
            b.addedDate = DateTime.Now;
            b.updatedDate = DateTime.Now;
            b.obj_bookType = (from bt in tableBookType where bt.bookTypeID == bookType.bookTypeID select bt).Single();

            Tome t = new Tome()
                {
                    title = AppResources.BooksPrestructuredTomeVolume1,
                    addedDate = DateTime.Now,
                    updatedDate = DateTime.Now,
                    obj_book = b,
                    information = 1
                };
            switch (bookType.bookTypeID)
            {
                case 1: Chapter c1 = new Chapter()
                        {
                            chapterNumber = 1,
                            title = AppResources.BooksPrestructuredTomeIntroduction,
                            addedDate = DateTime.Now,
                            updatedDate = DateTime.Now,
                            obj_tome = t
                        };
                        tableChapter.InsertOnSubmit(c1);
                        Chapter c2 = new Chapter()
                        {
                            chapterNumber = 2,
                            title = AppResources.BooksPrestructuredTomeChapter1,
                            addedDate = DateTime.Now,
                            updatedDate = DateTime.Now,
                            obj_tome = t
                        };
                        tableChapter.InsertOnSubmit(c2);
                        Chapter c3 = new Chapter()
                        {
                            chapterNumber = 3,
                            title = AppResources.BooksPrestructuredTomeEpilogue,
                            addedDate = DateTime.Now,
                            updatedDate = DateTime.Now,
                            obj_tome = t
                        };
                        tableChapter.InsertOnSubmit(c3);
                        break;
                case 2: Chapter c4 = new Chapter()
                        {
                            chapterNumber = 1,
                            title = AppResources.BooksPrestructuredTomePoem1,
                            addedDate = DateTime.Now,
                            updatedDate = DateTime.Now,
                            obj_tome = t
                        };
                        tableChapter.InsertOnSubmit(c4);
                        Chapter c5 = new Chapter()
                        {
                            chapterNumber = 2,
                            title = AppResources.BooksPrestructuredTomePoem2,
                            addedDate = DateTime.Now,
                            updatedDate = DateTime.Now,
                            obj_tome = t
                        };
                        tableChapter.InsertOnSubmit(c5);
                        break;
                case 3: Chapter c6 = new Chapter()
                        {
                            chapterNumber = 1,
                            title = AppResources.BooksPrestructuredTomeIntroduction,
                            addedDate = DateTime.Now,
                            updatedDate = DateTime.Now,
                            obj_tome = t
                        };
                        tableChapter.InsertOnSubmit(c6);
                        Chapter c7 = new Chapter()
                        {
                            chapterNumber = 2,
                            title = AppResources.BooksPrestructuredTomeShortstory1,
                            addedDate = DateTime.Now,
                            updatedDate = DateTime.Now,
                            obj_tome = t
                        };
                        tableChapter.InsertOnSubmit(c7);
                        break;
            }
            
            
            tableTome.InsertOnSubmit(t);
            tableBook.InsertOnSubmit(b);
            this.wtb.SubmitChanges();
            this.NotifyPropertyChanged("Books");
            this.loadData();
        }

        /// <summary>
        /// Aktualisiert das Werk mit der übergebenen ID mit den angegebenen Werten.
        /// </summary>
        /// <param name="bookID">ID des Werkes</param>
        /// <param name="name">Neuer Name des Werkes</param>
        /// <param name="bookTypeID">Neuer Buchtyp des Werkes</param>
        public void updateBook(int bookID, String name, int bookTypeID) {
            Book book = (from b in tableBook where b.bookID == bookID select b).Single();
            book.name = name;
            book.obj_bookType = (from bt in tableBookType where bt.bookTypeID == bookTypeID select bt).Single();
            book.updatedDate = DateTime.Now;
            this.wtb.SubmitChanges();
            this.NotifyPropertyChanged("Books");
            this.loadData();
        }

        /// <summary>
        /// Gibt das Werk mit der angegebenen ID zurück.
        /// </summary>
        /// <param name="bookID">ID des Werkes</param>
        /// <returns>Werk Entity</returns>
        public Book getBookByID(int bookID)
        {
            Book book = (from b in tableBook where b.bookID == bookID select b).Single();
            return book;
        }

        /// <summary>
        /// Läd die Buchtypen aus der Datenbank.
        /// </summary>
        public void loadBookTypes()
        {
            // buchtypen laden
            var sqlBookTypes = from bt in tableBookType
                               where bt.deleted == false
                               select bt;
            ObservableCollection<datawrapper.BookType> tmpBookTypes = new ObservableCollection<datawrapper.BookType>();
            foreach (var bt in sqlBookTypes)
            {
                tmpBookTypes.Add(
                    new datawrapper.BookType()
                    {
                        name = bt.name,
                        numberOfChapter = bt.numberOfChapter,
                        updatedDate = bt.updatedDate,
                        addedDate = bt.updatedDate,
                        bookTypeID = bt.bookTypeID
                    }
                );
            }

            this.booktypes = tmpBookTypes;
        }
        /// <summary>
        /// Erstellt ein neues Tome aus den angegebenen Werten.
        /// Wenn ein Wert ungültig ist, wird eine ArgumentException geworfen.
        /// </summary>
        /// <param name="name">Name des Tomes</param>
        /// <param name="typeID">ID des zugehörigen Werkes</param>
        public void addTome(String title, int bookID, int bookTypeID)
        {

            models.Book book = (from t in tableBook
                                where t.bookID == bookID
                                select t).FirstOrDefault();

            if (title.Equals(""))
            {
                throw new ArgumentException("Ein Titel muss angegeben werden", "title");
            }
            if (book == null)
            {
                throw new ArgumentException("Ein Band muss einem Werk angehören", "book");
            }

            Boolean tomeTitleExists = (from t in tableTome
                                       where t.obj_book.bookID == bookID && t.title.Equals(title)
                                       select t).Count() > 0;

            if (tomeTitleExists) {
                throw new ArgumentException("Ein Band mit dem angegebenen Namen existiert in diesem Werk bereits.");
            }
            
            Tome to = new Tome();
            to.title = title;
            to.obj_book = book;
            to.addedDate = DateTime.Now;
            to.updatedDate = DateTime.Now;


            switch (bookTypeID)
            {
                case 1: Chapter c1 = new Chapter()
                        {
                            chapterNumber = 1,
                            title = AppResources.BooksPrestructuredTomeIntroduction,
                            addedDate = DateTime.Now,
                            updatedDate = DateTime.Now,
                            obj_tome = to
                        };
                    tableChapter.InsertOnSubmit(c1);
                    Chapter c2 = new Chapter()
                    {
                        chapterNumber = 2,
                        title = AppResources.BooksPrestructuredTomeChapter1,
                        addedDate = DateTime.Now,
                        updatedDate = DateTime.Now,
                        obj_tome = to
                    };
                    tableChapter.InsertOnSubmit(c2);
                    Chapter c3 = new Chapter()
                    {
                        chapterNumber = 3,
                        title = AppResources.BooksPrestructuredTomeEpilogue,
                        addedDate = DateTime.Now,
                        updatedDate = DateTime.Now,
                        obj_tome = to
                    };
                    tableChapter.InsertOnSubmit(c3);
                    break;
                case 2: Chapter c4 = new Chapter()
                        {
                            chapterNumber = 1,
                            title = AppResources.BooksPrestructuredTomePoem1,
                            addedDate = DateTime.Now,
                            updatedDate = DateTime.Now,
                            obj_tome = to
                        };
                    tableChapter.InsertOnSubmit(c4);
                    Chapter c5 = new Chapter()
                    {
                        chapterNumber = 2,
                        title = AppResources.BooksPrestructuredTomePoem2,
                        addedDate = DateTime.Now,
                        updatedDate = DateTime.Now,
                        obj_tome = to
                    };
                    tableChapter.InsertOnSubmit(c5);
                    break;
                case 3: Chapter c6 = new Chapter()
                        {
                            chapterNumber = 1,
                            title = AppResources.BooksPrestructuredTomeIntroduction,
                            addedDate = DateTime.Now,
                            updatedDate = DateTime.Now,
                            obj_tome = to
                        };
                    tableChapter.InsertOnSubmit(c6);
                    Chapter c7 = new Chapter()
                    {
                        chapterNumber = 2,
                        title = AppResources.BooksPrestructuredTomeShortstory1,
                        addedDate = DateTime.Now,
                        updatedDate = DateTime.Now,
                        obj_tome = to
                    };
                    tableChapter.InsertOnSubmit(c7);
                    break;
            }
                    this.tableTome.InsertOnSubmit(to);
                    this.wtb.SubmitChanges();
                    this.loadData();
            
        }

        /// <summary>
        /// Läd alle Werke mit den zugehörigen Bänden aus der Datenbank und stellt diese
        /// über Properties bereit.
        /// </summary>
        public void loadData() 
        {
            
            // buecher laden
            var sqlBooks = from b in tableBook where b.deleted == false
                                   select b;

            ObservableCollection<datawrapper.Book> tmpBooks = new ObservableCollection<datawrapper.Book>();

            foreach(var b in sqlBooks) 
            {
                List<datawrapper.Tome> tmpTomes = new List<datawrapper.Tome>();
                var sqlTomes = from t in tableTome
                               where t.obj_book.bookID == b.bookID && t.deleted == false
                               select t;

                foreach(var t in sqlTomes) 
                {
                    var sqlChapters = from c in tableChapter
                                      where c.obj_tome.tomeID == t.tomeID && c.deleted == false
                                      select c;

                    List<datawrapper.Chapter> listChapter = new List<datawrapper.Chapter>();
                    foreach (var c in sqlChapters)
                    {
                        datawrapper.Chapter chapter = new datawrapper.Chapter()
                        {

                        };
                        listChapter.Add(chapter);
                    }
                    datawrapper.Tome tome = new datawrapper.Tome() { 
                        addedDate = t.addedDate,
                        deleted = t.deleted,
                        title = t.title,
                        tomeID = t.tomeID,
                        tomeNumber = t.tomeNumber,
                        updatedDate = t.updatedDate,
                        chapters = listChapter
                    };

                    tmpTomes.Add(tome);

                }

                if (!PhoneApplicationService.Current.State.ContainsKey("assignNote")) { 
                    datawrapper.Tome newTome = new datawrapper.Tome()
                    {
                        tomeID = -1
                    };
                    tmpTomes.Add(newTome);
                }
                

                datawrapper.Book book = new datawrapper.Book() {
                    addedDate = b.addedDate,
                    bookID = b.bookID,
                    bookType = null,
                    name = b.name,
                    tomes = tmpTomes,
                    updatedDate = b.updatedDate
                };
                tmpBooks.Add(book);
            }

            if (!PhoneApplicationService.Current.State.ContainsKey("assignNote"))
            {
                tmpBooks.Add(new datawrapper.Book()
                {
                    name= AppResources.NewBook,
                    bookID=-1,
                    addedDate = new DateTime(2012,6,3,22,10,22)
                });
                
            }
            this.books = tmpBooks;
            this.NotifyPropertyChanged("Books");
        }

        /// <summary>
        /// Löscht das angegebene Werk.
        /// Löschen ist noch nicht entgültig.
        /// </summary>
        /// <param name="item">Werk, das gelöscht werden soll</param>
        /// <param name="keepTomes">Gibt an, ob die Bände innerhalb des Werkes mitgelöscht werden sollen oder nicht [derzeit nicht genutzt]</param>
        public void deleteBook( datawrapper.Book item, bool keepTomes)
        {
            if (keepTomes)
            {
                obj_book = wtb.GetTable<Book>().Single(book => book.bookID == item.bookID);
                obj_book.deleted = true;
                this.wtb.SubmitChanges();
                this.NotifyPropertyChanged("Books");
                this.loadData();
            }
            else
            {

            }
                
        }

        /// <summary>
        /// Entfernt den Eintrag zum hinzufügen eines neuen Bandes im angegebenen Werk.
        /// </summary>
        /// <param name="b">Werk</param>
        public void removeAddTome(datawrapper.Book b)
        {
            int i = Books.IndexOf(b);
            for (int j = 0; j < Books.ElementAt(i).tomes.Count; j++)
            {
                if (Books.ElementAt(i).tomes.ElementAt(j).tomeID == -1)
                {
                    Books.ElementAt(i).tomes.RemoveAt(j);
                    
                }
            }
            this.NotifyPropertyChanged("Tomes");
        }

        /// <summary>
        /// Fügt den Eintrag zum Hinzufügen eines neuen Bandes im angegebenen Werk hinzu.
        /// </summary>
        /// <param name="b">Werk</param>
        public void addAddTome(datawrapper.Book b)
        {
            int i = Books.IndexOf(b);
            Boolean hasAdd = false;
            for (int j = 0; j < Books.ElementAt(i).tomes.Count && !hasAdd; j++)
            {
                hasAdd = Books.ElementAt(i).tomes.ElementAt(j).tomeID == -1;
            }
            if (!hasAdd)
            {
                Books.ElementAt(i).tomes.Add(
    new datawrapper.Tome()
    {
        title = AppResources.NewTomeTemplate,
        tomeID = -1
        
    }
    );
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        /// <summary>
        /// Löscht den Band mit der angegebenen ID.
        /// Löschen ist noch nicht entgültig.
        /// </summary>
        /// <param name="p">ID des Bandes</param>
        internal void deleteTome(int p)
        {
            var sqlTome = (from t in tableTome
                           where t.tomeID == p
                           select t).Single();
            sqlTome.deleted = true;

            if (sqlTome.chapters.Count != 0)
            {
                var sqlChapters = from c in tableChapter
                                  where c.obj_tome.tomeID == sqlTome.tomeID
                                  select c;
                foreach (var c in sqlChapters)
                {
                    c.deleted = true;

                    if (c.events.Count != 0)
                    {
                        Event sqlEvent = (from e in this.tableEvents
                                          where e.fk_chapterID == c.chapterID
                                          select e).Single();

                        sqlEvent.deleted = true;
                    }
                }
            }
            this.wtb.SubmitChanges();
            this.loadData();
        }
    }
}
