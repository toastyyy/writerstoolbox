using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.sampleData
{
    public class SampleDataTyp
    {
        public SampleDataTyp()
        {
            Typen = new List<Typ>();
        }
        public List<sampleData.Typ> Typen { get; set; }
    }
}
