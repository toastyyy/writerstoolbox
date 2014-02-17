using System;
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

namespace WritersToolbox.viewmodels
{
    
    public class BooksViewModel
    {
        public ObservableCollection<datawrapper.Book> Books { get; set; }
        public ObservableCollection<datawrapper.BookType> BookTypes { get; set; }
        private Boolean dataLoaded { get; set; }
        private WritersToolboxDatebase wtb;
        private Table<Book> tableBook;
        private Table<Tome> tableTome;
        private Table<Chapter> tableChapter;
        private Table<BookType> tableBookType;
        private Book obj_book;

        public BooksViewModel() {
            dataLoaded = false;
            wtb = WritersToolboxDatebase.getInstance();
            tableBook = wtb.GetTable<Book>();
            tableTome = wtb.GetTable<Tome>();
            tableChapter = wtb.GetTable<Chapter>();
            tableBookType = wtb.GetTable<BookType>();

            
        }

        public Boolean isDataLoaded() 
        {
            return dataLoaded;
        }

        public int getBookCount()
        {
            return this.Books.Count;
        }

        public void addBook(String name, int bookTypeID) {
            Book b = new Book();
            b.name = name;
            b.addedDate = DateTime.Now;
            b.updatedDate = DateTime.Now;
            b.obj_bookType = (from bt in tableBookType where bt.bookTypeID == bookTypeID select bt).Single();
            tableBook.InsertOnSubmit(b);
            this.wtb.SubmitChanges();
        }

        public void updateBook(int bookID, String name, int bookTypeID) {
            Book book = (from b in tableBook where b.bookID == bookID select b).Single();
            book.name = name;
            book.obj_bookType = (from bt in tableBookType where bt.bookTypeID == bookTypeID select bt).Single();
            book.updatedDate = DateTime.Now;
            this.wtb.SubmitChanges();
        }

        public void loadData() 
        {
            // buchtypen laden
            var sqlBookTypes = from bt in tableBookType
                               select bt;
            ObservableCollection<datawrapper.BookType> tmpBookTypes = new ObservableCollection<datawrapper.BookType>();
            foreach(var bt in sqlBookTypes) {
                tmpBookTypes.Add(
                    new datawrapper.BookType() { 
                        name = bt.name,
                        numberOfChapter = bt.numberOfChapter,
                        updatedDate = bt.updatedDate,
                        addedDate = bt.updatedDate,
                        bookTypeID = bt.bookTypeID
                    }
                );
            }

            this.BookTypes = tmpBookTypes;
            // buecher laden
            var sqlBooks = from b in tableBook
                                   select b;

            ObservableCollection<datawrapper.Book> tmpBooks = new ObservableCollection<datawrapper.Book>();

            foreach(var b in sqlBooks) 
            {
                List<datawrapper.Tome> tmpTomes = new List<datawrapper.Tome>();
                var sqlTomes = from t in tableTome
                               where t.obj_book.bookID == b.bookID
                               select t;

                foreach(var t in sqlTomes) 
                {
                    var sqlChapters = from c in tableChapter
                                      where c.obj_tome.tomeID == t.tomeID
                                      select c;

                    List<datawrapper.Chapter> listChapter = new List<datawrapper.Chapter>();
                    foreach (var c in sqlChapters) 
                    {
                        datawrapper.Chapter chapter = new datawrapper.Chapter() { 
                            
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

                datawrapper.Tome newTome = new datawrapper.Tome()
                {
                    tomeID = -1
                };
                tmpTomes.Add(newTome);

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

            tmpBooks.Add(new datawrapper.Book()
            {
                name="Neues Werk",
                bookID=-1,
                addedDate = new DateTime(2012,6,3,22,10,22)
            });
            this.Books = tmpBooks;
        }

        public void deleteBook( datawrapper.Book item)
        {
            
                obj_book = wtb.GetTable<Book>().Single(book => book.bookID == item.bookID);
                obj_book.deleted = true;
            
            wtb.SubmitChanges();
        }
    }
}
