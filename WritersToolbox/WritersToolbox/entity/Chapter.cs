using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using System.Windows.Media;
using System.ComponentModel;


namespace WritersToolbox.entity
{
    /// <summary>
    /// Repraesentiert ein Entity vom Kapitel.
    /// </summary>
    [Table(Name = "Chapters")]
    class Chapter 
    {
        [Column(IsPrimaryKey = true,
            AutoSync = AutoSync.OnInsert,
            DbType = "INT IDENTITY",
            IsDbGenerated = true)]
        public int chapterID { get; set; }

        [Column(CanBeNull = false)]
        public String title { get; set; }

        public Color color { get; set; }

        [Column(CanBeNull = false)]
        public int chapterNumber { get; set; }

        [Column(Name = "fk_tomeID")]
        private int? fk_tomeID;

        private EntityRef<Tome> _tomes = new EntityRef<Tome>();

        [Association(Name = "FK_Chapter_Tome",
            IsForeignKey = true,
            Storage = "_tomes",
            ThisKey = "fk_tomeID")]
        public Tome obj_tome
        {
            get { return _tomes.Entity; }
            set { _tomes.Entity = value; }
        }
    }
}
