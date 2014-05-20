﻿using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
using WritersToolbox.Resources;

namespace WritersToolbox.viewmodels
{
    class TomeDetailsViewModel : INotifyPropertyChanged
    {
        //Datenbank
        private WritersToolboxDatebase wtb;
        //Chapter als Entity object
        private models.Chapter obj_chapter;
        //test
        datawrapper.Book bo;
        private models.Event obj_event;
        //TypeObjecte, um die beteiligte Objekte zu zeigen.-Tabelle
        private Table<TypeObject> tableTypeObject;
        //Event-Tabelle
        private Table<Event> tableEvent;
        //Chapter-Tabelle
        private Table<Chapter> tableChapter;
        //Tome-Tabelle
        private Table<Tome> tableTome;
        //Book-Tabelle
        private Table<Book> tableBook;

        private Table<MemoryNote> tableMemoryNote;
        //Band
        public Tome tome {get;set;}
        //List der beteiligten Typeobjekte, die mit dem View verbunden wird.
        private ObservableCollection<datawrapper.TypeObject> _typeObjects;
        public ObservableCollection<datawrapper.TypeObject> typeObjects { get { return _typeObjects; } }

        private bool on_off_Status;

        //Structur
        private ObservableCollection<datawrapper.Chapter> _structur;
        public ObservableCollection<datawrapper.Chapter> structur { get { return _structur; } }

