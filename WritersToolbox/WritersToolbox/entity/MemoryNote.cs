using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace WritersToolbox.entity
{
    /// <summary>
    /// Repraesentiert ein Entity vom Typ Notiz / Zettel.
    /// </summary>
    class MemoryNote
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public String contentText { get; set; }
        public String ImagePaths { get; set; }
        public String AudioPaths { get; set; }
    }
}
