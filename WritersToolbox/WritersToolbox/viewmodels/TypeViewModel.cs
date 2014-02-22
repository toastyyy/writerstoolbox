using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;

namespace WritersToolbox.viewmodels
{
    class TypeViewModel : INotifyPropertyChanged
    {
        private WritersToolboxDatebase db;
        private Table<models.Type> tableType;
        private int _typeID;

        public datawrapper.Type Type { get; set; }

        public TypeViewModel(int typeID) {
            this.db = WritersToolboxDatebase.getInstance();
            this.tableType = this.db.GetTable<models.Type>();

            this._typeID = typeID;
        }

        public void loadData() {
            models.Type sqlType = (from t in tableType
                          where t.typeID == this._typeID
                          select t).Single();
            this.Type = new datawrapper.Type() { 
                color = sqlType.color,
                imageString = sqlType.imageString,
                title = sqlType.title,
                typeID = sqlType.typeID,
                typeObjects = new ObservableCollection<datawrapper.TypeObject>()
            };
            this.NotifyPropertyChanged("Type");
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

        public void updateType(int typeID, String title, String color, String imageString)
        {
            try
            {
                models.Type type = (from t in tableType
                                    where t.typeID == typeID
                                    select t).First();
                type.title = title;
                type.imageString = imageString;
                type.color = color;
                this.db.SubmitChanges();
                this.loadData();
            }
            catch (Exception e)
            {
            }
        }
    }
}