        /// <summary>
        /// Defaultkonstruktor
        /// </summary>
        public TomeDetailsViewModel(int tomeID)
        {
            try
            {

                //Verbindung zur Dantenbank aufrufen.
                wtb = WritersToolboxDatebase.getInstance();
                //Referenz zu TypeObjectTable aufrufen.
                tableTypeObject = wtb.GetTable<TypeObject>();
                //Andere Referente aufrufen, um die Verknüpfung zu den beteiligten Typeobjekte herzustellen.
                tableChapter = wtb.GetTable<Chapter>(); ;
                tableEvent = wtb.GetTable<Event>(); ;
                tableTome = wtb.GetTable<Tome>();
                tableBook = wtb.GetTable<Book>();
                tableMemoryNote = wtb.GetTable<MemoryNote>();
                _typeObjects = getAssociatedTypeObjects(tomeID);
                //Aktualles Band.
                tome = tableTome.Single(t => t.tomeID == tomeID);
                _structur = getStructure();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Kode um die Anzeige der Information eines Bandes einzustellen.
        /// </summary>
        /// <returns>Kode</returns>
        public int getInformation()
        {
            return tome.information;
        }

        public bool isExistNoteInEvent(int eventID, string title)
        {
            return wtb.GetTable<models.MemoryNote>().Count(_m => _m.obj_Event.eventID == eventID && _m.title == title) == 1;
        }

        public void removeNote(int eventid, string title)
        {
            models.MemoryNote tempNote = (from _n in tableMemoryNote
                                         where _n.obj_Event.eventID == eventid && _n.title == title
                                         select _n).First();

            if (tempNote.contentAudioString != null)
            {
                string[] tokens = tempNote.contentAudioString.Split('|');

                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    foreach (string item in tokens)
                    {
                        if (isoStore.FileExists(item))
                        {
                            isoStore.DeleteFile(item);
                        }
                    }
                }
            }
            tableMemoryNote.DeleteOnSubmit(tempNote);
            wtb.SubmitChanges();
        }

        /// <summary>
        /// liefert Anzahl Kapitel durch abfrage in der Datenbank zurück.
        /// </summary>
        /// <returns>liefert Anzahl Kapitel zurück</returns>
        public int getNumberOfChapters()
        {
            return tableChapter.Count(c => (c.obj_tome.tomeID == tome.tomeID && !c.deleted));
        }

        /// <summary>
        /// liefert Anzahl Ereignisse durch abfrage in der Datenbank zurück.
        /// </summary>
        /// <returns>liefert Anzahl Ereignisse zurück</returns>
        public int getNumberOfEvents()
        {
            return tableEvent.Count(e => (e.obj_Chapter.obj_tome.tomeID == tome.tomeID && !e.deleted));
        }

        public String getBookTitle()
        {
            
            var sqlBook = from b in tableBook
                       where b.bookID == tome.obj_book.bookID
                       select b;
             foreach (var book in sqlBook)
            {
                bo = new datawrapper.Book()
                {
                    name = book.name
                };
                
            }
             return bo.name;
        }
        /// <summary>
        /// liefert Anzahl Typeobjekte durch abfrage in der Datenbank zurück.
        /// </summary>
        /// <returns>liefert Anzahl Typeobjekte zurück</returns>
        public int getNumberOfTypeObjects()
        {          
            return typeObjects.Count;
        }

        private int numberOfSigns;
        private int numberOfTokens;

        /// <summary>
        /// liefert die Anzahl an Zeichen aller Finaltexte eines Bandes
        /// </summary>
        /// <returns></returns>
        public int getsNumberOfSignsFinaltext()
        {
            //FRAGE: kann ich "countFinaltext()" zentraler i.wo einmal aufrufen?? quasi immer nur wenn sich die infos überhaupt ändern?
            //noch eine FRAGE: "countFinaltext()" ist fast das gleich wie "getstrucutur" (zugriff auf die datenbank, foreach Durchlauf etc), ich gehe halt nurnoch auf die finaltexte ein, sollte man das in einem machen?
            //countFinaltext();
            return numberOfSigns;
        }

        /// <summary>
        /// liefert die Anzahl an Wörtern aller Finaltexte eines Bandes
        /// </summary>
        /// <returns></returns>
        public int getNumberOfWords()
        {
            //FRAGE: kann ich "countFinaltext()" zentraler i.wo einmal aufrufen?? quasi immer nur wenn sich die infos überhaupt ändern?
            //noch eine FRAGE: "countFinaltext()" ist fast das gleich wie "getstrucutur" (zugriff auf die datenbank, foreach Durchlauf etc), ich gehe halt nurnoch auf die finaltexte ein, sollte man das in einem machen?
            //countFinaltext();
            return numberOfTokens;
        }
        

        /// <summary>
        /// Kode der Information eines Bands in Datenbank aktualisieren.
        /// </summary>
        /// <param name="information">InformationsKode</param>
        public void updateInformation(int information)
        {
            //models.Tome tempTome = tableTome.Single(tome => tome.tomeID == tomeID);
            tome.information = information;
            wtb.SubmitChanges();
        }

        /// <summary>
        /// Rückgabe der List der beteiligten TypeObjekte.
        /// </summary>
        /// <param name="bookID">ausgewähltes Buch</param>
        /// <returns>beteiligte Typeobjekte</returns>
        private ObservableCollection<datawrapper.TypeObject> getAssociatedTypeObjects(int tomeID)
        {
            //Abfrage auf beteiligte Typeobjekte in einem Band.
            var typeObjects = from typeObject in tableTypeObject
                                 where typeObject.events.Any(e => e.obj_Event.obj_Chapter.obj_tome.tomeID == tomeID)
                                select typeObject;
            //Zwischenspeicher der beteiligten Typeobjekte.
            ObservableCollection<datawrapper.TypeObject> associatedTypeObject = null;

            if (typeObjects != null)
            {
                //Referenz übergeben.
                associatedTypeObject = new ObservableCollection<datawrapper.TypeObject>();
                //Durch die Tabelle durchlaufen.
                foreach (TypeObject item in typeObjects)
                {
                    //Angehängte Notizen zu einem TypeObjekt auslesen und in einer List speichern.
                    ObservableCollection<datawrapper.MemoryNote> tempMemoryNoteList =
                        new ObservableCollection<datawrapper.MemoryNote>();

                    foreach (var tempNote in item.notes)
                    {
                        //Notiz auslesen.
                        datawrapper.MemoryNote _tempNote = new datawrapper.MemoryNote()
                        {
                            addedDate = tempNote.addedDate,
                            associated = tempNote.associated,
                            contentAudioString = tempNote.contentAudioString == null ? "" : tempNote.contentAudioString,
                            contentImageString = tempNote.ContentImageString == null ? "" : tempNote.ContentImageString,
                            contentText = tempNote.contentText,
                            deleted = tempNote.deleted,
                            fk_eventID = tempNote.obj_Event == null ? 0 : tempNote.obj_Event.eventID == null ? 0 : tempNote.obj_Event.eventID ,
                            fk_typeObjectID = tempNote.obj_TypeObject == null ? 0 : tempNote.obj_TypeObject.typeObjectID == null ? 0 : tempNote.obj_TypeObject.typeObjectID,
                            memoryNoteID = tempNote.memoryNoteID,
                            tags = tempNote.tags,
                            title = tempNote.title,
                            updatedDate = tempNote.updatedDate
                        };
                        //Notiz in der List speichern.
                        tempMemoryNoteList.Add(_tempNote);
                    }
                    //Type des TypeObjektes auslesen.
                    //Um zu wissen, zu welchem Type das TypeObjekt gehört.
                    datawrapper.Type tempType = new datawrapper.Type()
                    {
                        color = item.obj_Type.color,
                        imageString = item.obj_Type.imageString,
                        title = item.obj_Type.title,
                        typeID = item.obj_Type.typeID,
                        typeObjects = null  //Man braucht nicht die TypeObjecte eines Types zu wissen 
                        //aber umgekehrt schon.
                    };

                    //TypeObject erstellen.
                    datawrapper.TypeObject tempTypeObject = new datawrapper.TypeObject()
                    {
                        typeObjectID = item.typeObjectID,
                        name = item.name,
                        color = item.color,
                        imageString = item.imageString,
                        used = item.used,
                        notes = tempMemoryNoteList,
                        type = tempType

                    };
                    //Erstelltes Typeobjekt in der List hinzufügen.
                    associatedTypeObject.Add(tempTypeObject);
                }
            }
            return associatedTypeObject;
        }

        //TODO
        /// <summary>
        /// liefert die Struktur des Bandes zurück
        ///     Chapter -> Events.
        /// </summary>
        /// <returns>Struktur in ObservableCollection</returns>
        private ObservableCollection<datawrapper.Chapter> getStructure()
        {
            ObservableCollection<datawrapper.Chapter> _tempChapterList =
                new ObservableCollection<datawrapper.Chapter>();

            numberOfSigns = 0;
            numberOfTokens = 0;
            
////deleted geändert
            //Alle Kapitel des Bandes von Datenbank holen.
            List<models.Chapter> chapters = (from chapter in tableChapter
                                             where chapter.obj_tome.tomeID == tome.tomeID && chapter.deleted == false
                                             orderby chapter.chapterNumber
                                             select chapter).ToList();
            //Alle geholten Kapitel durchlaufen.
            foreach (models.Chapter item in chapters)
            {
                //Konvertierung eines Kapitels von models zu datawrapper 
                datawrapper.Chapter _chapter = (datawrapper.Chapter)item;

                //Alle events des Kapitels von Datenbank holen.
                List<models.Event> events = (from _event in tableEvent
                                             where _event.obj_Chapter.chapterID == item.chapterID && _event.deleted == false
                                             orderby _event.orderInChapter
                                             select _event).ToList();

                ObservableCollection<datawrapper.Event> _events = new ObservableCollection<datawrapper.Event>();
                //alle geholten Events durchlaufen.
                foreach (models.Event item2 in events)
                {
                    //Casten des Eregnis von models zu Datawrapper, und in ObservableCollection Speichern.
                    _events.Add((datawrapper.Event)item2);
                    numberOfSigns += item2.finaltext.Length;
                    string finaltext = item2.finaltext;
                    string[] tokens = finaltext.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries);
                    numberOfTokens += tokens.Count();
                }


                //"neues Ereignis" einfügen
                datawrapper.Event _e = new datawrapper.Event()
                {
                    title = AppResources.TomeDetailsEvent + " " + AppResources.TomeDetailsAddOne,
                    eventID = 0,
                    chapter = new datawrapper.Chapter() { chapterID = item.chapterID }

                };
                _events.Add(_e);

                //Die Events zu dem Kapitel hinzufügen
                _chapter.events = _events;

                //Kapitel zur List hinzufügen
                _tempChapterList.Add(_chapter);
            }

            //"neues Kapitel" einfügen
            datawrapper.Chapter _c = new datawrapper.Chapter()
            {
                title = AppResources.TomeDetailsNewOne + " " + AppResources.TomeDetailsChapter,
                chapterID = 0
            };
            _tempChapterList.Add(_c);

            return _tempChapterList;
        }

