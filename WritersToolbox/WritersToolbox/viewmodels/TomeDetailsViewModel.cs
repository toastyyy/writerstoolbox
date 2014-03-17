using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;

namespace WritersToolbox.viewmodels
{
    class TomeDetailsViewModel
    {
        //Datenbank
        private WritersToolboxDatebase wtb;
        //Chapter als Entity object
        private models.Chapter obj_chapter;

        //TypeObjecte, um die beteiligte Objekte zu zeigen.-Tabelle
        private Table<TypeObject> tableTypeObject;
        //Event-Tabelle
        private Table<Event> tableEvent;
        //Chapter-Tabelle
        private Table<Chapter> tableChapter;
        //Tome-Tabelle
        private Table<Tome> tableTome;
        //Band
        private Tome tome;
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

        /// <summary>
        /// liefert Anzahl Kapitel durch abfrage in der Datenbank zurück.
        /// </summary>
        /// <returns>liefert Anzahl Kapitel zurück</returns>
        public int getNumberOfChapters()
        {
            return tableChapter.Count(c => c.obj_tome.tomeID == tome.tomeID);
        }

        /// <summary>
        /// liefert Anzahl Ereignisse durch abfrage in der Datenbank zurück.
        /// </summary>
        /// <returns>liefert Anzahl Ereignisse zurück</returns>
        public int getNumberOfEvents()
        {
            return tableEvent.Count(e => e.obj_Chapter.obj_tome.tomeID == tome.tomeID);
        }

        /// <summary>
        /// liefert Anzahl Typeobjekte durch abfrage in der Datenbank zurück.
        /// </summary>
        /// <returns>liefert Anzahl Typeobjekte zurück</returns>
        public int getNumberOfTypeObjects()
        {
            return 0; //tableTypeObject.Count(to => to.obj_Event.obj_Chapter.obj_tome.tomeID == tome.tomeID);
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

            //Alle Kapitel des Bandes von Datenbank holen.
            List<models.Chapter> chapters = (from chapter in tableChapter
                                             where chapter.obj_tome.tomeID == tome.tomeID
                                             orderby chapter.chapterNumber
                                             select chapter).ToList();
            //Alle geholten Kapitel durchlaufen.
            foreach (models.Chapter item in chapters)
            {
                //Konvertierung eines Kapitels von models zu datawrapper 
                datawrapper.Chapter _chapter = (datawrapper.Chapter)item;

                //Alle events des Kapitels von Datenbank holen.
                List<models.Event> events = (from _event in tableEvent
                                             where _event.obj_Chapter.chapterID == item.chapterID
                                             orderby _event.orderInChapter
                                             select _event).ToList();

                ObservableCollection<datawrapper.Event> _events = new ObservableCollection<datawrapper.Event>();
                //alle geholten Events durchlaufen.
                foreach (models.Event item2 in events)
                {
                    //Casten des Eregnis von models zu Datawrapper, und in ObservableCollection Speichern.
                    _events.Add((datawrapper.Event)item2);
                }


                //"neues Ereignis" einfügen
                datawrapper.Event _e = new datawrapper.Event()
                {
                    title = "Ereignis hinzufügen",
                    eventID = 0

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
                title = "Neues Kapitel",
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
        /// Ein Kapitel in Datenbank speichern
        /// </summary>
        /// <param name="_c"></param>
        public void saveChapter(datawrapper.Chapter _c) 
        {
            try
            {
               


                ObservableCollection<datawrapper.Event> _events = new ObservableCollection<datawrapper.Event>();
                //"neues Ereignis" einfügen
                datawrapper.Event _e = new datawrapper.Event()
                {
                    title = "Ereignis hinzufügen",
                    eventID = 0

                };
                _events.Add(_e);

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
                _c.events = _events;
                

                //Für die Reihenfolge den letzten Eintrag ("Neues Kapitel") löschen, das neue Kapitel einfügen und "Neues Kapitel" wieder hinzufügen
                _structur.RemoveAt(_structur.Count - 1);
                _structur.Add(_c);

                //"neues Kapitel" einfügen
                datawrapper.Chapter _newC = new datawrapper.Chapter()
                {
                    title = "Neues Kapitel",
                    chapterID = 0 // TODO: ÄNDERN AUF -1
                };
                _structur.Add(_newC);


               
                



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void deleteChapter() { 
        
        }

        public void removeNewChapterEntry() {
            if (this._structur.Last().chapterID == 0) {
                this._structur.RemoveAt(this._structur.Count - 1);
            }
        }

        public void addNewChapterEntry() {
            if (this._structur.Last().chapterID != 0)
            {
                this._structur.Add(new datawrapper.Chapter()
                {
                    title = "Neues Kapitel",
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
            IEnumerator<datawrapper.Chapter> enumerator = this._structur.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.chapterID != 0 && enumerator.Current.events.Last().eventID != 0)
                {
                    enumerator.Current.events.Add(new datawrapper.Event()
                    {
                        title = "Ereignis hinzufügen",
                        eventID = 0

                    });
                }
            }
        }
    }
}
