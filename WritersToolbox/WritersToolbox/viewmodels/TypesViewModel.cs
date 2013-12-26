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
namespace WritersToolbox.viewmodels
{
    class TypesViewModel
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
        public String getTitleForType(int typeID) 
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

            if(imagePath.Equals("")) 
            {
                return null;
            } else {
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
            t.title = title;
            t.color = color;
            t.imageString = image;
            this.tableType.InsertOnSubmit(t);
            this.db.SubmitChanges();
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
        }

        private Color fromHexToColor(String hex)
        {
            Byte colorR = Convert.ToByte(hex.Substring(0, 2));
            Byte colorG = Convert.ToByte(hex.Substring(2, 2));
            Byte colorB = Convert.ToByte(hex.Substring(4, 2));

            return Color.FromArgb(255, colorR, colorG, colorB);
        }
    }
}
