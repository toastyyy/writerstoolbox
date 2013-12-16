using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using WritersToolbox.models;

namespace WritersToolbox.demo
{
    class Data
    {
        public static void AddToDB(DataContext db)
        {
            BookType bt = new BookType();
            bt.name = "Roman";
            bt.updatedDate = DateTime.Now;
            bt.addedDate = DateTime.Now;
            bt.numberOfChapter = 3;

            Book b = new Book();
            b.name = "Harry Potter";
            b.addedDate = DateTime.Now;
            b.updatedDate = DateTime.Now;
            b.obj_bookType = bt;

            Tome t1 = new Tome();
            t1.title = "Stein der Weisen";
            t1.addedDate = DateTime.Now;
            t1.updatedDate = DateTime.Now;
            t1.obj_book = b;

            Tome t2 = new Tome();
            t2.title = "Kammer des Schreckens";
            t2.addedDate = DateTime.Now;
            t2.updatedDate = DateTime.Now;
            t2.obj_book = b;

            MemoryNote mn = new MemoryNote();
            mn.addedDate = DateTime.Now;
            mn.updatedDate = DateTime.Now;
            mn.contentText = "lorem ipsum dolor sit amet";
            mn.title = "testnotiz";
            mn.associated = false;
            mn.tags = "test1|test2";



            db.GetTable<BookType>().InsertOnSubmit(bt);
            db.GetTable<MemoryNote>().InsertOnSubmit(mn);
            db.GetTable<Book>().InsertOnSubmit(b);
            db.GetTable<Tome>().InsertOnSubmit(t1);
            db.GetTable<Tome>().InsertOnSubmit(t2);

            db.SubmitChanges();
        }
    }
}
