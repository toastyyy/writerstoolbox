using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
namespace WritersToolbox.viewmodels
{
    class ExportViewModel : INotifyPropertyChanged
    {
        WritersToolboxDatebase wtb = null;
        public Dictionary<String, IEnumerator> exportData { get; set; }

        public ExportViewModel() {
            wtb = WritersToolboxDatebase.getInstance();
            this.LoadData();
        }

        public void LoadData() {
            this.exportData = new Dictionary<string, IEnumerator>();
            
            // erst alle buchtypen speichern
            List<datawrapper.BookType> allBookTypes = new List<datawrapper.BookType>();
            List<datawrapper.Book> allBooks = new List<datawrapper.Book>();
            List<datawrapper.Tome> allTomes = new List<datawrapper.Tome>();
            List<datawrapper.Chapter> allChapters = new List<datawrapper.Chapter>();
            List<datawrapper.Event> allEvents = new List<datawrapper.Event>();
            List<datawrapper.Type> allTypes = new List<datawrapper.Type>();
            List<datawrapper.TypeObject> allTypeObjects = new List<datawrapper.TypeObject>();
            List<datawrapper.MemoryNote> allNotes = new List<datawrapper.MemoryNote>();

            var sqlBookTypes = from bt in this.wtb.GetTable<BookType>()
                               select bt;
            foreach(var bookType in sqlBookTypes) {                
                allBookTypes.Add(new datawrapper.BookType() { 
                    addedDate = bookType.addedDate,
                    bookTypeID = bookType.bookTypeID,
                    name = bookType.name,
                    numberOfChapter = bookType.numberOfChapter,
                    updatedDate = bookType.updatedDate
                });
            }
            this.exportData.Add("bookTypes", allBookTypes.GetEnumerator());

            var sqlBooks = from b in this.wtb.GetTable<Book>()
                           select b;
            foreach (var book in sqlBooks) {
                allBooks.Add(new datawrapper.Book()
                {
                    addedDate = book.addedDate,
                    bookID = book.bookID,
                    bookType = new datawrapper.BookType() { bookTypeID = book.obj_bookType.bookTypeID },
                    name = book.name,
                    updatedDate = book.updatedDate
                });
            }
            this.exportData.Add("books", allBooks.GetEnumerator());

            var sqlTomes = from t in this.wtb.GetTable<Tome>()
                           select t;
            foreach (var tome in sqlTomes)
            {
                allTomes.Add(new datawrapper.Tome()
                {
                    addedDate = tome.addedDate,
                    book = new datawrapper.Book() { bookID = tome.obj_book.bookID },
                    deleted = tome.deleted,
                    information = tome.information,
                    title = tome.title,
                    tomeID = tome.tomeID,
                    tomeNumber = tome.tomeNumber,
                    updatedDate = tome.updatedDate
                });
            }
            this.exportData.Add("tomes", allTomes.GetEnumerator());

            var sqlChapters = from c in this.wtb.GetTable<Chapter>()
                           select c;
            foreach (var chapter in sqlChapters)
            {
                allChapters.Add(new datawrapper.Chapter()
                {
                    addedDate = chapter.addedDate,
                    tome = new datawrapper.Tome() { tomeID = chapter.obj_tome.tomeID },
                    chapterID = chapter.chapterID,
                    chapterNumber = chapter.chapterNumber,
                    deleted = chapter.deleted,
                    title = chapter.title,
                    updatedDate = chapter.updatedDate
                });
            }
            this.exportData.Add("chapters", allChapters.GetEnumerator());

            var sqlEvents = from e in this.wtb.GetTable<Event>()
                              select e;
            foreach (var ev in sqlEvents)
            {
                allEvents.Add(new datawrapper.Event()
                {
                    deleted = ev.deleted,
                    chapter = new datawrapper.Chapter() { chapterID = ev.obj_Chapter.chapterID },
                    eventID = ev.eventID,
                    finaltext = ev.finaltext,
                    orderInChapter = ev.orderInChapter,
                    title = ev.title
                });
            }
            this.exportData.Add("events", allEvents.GetEnumerator());

            var sqlNotes = from mn in this.wtb.GetTable<MemoryNote>()
                            select mn;
            foreach (var mn in sqlNotes)
            {
                allNotes.Add(new datawrapper.MemoryNote()
                {
                    addedDate = mn.addedDate,
                    associated = mn.associated,
                    contentAudioString = mn.contentAudioString,
                    contentImageString = mn.ContentImageString,
                    contentText = mn.contentText,
                    deleted = mn.deleted,
                    location = mn.location,
                    tags = mn.tags,
                    title = mn.title,
                    updatedDate = mn.updatedDate,
                    fk_eventID = (mn.obj_Event != null) ? mn.obj_Event.eventID : -1,
                    fk_typeObjectID = (mn.obj_TypeObject != null) ? mn.obj_TypeObject.typeObjectID : -1,
                    memoryNoteID = mn.memoryNoteID
                });
            }
            this.exportData.Add("notes", allNotes.GetEnumerator());

            var sqlTypes = from t in this.wtb.GetTable<models.Type>()
                           select t;
            foreach (var t in sqlTypes)
            {
                allTypes.Add(new datawrapper.Type()
                {
                    color = t.color,
                    imageString = t.imageString,
                    title = t.title,
                    typeID = t.typeID
                });
            }
            this.exportData.Add("types", allTypes.GetEnumerator());

            var sqlTypeObjects = from to in this.wtb.GetTable<TypeObject>()
                           select to;
            foreach (var to in sqlTypeObjects)
            {
                allTypeObjects.Add(new datawrapper.TypeObject()
                {
                    color = to.color,
                    imageString = to.imageString,
                    name = to.name,
                    type = new datawrapper.Type() { typeID = to.obj_Type.typeID },
                    typeObjectID = to.typeObjectID,
                    used = to.used
                });
            }
            this.exportData.Add("typeObjects", allTypeObjects.GetEnumerator());
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

        public void addPersistentObject() { }
    }
}
