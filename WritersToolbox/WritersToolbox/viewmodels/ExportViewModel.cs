using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
namespace WritersToolbox.viewmodels
{
    class ExportViewModel : INotifyPropertyChanged
    {
        WritersToolboxDatebase wtb = null;
        public Dictionary<String, IEnumerator> exportData { get; set; }
        private List<MemoryNote> persistNotes = new List<MemoryNote>();
        private List<models.Type> persistTypes = new List<models.Type>();
        private List<TypeObject> persistTypeObjects = new List<TypeObject>();
        private List<Event> persistEvents = new List<Event>();
        private List<Chapter> persistChapters = new List<Chapter>();
        private List<Tome> persistTomes = new List<Tome>();
        private List<Book> persistBooks = new List<Book>();
        private List<BookType> persistBookTypes = new List<BookType>();
        private List<EventTypeObjects> persistEventTypeObjects = new List<EventTypeObjects>();
        public ExportViewModel() {
            wtb = WritersToolboxDatebase.getInstance();
            this.LoadData();
        }

        /// <summary>
        /// Läd alle Entities aus allen Tabellen in öffentlich zugängliche Listen.
        /// </summary>
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
            List<datawrapper.EventTypeObject> allETOs = new List<datawrapper.EventTypeObject>();
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

            var sqlETO = from eto in this.wtb.GetTable<EventTypeObjects>()
                                 select eto;
            foreach (var eto in sqlETO)
            {
                allETOs.Add(new datawrapper.EventTypeObject()
                {
                    EventID = eto.fk_eventID,
                    TypeObjectID = eto.fk_typeObjectID
                });
            }
            this.exportData.Add("eventTypeObjects", allETOs.GetEnumerator());
        }
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Used to notify the app that a property has changed.
        /// </summary>
        /// <param name="propertyName">Property Name</param>

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Fügt ein Entity vom Typ MemoryNote mit den gegebenen Attributen hinzu.
        /// Das Entity wird nicht direkt persistiert, weil die Beziehungen zunächst
        /// durch die Methode restoreDatabase wiederhergestellt werden müssen.
        /// </summary>
        /// <param name="attributes">Attribute des Entities</param>
        public void persistNote(Dictionary<string, string> attributes) {
            MemoryNote mn = new MemoryNote();
            mn.addedDate = DateTime.Parse(attributes["addedDate"]);
            mn.associated = Boolean.Parse(attributes["associated"]);
            mn.contentAudioString = attributes["contentAudioString"];
            mn.ContentImageString = attributes["contentImageString"];
            mn.contentText = attributes["contentText"];
            mn.deleted = Boolean.Parse(attributes["deleted"]);
            //mn.location = attributes["location"];
            mn.memoryNoteID = Int32.Parse(attributes["memoryNoteID"]);
            mn.tags = attributes["tags"];
            mn.title = attributes["title"];
            mn.updatedDate = DateTime.Parse(attributes["updatedDate"]);
            
            mn.obj_Event = new Event() { eventID = Int32.Parse(attributes["eventID"]) };
            mn.obj_TypeObject = new TypeObject() { typeObjectID = Int32.Parse(attributes["typeObjectID"]) };
            Debug.WriteLine("Notiz speichern: " + attributes.ToString());
            this.persistNotes.Add(mn);
        }

        /// <summary>
        /// Fügt ein Entity vom Typ Type mit den gegebenen Attributen hinzu.
        /// Das Entity wird nicht direkt persistiert, weil die Beziehungen zunächst
        /// durch die Methode restoreDatabase wiederhergestellt werden müssen.
        /// </summary>
        /// <param name="attributes">Attribute des Entities</param>
        public void persistType(Dictionary<string, string> attributes) { 
            models.Type t = new models.Type();
            t.color = attributes["color"];
            t.deleted = Boolean.Parse(attributes["deleted"]);
            t.imageString = attributes["imageString"];
            t.title = attributes["title"];
            t.typeID = Int32.Parse(attributes["typeID"]);
            Debug.WriteLine("Typ speichern: " + attributes.ToString());
            this.persistTypes.Add(t);
        }

        /// <summary>
        /// Fügt ein Entity vom Typ TypeObject mit den gegebenen Attributen hinzu.
        /// Das Entity wird nicht direkt persistiert, weil die Beziehungen zunächst
        /// durch die Methode restoreDatabase wiederhergestellt werden müssen.
        /// </summary>
        /// <param name="attributes">Attribute des Entities</param>
        public void persistTypeObject(Dictionary<string, string> attributes) {
            TypeObject to = new TypeObject();
            to.color = attributes["color"];
            to.deleted = Boolean.Parse(attributes["deleted"]);
            to.fk_typeID = Int32.Parse(attributes["typeID"]);
            to.imageString = attributes["imageString"];
            to.name = attributes["name"];
            to.typeObjectID = Int32.Parse(attributes["typeObjectID"]);
            to.used = Boolean.Parse(attributes["used"]);
            Debug.WriteLine("Typobjekt speichern: " + attributes.ToString());
            persistTypeObjects.Add(to);
        }

