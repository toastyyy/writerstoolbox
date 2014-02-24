using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.Resources;

namespace WritersToolbox
{
    /// <summary>
    /// Bietet Zugriff auf Zeichenfolgenressourcen.
    /// </summary>
    public class LocalizedStrings
    {
        private AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; }}

    }
}
