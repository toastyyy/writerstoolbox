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
using Microsoft.Phone.Shell;
using WritersToolbox.Resources;
using System.IO.IsolatedStorage;
namespace WritersToolbox.viewmodels
{
    /// <summary>
    /// Viewmodel für Types.xaml.
    /// Stellt die Typen mit ihren jeweiligen Typobjekten im View bereit.
    /// </summary>
    public class TypesViewModel : INotifyPropertyChanged
    {
        private WritersToolboxDatebase db; // Datenbankinstanz
        private Table<models.Type> tableType; // Tabelle für die Typen
        private Table<TypeObject> tableTypeObject; // Tabelle für die Typobjekte
        private ObservableCollection<datawrapper.Type> types; // Liste der Typen für den View

        /// <summary>
        /// Property für alle vorhandenen Typen.
        /// Informiert mit NotifyPropertyChanged.
        /// </summary>
        public ObservableCollection<datawrapper.Type> Types
        {
            get { return types; }
            set
            {
                types = value;
                NotifyPropertyChanged("Types");
            }
        }

        /// <summary>
        /// Neue Instanz des Viewmodels erzeugen.
        /// </summary>
        public TypesViewModel()
        {
            // Datenbank und Tabellen laden
            this.db = WritersToolboxDatebase.getInstance();
            this.tableType = this.db.GetTable<models.Type>();
            this.tableTypeObject = this.db.GetTable<TypeObject>();
        }

        /// <summary>
        /// Entfernt den Eintrag zum hinzufügen eines Typobjektes zum angegebenen
        /// Typen.
        /// </summary>
        /// <param name="t">Typ, bei dem der Eintrag entfernt werden soll</param>
        public void removeAddTypeObject(datawrapper.Type t) {
            int i = Types.IndexOf(t);
                for (int j = 0; j < Types.ElementAt(i).typeObjects.Count; j++) {
                    if (Types.ElementAt(i).typeObjects.ElementAt(j).type.typeID == -2) {
                        Types.ElementAt(i).typeObjects.RemoveAt(j);
                    }
                }
            
            this.NotifyPropertyChanged("Types");
        }

        /// <summary>
        /// Fügt den Eintrag zum Hinzufügen eines Typobjektes zum angegebenen Typ hinzu.
        /// </summary>
        /// <param name="t">Typ, bei dem der Eintrag hinzugefügt werden soll.</param>
        public void addAddTypeObject(datawrapper.Type t)
        {
            int i = Types.IndexOf(t);
                Boolean hasAdd = false;
                for (int j = 0; j < Types.ElementAt(i).typeObjects.Count && !hasAdd; j++) {
                    hasAdd = Types.ElementAt(i).typeObjects.ElementAt(j).type.typeID == -2;
                }
                if (!hasAdd) { 
                                Types.ElementAt(i).typeObjects.Add(
                    new datawrapper.TypeObject() {
                        name = AppResources.NewTypeObject,
                        imageString = "../icons/add.png",
                        type = new datawrapper.Type() { typeID = -2 },
                        color = "#FFADD8E6"
                    }
                    );
                }
            this.NotifyPropertyChanged("Types");
        }

        /// <summary>
        /// Gibt ein Array mit den Datenbank-IDs aller Typen zurück.
        /// </summary>
        /// <returns>Array mit den Datenbank-IDs aller Typen</returns>
        public int[] getAllTypeIDs()
        {
            var result = from t in tableType select t.typeID;
            return result.ToArray();
        }

        /// <summary>
        /// Gibt alle TypObjekte zu einem vorgegebenen Typ zurück.
        /// </summary>
        /// <param name="typeID">ID des vorgegebenen Typs</param>
        /// <returns>Array mit den Datenbank-IDs aller zugehörigen TypObjekte</returns>
        public int[] getAllTypeObjectIDsForTypeID(int typeID)
        {
            var result = from t in tableTypeObject
                         where t.fk_typeID == typeID
                         select t.typeObjectID;

            return result.ToArray();
        }

        /// <summary>
        /// Prüft, ob eine Notiz mit dem angegebenen Titel bereits dem angegebenen Typobjekt hinzugefügt wurde.
        /// </summary>
        /// <param name="typeObjectID">ID des Typobjektes</param>
        /// <param name="title">Titel der Notiz</param>
        /// <returns></returns>
        public bool isExistNoteInTypeobject(int typeObjectID, string title)
        {
            return db.GetTable<models.MemoryNote>().Count(_m => _m.obj_TypeObject.typeObjectID == typeObjectID && _m.title == title) == 1;
        }

        /// <summary>
        /// Entfernt die Notiz mit dem angegebenen Titel von dem Typobjekt
        /// </summary>
        /// <param name="typeObjectID">ID des Typobjektes</param>
        /// <param name="title">Titel der Notiz</param>
        public void removeNote(int typeObjectID, string title)
        {
            models.MemoryNote tempNote = db.GetTable<models.MemoryNote>().Where(_n => _n.obj_TypeObject.typeObjectID == typeObjectID && _n.title == title).First();

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
            db.GetTable<models.MemoryNote>().DeleteOnSubmit(tempNote);
            db.SubmitChanges();
        }