        /// <summary>
        /// Fügt ein Entity vom Typ Event mit den gegebenen Attributen hinzu.
        /// Das Entity wird nicht direkt persistiert, weil die Beziehungen zunächst
        /// durch die Methode restoreDatabase wiederhergestellt werden müssen.
        /// </summary>
        /// <param name="attributes">Attribute des Entities</param>
        public void persistEvent(Dictionary<string, string> attributes)
        {
            Event e = new Event();
            e.deleted = Boolean.Parse(attributes["deleted"]);
            e.eventID = Int32.Parse(attributes["eventID"]);
            e.finaltext = attributes["finaltext"];
            e.fk_chapterID = Int32.Parse(attributes["chapterID"]);
            e.orderInChapter = Int32.Parse(attributes["orderInChapter"]);
            e.title = attributes["title"];
            persistEvents.Add(e);
        }

        /// <summary>
        /// Fügt ein Entity vom Typ Chapter mit den gegebenen Attributen hinzu.
        /// Das Entity wird nicht direkt persistiert, weil die Beziehungen zunächst
        /// durch die Methode restoreDatabase wiederhergestellt werden müssen.
        /// </summary>
        /// <param name="attributes">Attribute des Entities</param>
        public void persistChapter(Dictionary<string, string> attributes)
        {
            Chapter c = new Chapter();
            c.addedDate = DateTime.Parse(attributes["addedDate"]);
            c.chapterID = Int32.Parse(attributes["chapterID"]);
            c.chapterNumber = Int32.Parse(attributes["chapterNumber"]);
            c.deleted = Boolean.Parse(attributes["deleted"]);
            c.title = attributes["title"];
            c.updatedDate = DateTime.Parse(attributes["updatedDate"]);
            c.obj_tome = new Tome() { tomeID = Int32.Parse(attributes["tomeID"]) };
            persistChapters.Add(c);
        }

        /// <summary>
        /// Fügt ein Entity vom Typ Tome mit den gegebenen Attributen hinzu.
        /// Das Entity wird nicht direkt persistiert, weil die Beziehungen zunächst
        /// durch die Methode restoreDatabase wiederhergestellt werden müssen.
        /// </summary>
        /// <param name="attributes">Attribute des Entities</param>
        public void persistTome(Dictionary<string, string> attributes) {
            Tome t = new Tome();
            t.addedDate = DateTime.Parse(attributes["addedDate"]);
            t.deleted = Boolean.Parse(attributes["deleted"]);
            t.information = Int32.Parse(attributes["information"]);
            t.title = attributes["title"];
            t.tomeID = Int32.Parse(attributes["tomeID"]);
            t.tomeNumber = Int32.Parse(attributes["tomeNumber"]);
            t.updatedDate = DateTime.Parse(attributes["updatedDate"]);
            t.obj_book = new Book() { bookID = Int32.Parse(attributes["bookID"]) };
            persistTomes.Add(t);
        }

        /// <summary>
        /// Fügt ein Entity vom Typ Book mit den gegebenen Attributen hinzu.
        /// Das Entity wird nicht direkt persistiert, weil die Beziehungen zunächst
        /// durch die Methode restoreDatabase wiederhergestellt werden müssen.
        /// </summary>
        /// <param name="attributes">Attribute des Entities</param>
        public void persistBook(Dictionary<string, string> attributes)
        {
            Book b = new Book();
            b.addedDate = DateTime.Parse(attributes["addedDate"]);
            b.bookID = Int32.Parse(attributes["bookID"]);
            b.deleted = Boolean.Parse(attributes["deleted"]);
            b.name = attributes["name"];
            b.updatedDate = DateTime.Parse(attributes["updatedDate"]);
            b.obj_bookType = new BookType() { bookTypeID = Int32.Parse(attributes["bookTypeID"]) };
            persistBooks.Add(b);
        }

