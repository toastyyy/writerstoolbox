using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritersToolbox
{
    class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static WritersToolbox.App localizedresources = new WritersToolbox.App();

        public WritersToolbox.App Localizedresources { get { return localizedresources; } }

    }
}
