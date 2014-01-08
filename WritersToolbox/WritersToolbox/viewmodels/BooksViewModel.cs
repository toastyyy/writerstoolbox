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
        private Boolean dataLoaded { get; set; }
        private WritersToolboxDatebase wtb;
        private Table<Book> tableBook;
        private Table<Tome> tableTome;
        private Table<Chapter> tableChapter;
        public BooksViewModel() {
            dataLoaded = false;
            wtb = WritersToolboxDatebase.getInstance();
            tableBook = wtb.GetTable<Book>();
            tableTome = wtb.GetTable<Tome>();
            tableChapter = wtb.GetTable<Chapter>();
        }

        public Boolean isDataLoaded() 
        {
            return dataLoaded;
        }

        public void loadData() 
        {
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

            this.Books = tmpBooks;
        }
    }
}
