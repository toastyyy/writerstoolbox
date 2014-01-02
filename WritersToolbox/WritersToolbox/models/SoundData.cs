using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox.models
{
    public class SoundData
    {
        public string FilePath { get; set; }

        public override bool Equals(object obj)
        {
            return this.FilePath.Equals(((SoundData)obj).FilePath);
        }
    }
}
