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
    public class TypesViewModel : INotifyPropertyChanged
    {
        private WritersToolboxDatebase db;
        private Table<models.Type> tableType;
        private Table<TypeObject> tableTypeObject;
        public TypesViewModel()
        {
            this.db = WritersToolboxDatebase.getInstance();
            this.tableType = this.db.GetTable<models.Type>();
            this.tableTypeObject = this.db.GetTable<TypeObject>();
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
        /// Gibt die Farbe eines Typs zurück.
        /// </summary>
        /// <param name="typeID">ID des vorgegebenen Typs</param>
        /// <returns>Farbe (System.Windows.Media.Color)</returns>
        public Color getColorForType(int typeID)
        {
            var result = from t in tableType
                         where t.typeID == typeID
                         select t.color;

            return fromHexToColor(result.FirstOrDefault());
        }

        /// <summary>
        /// Gibt den Titel eines Typs zurück.
        /// </summary>
        /// <param name="typeID">ID des vorgegebenen Typs</param>
        /// <returns>Titel als String (z.B. "Charakter")</returns>
        public string getTitleForType(int typeID)
        {
            var result = from t in tableType
                         where t.typeID == typeID
                         select t.title;



            return result.First();
        }

        /// <summary>
        /// Gibt das Bild eines Typs zurück.
        /// </summary>
        /// <param name="typeID">ID des vorgegebenen Typs</param>
        /// <returns>Bild (System.Windows.Controls.Image) oder null wenn kein Bild vorhanden</returns>
        public Image getImageForType(int typeID)
        {
            String imagePath = (from t in tableType
                                where t.typeID == typeID
                                select t.imageString).FirstOrDefault();

            if (imagePath.Equals(""))
            {
                return null;
            }
            else
            {
                BitmapImage bi = new BitmapImage(new Uri(imagePath));
                Image img = new Image();
                img.Source = bi;
                return img;
            }

        }

        /// <summary>
        /// Gibt den Namen eines Typ-Objektes zurück.
        /// </summary>
        /// <param name="typeObjectID">ID des vorgegebenen Typ-Objektes</param>
        /// <returns>Name als String (z.B. "Harry Potter")</returns>
        public String getNameForTypeObject(int typeObjectID)
        {
            return (from to in tableTypeObject
                    where to.typeObjectID == typeObjectID
                    select to.name).FirstOrDefault();
        }

        public Color getColorForTypeObject(int typeObjectID)
        {
            var result = from to in tableTypeObject
                         where to.typeObjectID== typeObjectID
                         select to.color;

            return fromHexToColor(result.FirstOrDefault());
        }

        /// <summary>
        /// Gibt die Datenbank-IDs aller zum Typ-Objekt gehörenden Notizen zurück.
        /// </summary>
        /// <param name="typeObjectID">ID des vorgegebenen Typ-Objektes</param>
        /// <returns>int-Array mit den IDs der Notizen</returns>
        public int[] getNoteIDsForTypeObject(int typeObjectID)
        {
            var result = (from to in tableTypeObject
                          where to.typeObjectID == typeObjectID
                          select to.notes).FirstOrDefault();

            int[] retVar = new int[result.Count()];
            int index = 0;
            foreach (var row in result)
            {
                retVar[index] = ((MemoryNote)row).memoryNoteID;
                index++;
            }

            return retVar;
        }

        /// <summary>
        /// Gibt das dem Typ-Objekt zugeordnete Bild zurück.
        /// </summary>
        /// <param name="typeObjectID">ID des vorgegebenen Typ-Objektes</param>
        /// <returns>ld (System.Windows.Controls.Image) oder null wenn kein Bild vorhanden</returns>
        public Image getImageForTypeObject(int typeObjectID)
        {
            String imagePath = (from to in tableTypeObject
                                where to.typeObjectID == typeObjectID
                                select to.imageString).FirstOrDefault();

            if (imagePath.Equals(""))
            {
                return null;
            }
            else
            {
                BitmapImage bi = new BitmapImage(new Uri(imagePath));
                Image img = new Image();
                img.Source = bi;
                return img;
            }
        }

        /// <summary>
        /// Erstellt einen neuen Typ aus den vorgegebenen Werten.
        /// Ist ein Wert ungültig wird eine ArgumentException geworfen.
        /// </summary>
        /// <param name="title">Titel des Typs</param>
        /// <param name="color">Farbe des Typs (z.B. "00ff00")</param>
        /// <param name="image">Pfad zum Bildspeicherort</param>
        public void createType(String title, String color, String image)
        {
            models.Type t = new models.Type();
            if (title.Equals(""))
                throw new ArgumentException("Titel muss ausgefüllt sein", "Title");
            var sqlT = from types in tableType
                       where types.title.Equals(title)
                       select types;
            if (sqlT.Count() > 0) {
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

            this.tableTypeObject.InsertOnSubmit(to);
            this.db.SubmitChanges();
            this.LoadData();
        }


        public void deleteType(int typeID)
        {
            var type = (from t in tableType
                    where t.typeID == typeID
                    select t).Single();
            type.deleted = true;
            this.db.SubmitChanges();
            this.LoadData();
        }

        public void deleteTypeObject(int typeObjectID)
        {
            var typeObject = (from to in tableTypeObject
                              where to.typeObjectID == typeObjectID
                              select to).Single();
            typeObject.deleted = true;
            this.db.SubmitChanges();
            this.LoadData();
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
        /// Alle vorhandenen Typen
        /// </summary>
        private ObservableCollection<datawrapper.Type> types;

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

        public bool IsDataLoaded { get; set; }

        public void updateType(int typeID, String title, String color) { 
            var type = (from t in tableType
                         where t.typeID == typeID
                         select t).Single();
            type.title = title;
            type.color = color;
            this.db.SubmitChanges();
        }

        public void updateTypeObject(int typeObjectID, String name, String color, String imageString) {
            var t = (from to in tableTypeObject
                    where to.typeObjectID == typeObjectID
                    select to).Single();
            t.name = name;
            t.color = color;
            t.imageString = imageString;
           
            this.db.SubmitChanges();
        }
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
                List<datawrapper.TypeObject> TypeObjects = new List<datawrapper.TypeObject>();
                foreach (TypeObject to in result)
                {
                    datawrapper.TypeObject wrappedTO = new datawrapper.TypeObject()
                    {
                        color = to.color,
                        type = wrappedType,
                        imageString = to.imageString,
                        name = to.name,
                        typeObjectID = to.typeObjectID,
                        used = to.used
                    };
                    TypeObjects.Add(wrappedTO);
                }

                // Neues Typobjekt
                TypeObjects.Add(new datawrapper.TypeObject() { 
                    name = "Neues Objekt", 
                    imageString = "../icons/add.png", 
                    type = new datawrapper.Type()  {typeID = -2},
                    color = "#919191" 
                }
                );

                wrappedType.typeObjects = TypeObjects;
                tmpTypes.Add(wrappedType);
            }

            // hinzufuegen fuer neuer Typ
            List<datawrapper.TypeObject> t_o = new List<datawrapper.TypeObject>();
            t_o.Add(new datawrapper.TypeObject()
            {
                name = "Neuer Typ anlegen",
                imageString = "../icons/add.png",
                type = new datawrapper.Type() { typeID = -1 },
                color = "#919191"
            });
            tmpTypes.Add(new datawrapper.Type() { title = "Neuer Typ", imageString = "../icons/add.png", color = "#919191", typeObjects = t_o, typeID = -1 });

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
