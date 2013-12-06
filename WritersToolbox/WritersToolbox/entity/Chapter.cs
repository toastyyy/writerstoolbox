using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace WritersToolbox.entity
{
    /// <summary>
    /// Repraesentiert ein Entity vom Kapitel.
    /// </summary>
    class Chapter
    {
        
        public int chapterID { get; set; }
        public String title { get; set; }
        String color { get; set; }
        int chapterNumber { get; set; }
        int tomeID { get; set; }
    }
}
