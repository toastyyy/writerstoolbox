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
    class TypeDetailViewModel : INotifyPropertyChanged
    {
        private WritersToolboxDatebase wtb;
        private Table<TypeObject> tableTypeObject;
        private Table<MemoryNote> tableMemoryNote;

        private datawrapper.TypeObject typeObject;

        public datawrapper.TypeObject TypeObject {
            get { return typeObject; }
            set { typeObject = value; }
        }

        public TypeDetailViewModel(int id) 
        {
            wtb = WritersToolboxDatebase.getInstance();
            tableTypeObject = wtb.GetTable<TypeObject>();
            tableMemoryNote = wtb.GetTable<MemoryNote>();

            var v = from to in tableTypeObject
                    where to.typeObjectID == id
                    select to;

            var o = v.Single();

            var notes = from n in tableMemoryNote
                        where n.obj_TypeObject.typeObjectID == id
                        select n;
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
 
            typeObject = new datawrapper.TypeObject() 
            { 
                color = o.color,
                fk_typeID = o.fk_typeID,
                imageString = o.imageString,
                typeObjectID = o.typeObjectID,
                used = o.used,
                name = o.name,
                notes = listNotes
            };
            this.NotifyPropertyChanged("TypeObject");
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
