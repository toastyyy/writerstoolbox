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
    /// <summary>
    /// Die TypeDetailViewModel Klasse bzw. Präsentations-Logik ist eine aggregierte Datenquelle,
    /// die verschiedene Daten von TypeObject und ihre entsprechende Eigenschaften bereitstellt.
    /// </summary>
    class TypeDetailViewModel : INotifyPropertyChanged
    {
        private WritersToolboxDatebase wtb;
        private Table<TypeObject> tableTypeObject;
        private Table<MemoryNote> tableMemoryNote;
        private Table<models.Type> tableType;
        private datawrapper.TypeObject typeObject;
        private int typeObjectID = -1;
        /// <summary>
        /// Enthält nach dem Aufruf von LoadData das gewählte TypeObject.
        /// </summary>
        public datawrapper.TypeObject TypeObject {
            get { return typeObject; }
            set { typeObject = value; }
        }

        /// <summary>
        /// Erzeugt ein neues Viewmodel für das TypObjekt mit der angegebenen ID.
        /// </summary>
        /// <param name="id">ID des TypeObjects</param>
        public TypeDetailViewModel(int id) 
        {
            wtb = WritersToolboxDatebase.getInstance();
            tableTypeObject = wtb.GetTable<TypeObject>();
            tableMemoryNote = wtb.GetTable<MemoryNote>();
            tableType = wtb.GetTable<models.Type>();
            this.typeObjectID = id;
            this.LoadData();
        }

        /// <summary>
        /// Läd die Daten des Typobjekts.
        /// </summary>
        public void LoadData() {
            var v = from to in tableTypeObject
                    where to.typeObjectID == this.typeObjectID
                    select to;

            var o = v.Single();

            var notes = (from n in tableMemoryNote
                        where n.obj_TypeObject.typeObjectID == this.typeObjectID && n.deleted == false
                        select n).OrderByDescending(x => x.addedDate);
            ObservableCollection<datawrapper.MemoryNote> listNotes = new ObservableCollection<datawrapper.MemoryNote>();

            foreach (var n in notes)
            {
                datawrapper.MemoryNote note = new datawrapper.MemoryNote()
                {
                    addedDate = n.addedDate,
                    associated = n.associated,
                    contentAudioString = n.contentAudioString,
                    contentImageString = n.ContentImageString,
                    contentText = n.contentText,
                    deleted = n.deleted,
                    fk_eventID = (n.obj_Event == null) ? -1 : n.obj_Event.eventID,
                    fk_typeObjectID = (n.obj_TypeObject == null) ? -1 : n.obj_TypeObject.typeObjectID,
                    location = n.location,
                    memoryNoteID = n.memoryNoteID,
                    tags = n.tags,
                    title = n.title,
                    updatedDate = n.updatedDate,
                };
                listNotes.Add(note);

            }

            // zugehoerigen typ ermitteln

            models.Type typ = (from t in tableType
                               where t.typeID == o.fk_typeID
                               select t).Single();

            datawrapper.Type wrappedType = new datawrapper.Type()
            {
                color = typ.color,
                imageString = typ.imageString,
                title = typ.title,
                typeID = typ.typeID
            };
            typeObject = new datawrapper.TypeObject()
            {
                color = o.color,
                type = wrappedType,
                imageString = o.imageString,
                typeObjectID = o.typeObjectID,
                used = o.used,
                name = o.name,
                notes = listNotes
            };
            this.NotifyPropertyChanged("TypeObject");
        }

        /// <summary>
        /// Löscht das Typobjekt mit der angegebenen ID.
        /// </summary>
        /// <param name="typeObjectID">TypeObject ID</param>
        /// <param name="keepNotes">Gibt an ob Notizen beibehalten werden sollen</param>
        public void deleteTypeObject(int typeObjectID, bool keepNotes)
        {
            var typeObject = (from to in tableTypeObject
                              where to.typeObjectID == typeObjectID
                              select to).Single();
            if (keepNotes)
            {
                var notes = from n in this.tableMemoryNote
                            where n.obj_TypeObject.Equals(typeObject)
                            select n;
                foreach (var note in notes)
                {
                    note.associated = false;
                    note.obj_TypeObject = null;
                }
            }
            typeObject.deleted = true;
            this.wtb.SubmitChanges();
        }

        /// <summary>
        /// Entfernt die Zuordnung der angegebenen ID vom Typobjekt oder verschiebt die Notiz in den Papierkorb.
        /// </summary>
        /// <param name="noteID">Notiz ID</param>
        /// <param name="unsortedNote">Wenn true, wird die Zuordnung aufgehoben, aber die Notiz nicht gelöscht</param>
        public void deleteNote(int noteID, bool unsortedNote)
        {
            var note = (from n in tableMemoryNote
                        where n.memoryNoteID == noteID
                        select n).Single();
            if (unsortedNote)
            {
                note.associated = false;
                note.obj_TypeObject = null;
            }
            else
            {
                note.deleted = true;
            }
            
            this.wtb.SubmitChanges();
            this.NotifyPropertyChanged("Note");
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
    }
}