        /// <summary>
        /// Erstellt einen neuen Typ aus den vorgegebenen Werten.
        /// Ist ein Wert ungültig wird eine ArgumentException geworfen.
        /// </summary>
        /// <param name="title">Titel des Typs</param>
        /// <param name="color">Farbe des Typs (z.B. "00ff00") [aktuell nicht verwendet]</param>
        /// <param name="image">Pfad zum Bildspeicherort</param>
        public void createType(String title, String color, String image)
        {
           
            models.Type t = new models.Type();
            if (title.Trim().Equals(""))
                throw new ArgumentException("Titel muss ausgefüllt sein", "Title");
            var sqlT = from types in tableType
                       where types.title.Equals(title)
                       select types;
            
            if (sqlT.Count() > 0)  {
                throw new ArgumentException("Ein Typ mit dem angegebenen Namen ist bereits vorhanden.", "Title");
            }
            t.title = title;
            t.color = color;
            t.imageString = image;
            this.tableType.InsertOnSubmit(t);
            this.db.SubmitChanges();
            this.LoadData();
            
        }


        /// <summary>
        /// Aktualisiert den Typ mit der angegebenen ID mit den angegebenen Werten.
        /// </summary>
        /// <param name="typeID">ID des Typs</param>
        /// <param name="title">Neuer Titel für den Typ</param>
        /// <param name="color">Neue Farbe des Typs</param>
        /// <param name="imageString">Neuer Bildpfad für den Typ</param>
        public void updateType(int typeID, String title, String color, String imageString) {            
            try
            {
                models.Type type = (from t in tableType
                                    where t.typeID == typeID
                                    select t).First();
                type.title = title;
                type.imageString = imageString;
                type.color = color;
                this.db.SubmitChanges();
                this.LoadData();
            }
            catch (Exception e) { 
            }
        }

        /// <summary>
        /// Erstellt ein neues Typ-Objekt aus den angegebenen Werten.
        /// Wenn ein Wert ungültig ist, wird eine ArgumentException geworfen.
        /// </summary>
        /// <param name="name">Name des Typ-Objektes</param>
        /// <param name="color">Farbe des Typ-Objektes (z.B. "00ff00")</param>
        /// <param name="image">Pfad zum Bild</param>
        /// <param name="typeID">ID des zugehörigen Typs</param>
        public void createTypeObject(String name, String color, String image, int typeID)
        {
            models.Type type = (from t in tableType
                                where t.typeID == typeID
                                select t).FirstOrDefault();

            if (name.Equals(""))
            {
                throw new ArgumentException("Name muss angegeben werden", "name");
            }
            if (type == null)
            {
                throw new ArgumentException("TypeObject muss einem Typ angehören", "type");
            }
            if (image == null)
            {
                image = "../icons/TypeObjects/character.png";
            }
            TypeObject to = new TypeObject();
            to.name = name;
            if (color.Equals(""))
            {
                to.color = type.color;
            }
            else
            {
                to.color = color;
            }
            to.obj_Type = type;
            to.imageString = image;
            this.tableTypeObject.InsertOnSubmit(to);
            this.db.SubmitChanges();
            this.LoadData();
        }

        /// <summary>
        /// Löscht den angegebenen Typ.
        /// </summary>
        /// <param name="typeID">ID des Typs</param>
        public void deleteType(int typeID)
        {
            var type = (from t in tableType
                    where t.typeID == typeID
                    select t).Single();
            type.deleted = true;
            this.db.SubmitChanges();
            this.LoadData();
        }

        /// <summary>
        /// Löscht das angegebene Typobjekt zusammen mit allen angehangenen Notizen.
        /// Löschvorgang nicht entgültig. Es wird nur ein Deleted-Flag gesetzt.
        /// </summary>
        /// <param name="typeObjectID">ID des Typobjekts</param>
        public void deleteTypeObject(int typeObjectID)
        {
            var typeObject = (from to in tableTypeObject
                              where to.typeObjectID == typeObjectID
                              select to).Single();
            typeObject.deleted = true;
            this.db.SubmitChanges();
            this.LoadData();
        }

        /// <summary>
        /// Löscht das angegebene Typobjekt, aber entfernt vorher die Beziehung mit Notizen.
        /// Löschvorgang nicht entgültig. Es wird nur ein Deleted-Flag gesetzt.
        /// </summary>
        /// <param name="typeObjectID">ID des Typobjektes</param>
        public void deleteTypeObjectSoft(int typeObjectID)
        {
            var typeObject = (from to in tableTypeObject
                              where to.typeObjectID == typeObjectID
                              select to).Single();


            var notes = from n in this.db.GetTable<MemoryNote>()
                        where n.obj_TypeObject.Equals(typeObject)
                        select n;

            foreach (var note in notes)
            {
                note.associated = false;
                note.obj_TypeObject = null;
                note.obj_Event = null;
            }

            typeObject.deleted = true;
            this.db.SubmitChanges();
            this.LoadData();
        }

