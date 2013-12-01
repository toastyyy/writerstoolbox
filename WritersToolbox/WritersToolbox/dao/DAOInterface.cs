using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.dao
{
    interface DAOInterface
    {
        /// <summary>
        /// Gibt die Anzahl der unsortierten Notizen zurueck.
        /// </summary>
        /// <returns>Anzahl der unsortierten Notizen</returns>
        int countUnsortedNotes();


    }
}
