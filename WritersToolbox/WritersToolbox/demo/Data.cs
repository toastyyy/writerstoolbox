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

            BookType bt2 = new BookType();
            bt2.name = "Gedichte";
            bt2.updatedDate = DateTime.Now;
            bt2.addedDate = DateTime.Now;
            bt2.numberOfChapter = 3;

            BookType bt3 = new BookType();
            bt3.name = "Kurzgeschichte";
            bt3.updatedDate = DateTime.Now;
            bt3.addedDate = DateTime.Now;
            bt3.numberOfChapter = 3;

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
                obj_book = b,
                information = 1
            };

            Tome t2 = new Tome()
            {
                title = "Kammer des Schreckens",
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                obj_book = b,
                information = 2

            };

            Tome t3 = new Tome()
            {
                title = "Die Gefährten",
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                obj_book = b2,
                information = 3
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

            Event e3 = new Event()
            {
                title = "Haus der Dursleys",
                obj_Chapter = c2
            };

            Event e4 = new Event()
            {
                title = "Bahnhof",
                obj_Chapter = c2
            };
            Event e5 = new Event()
            {
                title = "Haus der Dursleys",
                obj_Chapter = c3
            };

            Event e6 = new Event()
            {
                title = "Bahnhof",
                obj_Chapter = c3
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
                color = "#FF32CD32",
                imageString = "../icons/character_round_icon.png"
            };

            WritersToolbox.models.Type type2 = new WritersToolbox.models.Type()
            {
                title = "Handlungsort",
                color = "#FF32CD32"
            };

            WritersToolbox.models.Type type3 = new WritersToolbox.models.Type()
            {
                title = "Formeln",
                color = "#FF32CD32"
            };

            WritersToolbox.models.Type type4 = new WritersToolbox.models.Type()
            {
                title = "Gruppierungen",
                color = "#FF32CD32"
            };

            TypeObject typeObject1 = new TypeObject() 
            { 
                name = "Harry Potter",
                obj_Type = type1,
                used = true,
                color = "#0020B2AA",
                imageString = "../icons/character_round_icon.png",
                obj_Event = e2
            };

            TypeObject typeObject2= new TypeObject()
            {
                name = "Ron Weasley",
                obj_Type = type1,
                used = true,
                color = "#0020B2AA",
                imageString = "../icons/character_round_icon.png",
                obj_Event = e2
            };

            TypeObject typeObject3 = new TypeObject()
            {
                name = "Hogwarts",
                obj_Type = type2,
                color = "#FF20B2AA",
                obj_Event = e2
            };

            TypeObject typeObject4 = new TypeObject()
            {
                name = "Zauberwald",
                obj_Type = type2,
                color = "#FF20B2AA",
                obj_Event = e2
            };

            TypeObject typeObject5 = new TypeObject()
            {
                name = "Frodo Beutlin",
                obj_Type = type1,
                color = "#0020B2AA",
                imageString = "../icons/character_round_icon.png",
                obj_Event = e1
            };

            TypeObject typeObject6 = new TypeObject()
            {
                name = "Bilbo Beutlin",
                obj_Type = type1,
                color = "#0020B2AA",
                imageString = "../icons/character_round_icon.png",
                obj_Event = e1
            };

            TypeObject typeObject7 = new TypeObject()
            {
                name = "Gandalf",
                obj_Type = type1,
                color = "#0020B2AA",
                imageString = "../icons/character_round_icon.png",
                obj_Event = e1
            };
            TypeObject typeObject8 = new TypeObject()
            {
                name = "Gollum",
                obj_Type = type1,
                color = "#0020B2AA",
                imageString = "../icons/character_round_icon.png",
                obj_Event = e1,
                
            };

            MemoryNote mn2 = new MemoryNote()
            {
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                contentText = "lorem ipsum dolor sit amet",
                title = "aussehen",
                associated = true,
                tags = "harry|potter|aussehen|narbe|haare|klein",
                obj_TypeObject = typeObject1,
                obj_Event = e1
            };

            MemoryNote mn3 = new MemoryNote()
            {
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                contentText = "blablub trololololol",
                title = "testnotiz an harry",
                associated = true,
                tags = "harry|potter|test",
                obj_TypeObject = typeObject1,
                obj_Event = e1
            };

            MemoryNote mn4 = new MemoryNote()
            {
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                contentText = "blablub trololololol",
                title = "testnotiz an harry",
                associated = true,
                tags = "harry|potter|test",
                obj_TypeObject = typeObject1,
                obj_Event = e1
            };

            MemoryNote mn5 = new MemoryNote()
            {
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                contentText = "blablub trololololol",
                title = "testnotiz an harry",
                associated = true,
                tags = "harry|potter|test",
                obj_TypeObject = typeObject1,
                obj_Event = e1
            };

            MemoryNote mn6 = new MemoryNote()
            {
                addedDate = DateTime.Now,
                updatedDate = DateTime.Now,
                contentText = "blablub trololololol",
                title = "testnotiz an harry",
                associated = true,
                tags = "harry|potter|test",
                obj_TypeObject = typeObject1,
                obj_Event = e1
            };

            db.GetTable<BookType>().InsertOnSubmit(bt);
            db.GetTable<BookType>().InsertOnSubmit(bt2);
            db.GetTable<BookType>().InsertOnSubmit(bt3);
            db.GetTable<Book>().InsertOnSubmit(b);
            db.GetTable<Book>().InsertOnSubmit(b2);
            db.GetTable<Tome>().InsertOnSubmit(t1);
            db.GetTable<Tome>().InsertOnSubmit(t2);
            db.GetTable<Tome>().InsertOnSubmit(t3);
            db.GetTable<Chapter>().InsertOnSubmit(c1);
            db.GetTable<Chapter>().InsertOnSubmit(c2);
            db.GetTable<WritersToolbox.models.Event>().InsertOnSubmit(e1);
            db.GetTable<WritersToolbox.models.Event>().InsertOnSubmit(e2);
            db.GetTable<WritersToolbox.models.Event>().InsertOnSubmit(e3);
            db.GetTable<WritersToolbox.models.Event>().InsertOnSubmit(e4);
            db.GetTable<WritersToolbox.models.Event>().InsertOnSubmit(e5);
            db.GetTable<WritersToolbox.models.Event>().InsertOnSubmit(e6);
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
            db.GetTable<MemoryNote>().InsertOnSubmit(mn4);
            db.GetTable<MemoryNote>().InsertOnSubmit(mn5);
            db.GetTable<MemoryNote>().InsertOnSubmit(mn6);
            for (int i = 0; i < 20; i++)
            {
                MemoryNote mn = new MemoryNote()
                {
                    addedDate = DateTime.Now,
                    updatedDate = DateTime.Now,
                    contentText = "Mr. and Mrs. Dursley, of number four, Privet Drive, were proud to say that they were perfectly normal," +
                                    "thank you very much. They were the last people you'd expect to be involved in anything strange or mysterious," +
                                    "because they just didn't hold with such nonsense. Mr. Dursley was the director of a firm called Grunnings, which made drills. He was a big, beefy man with hardly any neck," + 
                                    "although he did have a very large mustache. Mrs. Dursley was thin and blonde and had nearly twice the usual amount of neck, which came in very useful as she spent so much of her" + 
                                    "time craning over garden fences, spying on the neighbors. The Dursleys had a small son called Dudley and in their opinion there was no finer boy anywhere." + 
                                    "The Dursleys had everything they wanted, but they also had a secret, and their greatest fear was that somebody would discover it. They didn't think they could bear it" +
                "if anyone found out about the Potters. Mrs. Potter was Mrs. Dursley's sister, but they hadn't met for several years; in fact, Mrs. Dursley pretended she didn't have a sister, because her sister and" + 
                "her good-for-nothing husband were as unDursleyish as it was possible to be. The Dursleys shuddered to think what the neighbors would say if the Potters arrived in the street. The Dursleys knew that the" + 
                "Potters had a small son, too, but they had never even seen him. This boy was another good reason for keeping the Potters away; they didn't want Dudley mixing with a child like that. " + 
                "When Mr. and Mrs. Dursley woke up on the dull, gray Tuesday our story starts, there was nothing about the cloudy sky outside to suggest that strange and mysterious things would soon be happening " + 
                "all over the country. Mr. Dursley hummed as he picked out his most boring tie for work, and Mrs. Dursley gossiped away happily as she wrestled a screaming Dudley into his high chair. None of them noticed a large, tawny owl flutter past the window.",
　　                title = i.ToString(),
                    associated = true,
                    tags = "harry|potter|test",
                    obj_TypeObject = typeObject1,
                    obj_Event = e1
                };
                db.GetTable<MemoryNote>().InsertOnSubmit(mn);
            }

            for (int i = 0; i < 50; i++)
            {
                MemoryNote mn = new MemoryNote()
                {
                    addedDate = DateTime.Now,
                    updatedDate = DateTime.Now,
                    contentText = "Mr. and Mrs. Dursley, of number four, Privet Drive, were proud to say that they were perfectly normal," +
                                    "thank you very much. They were the last people you'd expect to be involved in anything strange or mysterious," +
                                    "because they just didn't hold with such nonsense. Mr. Dursley was the director of a firm called Grunnings, which made drills. He was a big, beefy man with hardly any neck," +
                                    "although he did have a very large mustache. Mrs. Dursley was thin and blonde and had nearly twice the usual amount of neck, which came in very useful as she spent so much of her" +
                                    "time craning over garden fences, spying on the neighbors. The Dursleys had a small son called Dudley and in their opinion there was no finer boy anywhere." +
                                    "The Dursleys had everything they wanted, but they also had a secret, and their greatest fear was that somebody would discover it. They didn't think they could bear it" +
                "if anyone found out about the Potters. Mrs. Potter was Mrs. Dursley's sister, but they hadn't met for several years; in fact, Mrs. Dursley pretended she didn't have a sister, because her sister and" +
                "her good-for-nothing husband were as unDursleyish as it was possible to be. The Dursleys shuddered to think what the neighbors would say if the Potters arrived in the street. The Dursleys knew that the" +
                "Potters had a small son, too, but they had never even seen him. This boy was another good reason for keeping the Potters away; they didn't want Dudley mixing with a child like that. " +
                "When Mr. and Mrs. Dursley woke up on the dull, gray Tuesday our story starts, there was nothing about the cloudy sky outside to suggest that strange and mysterious things would soon be happening " +
                "all over the country. Mr. Dursley hummed as he picked out his most boring tie for work, and Mrs. Dursley gossiped away happily as she wrestled a screaming Dudley into his high chair. None of them noticed a large, tawny owl flutter past the window.",
                    title = i.ToString(),
                    associated = true,
                    tags = "Ron|Weasley|test",
                    obj_TypeObject = typeObject2,
                    obj_Event = e1
                };
                db.GetTable<MemoryNote>().InsertOnSubmit(mn);
            }

            db.SubmitChanges();
        }
    }
}
