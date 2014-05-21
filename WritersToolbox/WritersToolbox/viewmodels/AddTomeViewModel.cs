using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.datawrapper;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Data.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Phone.BackgroundAudio;
using System.IO;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System.Diagnostics;
namespace WritersToolbox.viewmodels

{
    /// <summary>
    /// Die AddTomeViewModel Klasse bzw. Präsentations-Logik ist eine aggregierte Datenquelle,
    /// die verschiedene Daten von Tome und ihre entsprechende Eigenschaften bereitstellt.
    /// </summary>
    class AddTomeViewModel
    {
        private models.WritersToolboxDatebase db;

        //tabelle tome
        private Table<models.Tome> tomeTable;
        //tabelle werk
        private Table<Book> tableBook; 
        //entity tome
        private models.Tome tome;

        /// <summary>
        /// Erzeugt eine neue Instanz des Viewmodels und läd Datenbanktabellen.
        /// </summary>
         public AddTomeViewModel () 
        {
            try
            {
                db = models.WritersToolboxDatebase.getInstance();
                tomeTable = db.GetTable<models.Tome>();
                this.tableBook = this.db.GetTable<Book>();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        /// <summary>
        /// Gibt den Titel des Bandes mit der angegebenen ID zurück.
        /// </summary>
        /// <param name="tomeID">Band ID</param>
        /// <returns>Titel des Bandes</returns>
         public string getTitle(int tomeID)
         {
             string title = (from tome in tomeTable
                             where tome.tomeID == tomeID
                             select tome.title).FirstOrDefault();

             return title == null ? "" : title;
         }

        /// <summary>
        /// Speichert Änderungen oder ein neues Tome - Objekt in der Datenbank.
        /// </summary>
        /// <param name="tomeID">ID des Bandes. Wenn 0 wird ein neuer Band angelegt</param>
        /// <param name="addedDate">Datum (Hinzugefügt)</param>
        /// <param name="title">Titel des Bandes</param>
        /// <param name="updatedDate">Datum (Aktualisierung)</param>
        /// <param name="bookType">Buchtyp [derzeit nicht verwendet]</param>
         public void save(int tomeID, DateTime addedDate, string title, DateTime updatedDate, datawrapper.BookType bookType)
         {
             try
             {
                 if (tomeID == 0) //neue MemoryNote speichern.
                 {
                     tome = new models.Tome();
                     tome.addedDate = new DateTime(addedDate.Year, addedDate.Month, addedDate.Day);
                     tome.title = title;
                     tome.updatedDate = new DateTime(updatedDate.Year, updatedDate.Month, updatedDate.Day); ;

                     //tome in DataContext hinzufügen.
                     db.GetTable<models.Tome>().InsertOnSubmit(tome);
                 }
                 else //Änderungen an einem tome speichern.
                 {
                     tome = db.GetTable<models.Tome>().Single(tomes => tome.tomeID == tomeID);

                     tome.title = title;
                     tome.updatedDate = new DateTime(updatedDate.Year, updatedDate.Month, updatedDate.Day); ;

                 }


                 //Änderung in der Datenbank übertragen.
                 db.SubmitChanges();
             }
             catch (Exception ex)
             {
                 Console.WriteLine(ex.Message);
             }

         }
    }
}