        /// <summary>
        /// Gibt die Anzahl der geladenen Typen zurück. Erwartet, dass LoadData vorher ausgeführt wurde.
        /// </summary>
        /// <returns>Anzahl der Typen</returns>
        public int getTypeCount()
        {
            return Types.Count;
        }


        private Color fromHexToColor(String hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);
            Byte colorR = Convert.ToByte(hex.Substring(0, 2), 16);
            Byte colorG = Convert.ToByte(hex.Substring(2, 2), 16);
            Byte colorB = Convert.ToByte(hex.Substring(4, 2), 16);

            return Color.FromArgb(255, colorR, colorG, colorB);
        }

        /// <summary>
        /// Prüft, ob die Methode LoadData bereits ausgeführt wurde.
        /// </summary>
        public bool IsDataLoaded { get; set; }

        /// <summary>
        /// Aktualisiert das angegebene Typobjekt mit den angegebenen Werten.
        /// </summary>
        /// <param name="typeObjectID">ID des TypObjektes</param>
        /// <param name="name">Neuer Name für das Typobjekt</param>
        /// <param name="color">Neue Farbe für das Typobjekt</param>
        /// <param name="imageString">Neue Bildpfad für das Typobjekt</param>
        public void updateTypeObject(int typeObjectID, String name, String color, String imageString) {
            var t = (from to in tableTypeObject
                    where to.typeObjectID == typeObjectID
                    select to).Single();
            t.name = name;
            t.color = color;
            if (imageString == null)
            {
                //default Image
                imageString = "icons/TypeObjects/character.png";
            }
            t.imageString = imageString;
           
            this.db.SubmitChanges();
        }

        /// <summary>
        /// Läd alle benötigten Daten aus der Datenbank. Muss vor dem Databinding ausgeführt werden!
        /// </summary>
        public void LoadData()
        {
            this.db.Refresh(RefreshMode.KeepChanges);
            tableType = this.db.GetTable<models.Type>();
            tableTypeObject = this.db.GetTable<TypeObject>();

            // aenderungen muessen separat gespeichert werden, weil sonst bei jedem
            // add das propertyChanged ereignis gefeuert wird.
            ObservableCollection<datawrapper.Type> tmpTypes = new ObservableCollection<datawrapper.Type>();
            var sqlTypes = from t in tableType
                           where t.deleted == false
                           select t;
            foreach (var t in sqlTypes)
            {
                datawrapper.Type wrappedType = new datawrapper.Type()
                {
                    typeID = t.typeID,
                    color = t.color,
                    imageString = t.imageString,
                    title = t.title,
                };
                var result = from to in tableTypeObject
                             where to.fk_typeID == t.typeID && to.deleted == false
                             select to;
                ObservableCollection<datawrapper.TypeObject> TypeObjects = new ObservableCollection<datawrapper.TypeObject>();
                foreach (TypeObject to in result)
                {
                    ObservableCollection<datawrapper.MemoryNote> noteDummy = new ObservableCollection<datawrapper.MemoryNote>();

                    var noteCount = (from n in this.db.GetTable<MemoryNote>()
                                where n.obj_TypeObject.typeObjectID == to.typeObjectID
                                select n).Count();
                    for (int i = 0; i < noteCount; i++) { 
                        noteDummy.Add(new datawrapper.MemoryNote());
                    }
                    datawrapper.TypeObject wrappedTO = new datawrapper.TypeObject()
                    {
                        color = to.color,
                        type = wrappedType,
                        imageString = to.imageString,
                        name = to.name,
                        typeObjectID = to.typeObjectID,
                        used = to.used,
                        notes = noteDummy
                    };
                    TypeObjects.Add(wrappedTO);
                }

                if (!(PhoneApplicationService.Current.State.ContainsKey("assignNote") || PhoneApplicationService.Current.State.ContainsKey("attachEvent")))
                {
                    // Neues Typobjekt
                    TypeObjects.Add(new datawrapper.TypeObject()
                    {
                        name = AppResources.NewTypeObject,
                        imageString = "../icons/add.png",
                        type = new datawrapper.Type() { typeID = -2 },
                        color = "#FFADD8E6"
                    }
                    );
                }
                wrappedType.typeObjects = TypeObjects;
                tmpTypes.Add(wrappedType);
            }

            // hinzufuegen fuer neuer Typ
            if (!(PhoneApplicationService.Current.State.ContainsKey("assignNote") || PhoneApplicationService.Current.State.ContainsKey("attachEvent")))
            {

                ObservableCollection<datawrapper.TypeObject> t_o = new ObservableCollection<datawrapper.TypeObject>();
                t_o.Add(new datawrapper.TypeObject()
                {
                    name = AppResources.NewType,
                    imageString = "../icons/add.png",
                    type = new datawrapper.Type() { typeID = -1 },
                    color = "#FFADD8E6"
                });
                tmpTypes.Add(new datawrapper.Type() { title = AppResources.NewType, imageString = "../icons/add.png", color = "#FFADD8E6", typeObjects = t_o, typeID = -1 });
            }
                this.NotifyPropertyChanged("Types");
                Types = tmpTypes;
                IsDataLoaded = true;
            
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
