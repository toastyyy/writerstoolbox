using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.models
{
    /// <summary>
    /// Repräsentiert die N:M Beziehung zwischen Ereignissen und Typobjekten auf Datenbankebene.
    /// </summary>
    [Table(Name = "Event_TypeObjects")]
    public class EventTypeObjects : INotifyPropertyChanged
    {
        [Column(Name = "fk_eventID",
            IsPrimaryKey=true)]
        public int fk_eventID;

        private EntityRef<Event> _event;

        [Association(Name = "FK_TypeObjects_Event",
            Storage = "_event",         //Speicherort der Child-Instanzen.
            IsForeignKey = true,
            ThisKey = "fk_eventID",      //Name des Primärschlüssels.
            OtherKey = "eventID")] //Name des Fremdschlüssels.
        public Event obj_Event
        {
            get
            {
                return this._event.Entity;
            }
            set
            {
                this._event.Entity = value;
                sendPropertyChanged("event");
            }
        }

        [Column(Name = "fk_typeObjectID",
            IsPrimaryKey=true)]
        public int fk_typeObjectID;

        private EntityRef<TypeObject> _typeObject;

        [Association(Name = "FK_TypeObjects_TypeObject",
            Storage = "_typeObject",         //Speicherort der Child-Instanzen.
            IsForeignKey = true,
            ThisKey = "fk_typeObjectID",      //Name des Primärschlüssels.
            OtherKey = "typeObjectID")] //Name des Fremdschlüssels.
        public TypeObject obj_typeObject
        {
            get
            {
                return this._typeObject.Entity;
            }
            set
            {
                this._typeObject.Entity = value;
                sendPropertyChanged("typeObject");
            }
        }
        //Benachrichtigt Clients, dass ein Eigenschaftswert geändert wurde.
        public event PropertyChangedEventHandler PropertyChanged;
        protected void sendPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
