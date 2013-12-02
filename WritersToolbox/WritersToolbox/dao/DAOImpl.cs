using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using Windows.Storage;
using System.Diagnostics;
using WritersToolbox.entity;
namespace WritersToolbox.dao
{
    class DAOImpl : DAOInterface
    {
        public static readonly String DB_NAME = "toolbox_2.db";


        private static SQLiteConnection dbConn;

        public static void connect()
        {
            String dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_NAME);
            Debug.WriteLine("DB_PATH: " + dbPath);
            dbConn = new SQLiteConnection(dbPath);
            generateDatabaseStructure();
            closeConnection();
        }

        public static void closeConnection()
        {
            dbConn.Close();
        }

        /// <summary>
        /// Generieren der Datenbankstruktur.
        /// </summary>
        private static void generateDatabaseStructure()
        {
            if (dbConn != null)
            {
                // HINWEIS: Datenbanktabellen werden nur erstellt, wenn sie noch nicht vorhanden sind.
                try
                {
                    dbConn.CreateTable<MemoryNote>();
                    MemoryNote mn = new MemoryNote();
                    mn.contentText =("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz");
                    dbConn.Insert(mn);
                }
                catch (SQLiteException sqlex) 
                {
                    Debug.WriteLine(sqlex.Message);
                }    
            }
        }

        public int countUnsortedNotes()
        {
            return 0;
        }
    }
}
