using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.datawrapper
{
    public class KeyedList<TKey, TItem> : List<TItem>
    {


        public TKey Key { set; get; }
        public IEnumerable<TItem> Items { set; get; }

        public KeyedList(TKey key, IEnumerable<TItem> items)
            : base(items)
        {
            Key = key;
            Items = items;
        }

        public KeyedList(IGrouping<TKey, TItem> grouping)
            : base(grouping)
        {
            Key = grouping.Key;
        }

        public KeyedList()
        {
        }

    }
}
