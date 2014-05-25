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
    /// <summary>
    /// Die TypeViewModel Klasse bzw. Präsentations-Logik ist eine aggregierte Datenquelle,
    /// die verschiedene Daten von Type und ihre entsprechende Eigenschaften bereitstellt.
    /// </summary>
    class TypeViewModel : INotifyPropertyChanged
    {
        private WritersToolboxDatebase db;
        private Table<models.Type> tableType;
        private int _typeID;

        /// <summary>
        /// Enthält nach der Ausführung von LoadData alle Daten des gewählten Typs.
        /// </summary>
        public datawrapper.Type Type { get; set; }

        /// <summary>
        /// Erstellt ein neues ViewModel für den Typ mit der angegebenen ID.
        /// </summary>
        /// <param name="typeID">Type ID</param>
        public TypeViewModel(int typeID) {
            this.db = WritersToolboxDatebase.getInstance();
            this.tableType = this.db.GetTable<models.Type>();

            this._typeID = typeID;
        }

        /// <summary>
        /// Läd die Daten für den aktuell gewählten Typ.
        /// </summary>
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
