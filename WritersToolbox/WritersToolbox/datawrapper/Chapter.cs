using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.datawrapper
{
    public class Chapter
    {
        public int chapterID { get; set; }
        public string title { get; set; }
        public int chapterNumber { get; set; }
        public DateTime addedDate { get; set; }
        public DateTime updatedDate { get; set; }
        public List<Event> events { get; set; }
        public Tome tome { get; set; }
        public bool deleted { get; set; }

        //public static datawrapper.Chapter getChapter(models.Chapter _chapter)
        //{
        //    datawrapper.Chapter tempChapter = new datawrapper.Chapter()
        //    {
        //        chapterID = _chapter.chapterID,
        //        addedDate = _chapter.addedDate,
        //        chapterNumber = _chapter.chapterNumber,
        //        deleted = _chapter.deleted,
        //        events = null,
        //        tome = null,
        //        updatedDate = _chapter.updatedDate
        //    };
        //    return tempChapter;
        //}
    }
}
