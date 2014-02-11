using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;

namespace WritersToolbox.viewmodels
{
    public class TomeDetailViewModel
    {
        public datawrapper.Tome Tome { get; set; }
        private Table<Tome> tableTomes;
        private Table<Chapter> tableChapters;
        private Table<Event> tableEvents;
        private WritersToolboxDatebase wtb;
        private String title;

        public TomeDetailViewModel(int tomeID) {
            wtb = WritersToolboxDatebase.getInstance();
            tableTomes = wtb.GetTable<Tome>();
            tableChapters = wtb.GetTable<Chapter>();
            tableEvents = wtb.GetTable<Event>();

            Tome sqlTome = (from tome in tableTomes
                          where tome.tomeID == tomeID
                          select tome).First();

            datawrapper.Tome t = new datawrapper.Tome() { 
                addedDate = sqlTome.addedDate,
                book = null,
                deleted = sqlTome.deleted,
                title = sqlTome.title,
                tomeID = sqlTome.tomeID,
                tomeNumber = sqlTome.tomeNumber,
                updatedDate = sqlTome.updatedDate
            };

            this.Tome = t;
        }

        public String getTitle()
        {
            return title;
        }
    }
}
