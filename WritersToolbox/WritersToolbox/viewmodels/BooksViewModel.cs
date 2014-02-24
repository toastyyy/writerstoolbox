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

namespace WritersToolbox.viewmodels
{

    public class BooksViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<datawrapper.Book> books;

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

        public void addBook(String name, datawrapper.BookType bookType) {
            Book b = new Book();
            b.name = name;
            b.addedDate = DateTime.Now;
            b.updatedDate = DateTime.Now;
            b.obj_bookType = (from bt in tableBookType where bt.bookTypeID == bookType.bookTypeID select bt).Single();

            Tome t = new Tome()
                {
                    title = "Band 1",
                    addedDate = DateTime.Now,
                    updatedDate = DateTime.Now,
                    obj_book = b,
                    information = 1
                };
            for(int i = 0; i < bookType.numberOfChapter; i++) 
            {
                Chapter c = new Chapter()
                    {
                        chapterNumber = i + 1,
                        title = "Kapitel " + i.ToString(),
                        addedDate = DateTime.Now,
                        updatedDate = DateTime.Now,
                        obj_tome = t
                    };
            }
            tableBook.InsertOnSubmit(b);
            this.wtb.SubmitChanges();
            this.NotifyPropertyChanged("Books");
        }

        public void updateBook(int bookID, String name, int bookTypeID) {
            Book book = (from b in tableBook where b.bookID == bookID select b).Single();
            book.name = name;
            book.obj_bookType = (from bt in tableBookType where bt.bookTypeID == bookTypeID select bt).Single();
            book.updatedDate = DateTime.Now;
            this.wtb.SubmitChanges();
            this.NotifyPropertyChanged("Books");
        }

        public Book getBookByID(int bookID)
        {
            Book book = (from b in tableBook where b.bookID == bookID select b).Single();
            return book;
        }

        public void loadData() 
        {
            // buchtypen laden
            var sqlBookTypes = from bt in tableBookType
                               where bt.deleted == false
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

            this.booktypes = tmpBookTypes;
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
            this.books = tmpBooks;
        }

        public void deleteBook( datawrapper.Book item, bool keepTomes)
        {
            if (keepTomes)
            {
                obj_book = wtb.GetTable<Book>().Single(book => book.bookID == item.bookID);
                obj_book.deleted = true;
                this.wtb.SubmitChanges();
                this.loadData();
                this.NotifyPropertyChanged("Books");
            }
            else
            {

            }
                
            wtb.SubmitChanges();
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

        
    }
}
