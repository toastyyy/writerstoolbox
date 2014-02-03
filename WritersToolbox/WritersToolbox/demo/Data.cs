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

            Book b2 = new Book();
            b2.name = "Der Herr der Ringe";
            b2.addedDate = DateTime.Now;
            b2.updatedDate = DateTime.Now;
            b2.obj_bookType = bt;

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

            Tome t3 = new Tome()
            {
                title = "Die Gefährten",
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                obj_book = b2
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
                ContentImageString = "sample_photo_01.jpg|sample_photo_03.jpg|sample_photo_05.jpg",
                obj_Event = e1
            };

            WritersToolbox.models.Type type1 = new WritersToolbox.models.Type() {
                title = "Charakter",
                color = "#2d66c3"
            };

            WritersToolbox.models.Type type2 = new WritersToolbox.models.Type()
            {
                title = "Handlungsort",
                color = "#d1441d"
            };

            WritersToolbox.models.Type type3 = new WritersToolbox.models.Type()
            {
                title = "Formeln",
                color = "#3ca032"
            };

            WritersToolbox.models.Type type4 = new WritersToolbox.models.Type()
            {
                title = "Gruppierungen",
                color = "#ffffff"
            };

            TypeObject typeObject1 = new TypeObject() 
            { 
                name = "Harry Potter",
                obj_Type = type1,
                used = true,
                color = "#2d66c3"
            };

            TypeObject typeObject2= new TypeObject()
            {
                name = "Ron Weasley",
                obj_Type = type1,
                used = true
            };

            TypeObject typeObject3 = new TypeObject()
            {
                name = "Hogwarts",
                obj_Type = type2
            };

            TypeObject typeObject4 = new TypeObject()
            {
                name = "Zauberwald",
                obj_Type = type2
            };

            TypeObject typeObject5 = new TypeObject()
            {
                name = "Frodo Beutlin",
                obj_Type = type1
            };

            TypeObject typeObject6 = new TypeObject()
            {
                name = "Bilbo Beutlin",
                obj_Type = type1
            };

            TypeObject typeObject7 = new TypeObject()
            {
                name = "Gandalf",
                obj_Type = type1
            };
            TypeObject typeObject8 = new TypeObject()
            {
                name = "Gollum",
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

            MemoryNote mn3 = new MemoryNote()
            {
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                contentText = "blablub trololololol",
                title = "testnotiz an harry",
                associated = true,
                tags = "harry|potter|test",
                obj_TypeObject = typeObject1
            };

            db.GetTable<BookType>().InsertOnSubmit(bt);
            db.GetTable<Book>().InsertOnSubmit(b);
            db.GetTable<Book>().InsertOnSubmit(b2);
            db.GetTable<Tome>().InsertOnSubmit(t1);
            db.GetTable<Tome>().InsertOnSubmit(t2);
            db.GetTable<Tome>().InsertOnSubmit(t3);
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
            db.GetTable<TypeObject>().InsertOnSubmit(typeObject2);
            db.GetTable<TypeObject>().InsertOnSubmit(typeObject3);
            db.GetTable<TypeObject>().InsertOnSubmit(typeObject4);
            db.GetTable<TypeObject>().InsertOnSubmit(typeObject5);
            db.GetTable<TypeObject>().InsertOnSubmit(typeObject6);
            db.GetTable<TypeObject>().InsertOnSubmit(typeObject7);
            db.GetTable<TypeObject>().InsertOnSubmit(typeObject8);
            db.GetTable<MemoryNote>().InsertOnSubmit(mn2);
            db.GetTable<MemoryNote>().InsertOnSubmit(mn3);

            db.SubmitChanges();
        }
    }
}