        public bool isChapterNameDuplicate(string chapterName)
        { 
            List<models.Chapter> _c = (from chapter in tableChapter
                                       where chapter.obj_tome.tomeID == tome.tomeID
                                       && chapter.title.Equals(chapterName)
                                       select chapter).ToList();

            return _c.Count != 0;
        }


        

        public void updateChapter(datawrapper.Chapter _c) 
        {
            try
            {
                //Problem: übergibt bei Focuslost (-> klick auf anderes Chapter) evtl das falsche chapter (da tap auf anderes...)
            //obj_memoryNote = db.GetTable<models.MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == memoryNoteID);
                obj_chapter = wtb.GetTable<models.Chapter>().Single(chapter => chapter.chapterID == _c.chapterID);
                obj_chapter.title = _c.title;
                wtb.SubmitChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// ///////////////
        /// </summary>
        /// <param name="_e"></param>
        public void updateEvent(datawrapper.Event _e)
        {
            try
            {
                //Problem: übergibt bei Focuslost (-> klick auf anderes Event) evtl das falsche chapter (da tap auf anderes...)
                //obj_memoryNote = db.GetTable<models.MemoryNote>().Single(memoryNote => memoryNote.memoryNoteID == memoryNoteID);
                obj_event = wtb.GetTable<models.Event>().Single(event_ => event_.eventID == _e.eventID);
                obj_event.title = _e.title;
                wtb.SubmitChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Ein Kapitel in Datenbank speichern
        /// </summary>
        /// <param name="_c"></param>
        public void saveChapter(datawrapper.Chapter _c) 
        {
            try
            {
                ObservableCollection<datawrapper.Event> _events = new ObservableCollection<datawrapper.Event>();
                

                obj_chapter = new models.Chapter()
                {
                    addedDate = _c.addedDate,
                    chapterNumber = _c.chapterNumber,
                    deleted = _c.deleted,
                    obj_tome = tome,    //Tome aus instanzvariable
                    title = _c.title,
                    updatedDate = _c.updatedDate

                };
                wtb.GetTable<models.Chapter>().InsertOnSubmit(obj_chapter);
                wtb.SubmitChanges();    //Datenbank aktualisieren.

                //"neues Ereignis" einfügen
                datawrapper.Event _e = new datawrapper.Event()
                {
                    title = AppResources.TomeDetailsEvent + " " + AppResources.TomeDetailsAddOne,
                    chapter = new datawrapper.Chapter() { chapterID = obj_chapter.chapterID },
                    eventID = 0
                };

                _events.Add(_e);

                _c.events = _events;
                

                //Für die Reihenfolge den letzten Eintrag ("Neues Kapitel") löschen, das neue Kapitel einfügen und "Neues Kapitel" wieder hinzufügen
                _structur.RemoveAt(_structur.Count - 1);
                _structur.Add(_c);

                //"neues Kapitel" einfügen
                datawrapper.Chapter _newC = new datawrapper.Chapter()
                {
                    title = AppResources.TomeDetailsNewOne + " " + AppResources.TomeDetailsChapter,
                    chapterID = 0 // TODO: ÄNDERN AUF -1
                };
                _structur.Add(_newC);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void deleteChapter(ObservableCollection<datawrapper.Chapter> ChapterList) {
            foreach (datawrapper.Chapter item in ChapterList)
            {
                List<models.Event> events = wtb.GetTable<models.Event>().Where(_event => _event.obj_Chapter.chapterID == item.chapterID).ToList();
                foreach (models.Event _e in events)
                {
                    _e.deleted = true;
                    _e.orderInChapter = -1;
                }
                obj_chapter = wtb.GetTable<models.Chapter>().Single(chapter => chapter.chapterID == item.chapterID);
                obj_chapter.deleted = true;
                int oldChapterNumber = obj_chapter.chapterNumber;
                obj_chapter.chapterNumber = -1;
                
                //Alle untere Kapitel aktualisieren.
                List<models.Chapter> chapters = wtb.GetTable<models.Chapter>().Where(chapter => chapter.chapterNumber > oldChapterNumber).ToList();
                foreach (models.Chapter _c in chapters)
                {
                    _c.chapterNumber -= 1;
                }
            
            }
            wtb.SubmitChanges();
            this._structur = this.getStructure();
        }

        public void deleteEvent(ObservableCollection<datawrapper.Event> EventList)
        {
            foreach (datawrapper.Event item in EventList)
            {
                obj_event = wtb.GetTable<models.Event>().Single(_e => _e.eventID == item.eventID);
                obj_event.deleted = true;
                int oldOrderInChapter = obj_event.orderInChapter;
                obj_event.orderInChapter = -1;
                List<models.Event> events = wtb.GetTable<models.Event>().Where(_e => _e.orderInChapter > oldOrderInChapter).ToList();
                foreach (models.Event _e in events)
                {
                    _e.orderInChapter -= 1;
                }
            }



            wtb.SubmitChanges();
            this._structur = this.getStructure();
        }


        public void removeNewChapterEntry() {
            if (this.structur.Count == 0 || this._structur.Last().chapterID == 0)
            {
                this._structur.RemoveAt(this._structur.Count - 1);
            }
        }

        public void addNewChapterEntry() {
            if (this.structur.Count == 0 || this._structur.Last().chapterID != 0)
            {
                this._structur.Add(new datawrapper.Chapter()
                {
                    title = AppResources.TomeDetailsNewOne + " " + AppResources.TomeDetailsChapter,
                    chapterID = 0 // TODO: ÄNDERN AUF -1
                });
            }
        }

        public void removeNewEventEntry() {
            IEnumerator<datawrapper.Chapter> enumerator = this._structur.GetEnumerator();
            while (enumerator.MoveNext()) {
                if (enumerator.Current.events.Count > 0 && enumerator.Current.events.Last().eventID == 0) {
                    enumerator.Current.events.RemoveAt(enumerator.Current.events.Count - 1);
                }
            }

        }

        public void addNewEventEntry() {

            foreach (datawrapper.Chapter item in _structur)
            {
                if (item.events.Count == 0 || (item.chapterID != 0 && item.events.Last().eventID != 0))
                {
                    item.events.Add(new datawrapper.Event()
                    {
                        title = AppResources.TomeDetailsEvent + " " + AppResources.TomeDetailsAddOne,
                        eventID = 0
                    });
                }
            }

        }

        public void moveChapterDown(datawrapper.Chapter chapter) {
            var sqlChapters = from ch in tableChapter
                              where ch.obj_tome.tomeID == this.tome.tomeID
                              select ch;
            Chapter exchange = null;
            int maxChapterNumber = 2 ^ 31;
            foreach (var sqlc in sqlChapters) {
                if (sqlc.chapterNumber > chapter.chapterNumber && sqlc.chapterNumber < maxChapterNumber) {
                    maxChapterNumber = sqlc.chapterNumber;
                    exchange = sqlc;
                }
            }

            if (exchange != null) {
                int tmp = exchange.chapterNumber;
                exchange.chapterNumber = chapter.chapterNumber;

                var clickedChapter = (from c in tableChapter
                                     where c.chapterID == chapter.chapterID
                                     select c).Single();
                Debug.WriteLine("Exchange '" + exchange.title + "' with '" + clickedChapter.title + "'");
                clickedChapter.chapterNumber = tmp;
                this.wtb.SubmitChanges();
                this._structur = this.getStructure();
                this.NotifyPropertyChanged("_structur");
            }
        }

        public void moveChapterUp(datawrapper.Chapter chapter)
        {
            var sqlChapters = from ch in tableChapter
                              where ch.obj_tome.tomeID == this.tome.tomeID
                              select ch;
            Chapter exchange = null;
            int maxChapterNumber = -1;
            foreach (var sqlc in sqlChapters)
            {
                if (sqlc.chapterNumber < chapter.chapterNumber && sqlc.chapterNumber > maxChapterNumber)
                {
                    maxChapterNumber = sqlc.chapterNumber;
                    exchange = sqlc;
                }
            }
            if (exchange != null)
            {
                int tmp = exchange.chapterNumber;
                exchange.chapterNumber = chapter.chapterNumber;

                var clickedChapter = (from c in tableChapter
                                      where c.chapterID == chapter.chapterID
                                      select c).Single();
                Debug.WriteLine("Exchange '" + exchange.title + "' with '" + clickedChapter.title + "'");
                clickedChapter.chapterNumber = tmp;
                this.wtb.SubmitChanges();
                this._structur = this.getStructure();
                this.NotifyPropertyChanged("_structur");
            }
        }

        public void moveEventDown(datawrapper.Event ev) {

            // zunaechst INNERHALB des kapitels eine neue position suchen
            var sqlEventsInChapter = from e in tableEvent
                                     where e.fk_chapterID == ev.chapter.chapterID
                                     select e;
            int maxEventPosition = 2 ^ 31;
            Event exchange = null;
            foreach (var e in sqlEventsInChapter) {
                if (e.orderInChapter > ev.orderInChapter && e.orderInChapter < maxEventPosition) {
                    maxEventPosition = e.orderInChapter;
                    exchange = e;
                }
            }

            if (exchange != null)
            {
                // ein nachfolgerevent wurde gefunden, plätze tauschen
                int tmp = exchange.orderInChapter;
                exchange.orderInChapter = ev.orderInChapter;

                var curEvent = (from e in tableEvent
                                where e.eventID == ev.eventID
                                select e).Single();
                curEvent.orderInChapter = tmp;
                Debug.WriteLine("Exchange '" + exchange.title + "' with '" + curEvent.title + "'");
                this.wtb.SubmitChanges();
            }
            else { 
                // kein nachfolgerevent gefunden, ggf. an erste position des folgenden kapitels verschieben
                var sqlThisChapter = (from c in tableChapter
                                     where c.chapterID == ev.chapter.chapterID
                                     select c).Single();
                var sqlAllChapterInTome = (from c in tableChapter
                                           where c.obj_tome.tomeID == this.tome.tomeID
                                           orderby c.chapterNumber
                                           select c);
                Chapter chapterToChange = null;
                int minChapterID = 2 ^ 31;
                foreach (var chapter in sqlAllChapterInTome) {
                    if (chapter.chapterNumber > sqlThisChapter.chapterNumber && chapter.chapterNumber < minChapterID) {
                        chapterToChange = chapter;
                        minChapterID = chapter.chapterNumber;
                    }
                }

                if (chapterToChange != null) { 
                    // ein nachfolgekapitel wurde gefunden, verschiebe ereignis an den beginn des nächsten kapitels
                    int firstID = (chapterToChange.events.Count() > 0) ? chapterToChange.events.First().orderInChapter : 1;
                    var moveEvents = from e in this.tableEvent
                                     where e.fk_chapterID == chapterToChange.chapterID
                                     select e;
                    foreach (var t in moveEvents) {
                        t.orderInChapter++;
                    }
                    this.wtb.SubmitChanges();
                    Event curEvent = (from e in tableEvent
                                     where e.eventID == ev.eventID
                                     select e).Single();
                    Debug.WriteLine("Changing Chapter of event '" + curEvent.title + curEvent.eventID + "' from '" + curEvent.obj_Chapter.title + curEvent.obj_Chapter.chapterID + "' to '" + chapterToChange.title + chapterToChange.chapterID + "'");

                    Chapter oldChapter = (from c in tableChapter
                                          where c.chapterID == curEvent.obj_Chapter.chapterID
                                          select c).Single();
                    oldChapter.events.Remove(curEvent);
                    curEvent.fk_chapterID = chapterToChange.chapterID;
                    curEvent.obj_Chapter = null;
                    curEvent.orderInChapter = firstID;
                    chapterToChange.events.Add(curEvent);

                    this.wtb.SubmitChanges();
                    curEvent = (from e in tableEvent
                                where e.eventID == ev.eventID
                                select e).Single();

                    curEvent.obj_Chapter = chapterToChange;
                    //curEvent.fk_chapterID = chapterToChange.chapterID;
                    //curEvent.obj_Chapter = chapterToChange;
                    //chapterToChange.events.Add(curEvent);
                    this.wtb.SubmitChanges();
                }
            }

            this._structur = this.getStructure();
        }

        public void moveEventUp(datawrapper.Event ev) {
            // zunaechst INNERHALB des kapitels eine neue position suchen
            var sqlEventsInChapter = from e in tableEvent
                                     where e.fk_chapterID == ev.chapter.chapterID
                                     select e;
            int maxEventPosition = -1;
            Event exchange = null;
            foreach (var e in sqlEventsInChapter)
            {
                if (e.orderInChapter < ev.orderInChapter && e.orderInChapter > maxEventPosition)
                {
                    maxEventPosition = e.orderInChapter;
                    exchange = e;
                }
            }

            if (exchange != null)
            {
                // ein vorheriges event wurde gefunden, plätze tauschen
                int tmp = exchange.orderInChapter;
                exchange.orderInChapter = ev.orderInChapter;

                var curEvent = (from e in tableEvent
                                where e.eventID == ev.eventID
                                select e).Single();
                curEvent.orderInChapter = tmp;
                Debug.WriteLine("Exchange '" + exchange.title + "' with '" + curEvent.title + "'");
                this.wtb.SubmitChanges();
            }
            else
            {
                // kein nachfolgerevent gefunden, ggf. an letzte position des vorherigen kapitels verschieben
                var sqlThisChapter = (from c in tableChapter
                                      where c.chapterID == ev.chapter.chapterID
                                      select c).Single();
                var sqlAllChapterInTome = (from c in tableChapter
                                           where c.obj_tome.tomeID == this.tome.tomeID
                                           orderby c.chapterNumber
                                           select c);
                Chapter chapterToChange = null;
                int maxChapterID = -1;
                foreach (var chapter in sqlAllChapterInTome)
                {
                    if (chapter.chapterNumber < sqlThisChapter.chapterNumber && chapter.chapterNumber > maxChapterID)
                    {
                        chapterToChange = chapter;
                        maxChapterID = chapter.chapterNumber;
                    }
                }

                if (chapterToChange != null)
                {
                    // ein nachfolgekapitel wurde gefunden, verschiebe ereignis an das ende des vorherigen kapitels
                    var moveEvents = from e in this.tableEvent
                                     where e.fk_chapterID == chapterToChange.chapterID
                                     orderby e.orderInChapter
                                     select e;
                    int lastID = (moveEvents.Count() > 0) ? moveEvents.ToArray().Last().orderInChapter + 1 : 1;

                    Event curEvent = (from e in tableEvent
                                      where e.eventID == ev.eventID
                                      select e).Single();
                    Debug.WriteLine("Changing Chapter of event '" + curEvent.title + curEvent.eventID + "' from '" + curEvent.obj_Chapter.title + curEvent.obj_Chapter.chapterID + "' to '" + chapterToChange.title + chapterToChange.chapterID + "'");

                    Chapter oldChapter = (from c in tableChapter
                                          where c.chapterID == curEvent.obj_Chapter.chapterID
                                          select c).Single();
                    oldChapter.events.Remove(curEvent);
                    curEvent.fk_chapterID = chapterToChange.chapterID;
                    curEvent.obj_Chapter = null;
                    curEvent.orderInChapter = lastID;
                    chapterToChange.events.Add(curEvent);

                    this.wtb.SubmitChanges();
                    curEvent = (from e in tableEvent
                                where e.eventID == ev.eventID
                                select e).Single();

                    curEvent.obj_Chapter = chapterToChange;

                    this.wtb.SubmitChanges();
                }
            }

            this._structur = this.getStructure();
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

        public void changeTitle(String newTitle) {
            this.tome.title = newTitle;
            this.wtb.SubmitChanges();
            this.NotifyPropertyChanged("tome");
        }

        //public bool isEventsInChapter(LongListMultiSelector l)
        //{
        //    foreach (datawrapper.Event item in l.ItemsSource)
        //    {
        //        int y = (from x in tableEvent
        //                 where x.eventID == item.eventID
        //                 select x).Count();
        //        if (y == 0)
        //            return false;
        //    }

        //    return true;
        //}

        public bool isEventsInChapter(LongListMultiSelector l, int chapterID)
        {
            foreach (datawrapper.Event item in l.ItemsSource)
            {
                int y = (from x in tableEvent
                         where x.eventID == item.eventID && x.obj_Chapter.chapterID == chapterID
                         select x).Count();
                if (y == 0)
                    return false;
            }

            return true;
        }


        public bool tomeTitleAlreadyExists(String tomeTitel) 
        {
            return (from t in this.tableTome
                         where t.title.Equals(tomeTitel)
                         select t).Count() > 0;
            
        }

        public bool isEventNameDuplicate(string eventName)
        {
            List<models.Event> _e = (from ev in tableEvent
                                     where ev.obj_Chapter.obj_tome.tomeID == tome.tomeID
                                     && ev.title.Equals(eventName)
                                     select ev).ToList();

            return _e.Count != 0;
        }

    }
}
