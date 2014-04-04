using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WritersToolbox.models
{
    public interface ISearchable
    {
        /// <summary>
        /// Gibt den Titel für ein Suchergebnis zurück.
        /// </summary>
        /// <returns>Titel des Suchergebnisses</returns>
        String Title
        {
            get;
            set;
        }
        /// <summary>
        /// Gibt weitere Informationen für die Darstellung in den Suchergebnissen zurück.
        /// </summary>
        /// <returns>Untertitel des Suchergebnisses</returns>
        String Subtitle
        { get; set; }
        /// <summary>
        /// Gibt die Uri zurück, die bei einer Aktion mit dem Suchergebnis ausgeführt werden soll.
        /// </summary>
        /// <returns>Uri der Aktion als Pfad zu einer Xaml-Datei.</returns>
        Uri Link {get;set;}
        /// <summary>
        /// Gibt zurück, ob ein Objekt ein bestimmtes Suchkriterium erfüllt.
        /// </summary>
        /// <param name="query">Suchkriterium</param>
        /// <returns>True, wenn das Suchkriterium zutrifft, ansonsten False</returns>
        Boolean matchesQuery(String query);
    }
}