        /// <summary>
        /// Fügt ein Entity vom Typ BookType mit den gegebenen Attributen hinzu.
        /// Das Entity wird nicht direkt persistiert, weil die Beziehungen zunächst
        /// durch die Methode restoreDatabase wiederhergestellt werden müssen.
        /// </summary>
        /// <param name="attributes">Attribute des Entities</param>
        public void persistBookType(Dictionary<string, string> attributes)
        {
            BookType bt = new BookType();
            bt.addedDate = DateTime.Parse(attributes["addedDate"]);
            bt.bookTypeID = Int32.Parse(attributes["bookTypeID"]);
            bt.deleted = Boolean.Parse(attributes["deleted"]);
            bt.name = attributes["name"];
            bt.numberOfChapter = Int32.Parse(attributes["numberOfChapter"]);
            bt.updatedDate = DateTime.Parse(attributes["updatedDate"]);
            persistBookTypes.Add(bt);
        }

        /// <summary>
        /// Fügt ein Entity vom Typ EventTypeObject (N:M Zwischentabelle) mit den gegebenen Attributen hinzu.
        /// Das Entity wird nicht direkt persistiert, weil die Beziehungen zunächst
        /// durch die Methode restoreDatabase wiederhergestellt werden müssen.
        /// </summary>
        /// <param name="attributes">Attribute des Entities</param>
        public void persistEventTypeObject(Dictionary<string, string> attributes) {
            EventTypeObjects eto = new EventTypeObjects();
            eto.fk_eventID = Int32.Parse(attributes["eventID"]);
            eto.fk_typeObjectID = Int32.Parse(attributes["typeObjectID"]);
            this.persistEventTypeObjects.Add(eto);
        }

        /// <summary>
        /// Gibt die Namen der Bilder aus den relevanten Spalten aller Entities zurück,
        /// damit diese importiert werden können.
        /// </summary>
        /// <returns>Liste von Dateinamen</returns>
        public List<string> getImageNames() {
            List<string> ret = new List<string>();
            IEnumerator e = this.persistNotes.GetEnumerator();
            while (e.MoveNext()) {
                String[] imgPaths = ((MemoryNote)e.Current).ContentImageString.Split('|');
                for (int i = 0; i < imgPaths.Length - 1; i++) {
                    if(!imgPaths[i].Equals("")) ret.Add(imgPaths[i]);
                }
            }

            e = this.persistTypeObjects.GetEnumerator();
            while (e.MoveNext()) {
                if (!((TypeObject)e.Current).imageString.Equals("")) ret.Add(((TypeObject)e.Current).imageString);
            }

            e = this.persistTypes.GetEnumerator();
            while (e.MoveNext())
            {
                if (!((models.Type)e.Current).imageString.Equals("")) ret.Add(((models.Type)e.Current).imageString);
            }
            return ret.Distinct().ToList();
        }

        /// <summary>
        /// Gibt die Namen aller Audiodateien aus den Spalten aller Entities zurück.
        /// </summary>
        /// <returns>Liste von Dateinamen</returns>
        public List<string> getAudioNames() {
            List<string> ret = new List<string>();
            IEnumerator e = this.persistNotes.GetEnumerator();
            while (e.MoveNext())
            {
                String[] audioPaths = ((MemoryNote)e.Current).contentAudioString.Split('|');
                for (int i = 0; i < audioPaths.Length - 1; i++)
                {
                    if (!audioPaths[i].Equals("")) ret.Add(audioPaths[i].Split(';')[0]);
                }
            }
            return ret.Distinct().ToList();
        }

        /// <summary>
        /// Updatet die Dateinamen in den Entities nachdem die zugehörigen Dateien importiert wurden.
        /// Dateiname -> Absoluter Pfad
        /// </summary>
        public void updateFilePaths() {
            IEnumerator e = this.persistNotes.GetEnumerator();
            while (e.MoveNext()) {
                String[] oldNames = ((MemoryNote)e.Current).ContentImageString.Split('|');
                String newString = "";
                for (int i = 0; i < oldNames.Count() - 1; i++) {
                    newString += getPathFromFilename(oldNames[i]) + "|";
                }
                ((MemoryNote)e.Current).ContentImageString = newString;
            }
            e = this.persistTypes.GetEnumerator();
            while (e.MoveNext()) {
                String newString = getPathFromFilename(((models.Type)e.Current).imageString);
                ((models.Type)e.Current).imageString = newString;
            }
            e = this.persistTypeObjects.GetEnumerator();
            while (e.MoveNext()) {
                String newString = getPathFromFilename(((TypeObject)e.Current).imageString);
                ((TypeObject)e.Current).imageString = newString;
            }
        }

