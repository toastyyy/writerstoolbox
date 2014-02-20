using System;
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
            return tableTypeObject.Count(to => to.obj_Event.obj_Chapter.obj_tome.tomeID == tome.tomeID);
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
                              where typeObject.obj_Event.obj_Chapter.obj_tome.tomeID == tomeID
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

                //datawrapper.Event _e = new datawrapper.Event()
                //{
                //    title = "Ereignis hinzufügen",
                //};
                //_events.Add(_e);

                //Die Events zu dem Kapitel hinzufügen
                _chapter.events = _events;

                //Kapitel zur List hinzufügen
                _tempChapterList.Add(_chapter);
            }

            return _tempChapterList;
        }
    }
}
