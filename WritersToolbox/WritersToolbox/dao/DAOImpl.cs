using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using Windows.Storage;

namespace WritersToolbox.dao
{
    class DAOImpl : DAOInterface
    {
        public static readonly String DB_NAME = "toolbox.db";


        private static SQLiteConnection dbConn;

        public static void connect()
        {
            String dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_NAME);
            DAOImpl.dbConn = new SQLiteConnection(dbPath);
        }

        public int countUnsortedNotes()
        {
            return 0;
        }
    }
}