        /// <summary>
        /// Regeneriert zunächst die Beziehungen zwischen den Entities und persistiert diese
        /// anschließend.
        /// </summary>
        public void restoreDatabase() {
            IEnumerator<BookType> enumBookType = this.persistBookTypes.GetEnumerator();
            while (enumBookType.MoveNext()) {
                Debug.WriteLine(enumBookType.Current.bookTypeID);
                this.wtb.GetTable<BookType>().InsertOnSubmit(enumBookType.Current);
            }

            IEnumerator<Book> enumBooks = this.persistBooks.GetEnumerator();
            while (enumBooks.MoveNext()) {
                Book b = enumBooks.Current;
                b.obj_bookType = this.persistBookTypes.Where(BT => BT.bookTypeID == b.obj_bookType.bookTypeID).Single();
                this.wtb.GetTable<Book>().InsertOnSubmit(b);
            }
            IEnumerator<Tome> enumTome = this.persistTomes.GetEnumerator();
            while (enumTome.MoveNext()) {
                Tome t = enumTome.Current;
                t.obj_book = this.persistBooks.Where(B => B.bookID == t.obj_book.bookID).Single();
                this.wtb.GetTable<Tome>().InsertOnSubmit(t);
            }
            IEnumerator<Chapter> enumChapter = this.persistChapters.GetEnumerator();
            while (enumChapter.MoveNext()) {
                Chapter c = enumChapter.Current;
                c.obj_tome = this.persistTomes.Where(T => T.tomeID == c.obj_tome.tomeID).Single();
                this.wtb.GetTable<Chapter>().InsertOnSubmit(c);
            }
            IEnumerator<Event> enumEvent = this.persistEvents.GetEnumerator();
            while (enumEvent.MoveNext()) {
                Event e = enumEvent.Current;
                e.obj_Chapter = this.persistChapters.Where(C => C.chapterID == e.fk_chapterID).Single();
                this.wtb.GetTable<Event>().InsertOnSubmit(e);
            }
            this.wtb.GetTable<models.Type>().InsertAllOnSubmit(this.persistTypes);

            IEnumerator<TypeObject> enumTO = this.persistTypeObjects.GetEnumerator();
            while (enumTO.MoveNext()) {
                TypeObject to = enumTO.Current;
                to.obj_Type = this.persistTypes.Where(T => T.typeID == to.fk_typeID).Single();
                this.wtb.GetTable<TypeObject>().InsertOnSubmit(to);
            }

            IEnumerator<EventTypeObjects> enumETO = this.persistEventTypeObjects.GetEnumerator();
            while (enumETO.MoveNext()) {
                EventTypeObjects eto = enumETO.Current;
                eto.obj_Event = this.persistEvents.Where(E => E.eventID == eto.fk_eventID).Single();
                eto.obj_typeObject = this.persistTypeObjects.Where(TO => TO.typeObjectID == eto.fk_typeObjectID).Single();
                this.wtb.GetTable<EventTypeObjects>().InsertOnSubmit(eto);
            }

            IEnumerator<MemoryNote> enumNote = this.persistNotes.GetEnumerator();
            while (enumNote.MoveNext()) {
                MemoryNote mn = enumNote.Current;
                if (mn.obj_Event != null && mn.obj_Event.eventID != -1)
                {
                    mn.obj_Event = this.persistEvents.Where(E => E.eventID == mn.obj_Event.eventID).Single();
                }
                else {
                    mn.obj_Event = null;
                }

                if (mn.obj_TypeObject != null && mn.obj_TypeObject.typeObjectID != -1)
                {
                    mn.obj_TypeObject = this.persistTypeObjects.Where(TO => TO.typeObjectID == mn.obj_TypeObject.typeObjectID).Single();
                }
                else {
                    mn.obj_TypeObject = null;
                }
                this.wtb.GetTable<MemoryNote>().InsertOnSubmit(mn);
            }
            this.wtb.SubmitChanges();
        } 

        /// <summary>
        /// Löscht die Datenbank und erstellt danach eine neue.
        /// </summary>
        public void deleteOldDB() {
            this.wtb.DeleteDatabase();
            this.wtb = WritersToolboxDatebase.forceNewInstance();
            this.wtb.CreateDatabase();
        }

        /// <summary>
        /// Gibt den absoluten Pfad eines Bildes anhand des Dateinamens zurück.
        /// </summary>
        /// <param name="filename">Dateiname</param>
        /// <returns>Absoluter Pfad zur Datei</returns>
        public String getPathFromFilename(String filename) {
            String path = "";
            MediaLibrary ml = new MediaLibrary();
            var arr = ml.Pictures.ToArray();
            var resultset = ml.Pictures.Where(P => P.Name.Equals(filename));
            if (resultset.Count() > 0) {
                path = resultset.First().GetPath();
            }
            return path;
        }
    }
}
