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

            Tome t1 = new Tome() 
            {
                title = "Stein der Weisen",
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                obj_book = b
            };

            Tome t2 = new Tome()
            {
                title = "Kammer des Schreckens",
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                obj_book = b
            };

            Chapter c1 = new Chapter() 
            { 
                chapterNumber = 1,
                title = "Ein Junge überlebt",
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                obj_tome = t1
            };

            Chapter c2 = new Chapter()
            {
                chapterNumber = 2,
                title = "Ein Fenster verschwindet",
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                obj_tome = t1
            };

            Chapter c3 = new Chapter()
            {
                chapterNumber = 3,
                title = "Briefe von niemandem",
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                obj_tome = t1
            };

            Event e1 = new Event() 
            { 
                title = "Haus der Dursleys",
                obj_Chapter = c1
            };

            Event e2 = new Event()
            {
                title = "Bahnhof",
                obj_Chapter = c1
            };

            MemoryNote mn1 = new MemoryNote() 
            { 
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                contentText = "lorem ipsum dolor sit amet",
                title = "testnotiz",
                associated = true,
                tags = "test1|test2",
                obj_Event = e1
            };

            WritersToolbox.models.Type type1 = new WritersToolbox.models.Type() {
                title = "Charakter",
                color = "#ff0000"
            };

            WritersToolbox.models.Type type2 = new WritersToolbox.models.Type()
            {
                title = "Handlungsort",
                color = "#00ff00"
            };

            WritersToolbox.models.Type type3 = new WritersToolbox.models.Type()
            {
                title = "Formeln",
                color = "#ff0000"
            };

            WritersToolbox.models.Type type4 = new WritersToolbox.models.Type()
            {
                title = "Gruppierungen",
                color = "#ffffff"
            };

            TypeObject typeObject1 = new TypeObject() 
            { 
                name = "Harry Potter",
                obj_Type = type1
            };

            MemoryNote mn2 = new MemoryNote()
            {
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                contentText = "lorem ipsum dolor sit amet",
                title = "aussehen",
                associated = true,
                tags = "harry|potter|aussehen|narbe|haare|klein",
                obj_TypeObject = typeObject1
            };

            db.GetTable<BookType>().InsertOnSubmit(bt);
            db.GetTable<Book>().InsertOnSubmit(b);
            db.GetTable<Tome>().InsertOnSubmit(t1);
            db.GetTable<Tome>().InsertOnSubmit(t2);
            db.GetTable<Chapter>().InsertOnSubmit(c1);
            db.GetTable<Chapter>().InsertOnSubmit(c2);
            db.GetTable<Event>().InsertOnSubmit(e1);
            db.GetTable<Event>().InsertOnSubmit(e2);
            db.GetTable<MemoryNote>().InsertOnSubmit(mn1);

            db.GetTable<WritersToolbox.models.Type>().InsertOnSubmit(type1);
            db.GetTable<WritersToolbox.models.Type>().InsertOnSubmit(type2);
            db.GetTable<WritersToolbox.models.Type>().InsertOnSubmit(type3);
            db.GetTable<WritersToolbox.models.Type>().InsertOnSubmit(type4);

            db.GetTable<TypeObject>().InsertOnSubmit(typeObject1);
            db.GetTable<MemoryNote>().InsertOnSubmit(mn2);

            db.SubmitChanges();
        }
    }
}
