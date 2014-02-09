using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.datawrapper
{
    public class SoundData
    {
        //Path des Memos.
        public string filePath { get; set; }

        /// <summary>
        /// Um die Objekte des Memos zu vergleiche.
        /// </summary>
        /// <param name="obj">Memo</param>
        /// <returns>True wenn es den Path der beiden Objecte gleich ist</returns>
        public override bool Equals(object obj)
        {
            return this.filePath.Equals(((SoundData)obj).filePath);
        }

        /// <summary>
        /// Um die Collections von Memo zu vergelichen.
        /// </summary>
        /// <param name="c1">erste Collection</param>
        /// <param name="c2">zweite Collection</param>
        /// <returns>True wenn die beide Collection mit gleichen Inhat sind</returns>
        public static bool isSoundCollectionEquals(ObservableCollection<SoundData> c1, ObservableCollection<SoundData> c2)
        {
            bool isequals = false;
            if (c1.Count == c2.Count)
            {
                if (c1.Count == 0 && c2.Count == 0)
                {
                    isequals = true;
                }
                for (int i = 0; i < c1.Count && !isequals; i++)
                {
                    isequals = c1[i].Equals(c2[i]);
                }
            }
            return isequals;
        }
    }
}
