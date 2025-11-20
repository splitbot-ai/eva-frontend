using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaMobil.Services
{
    public class PlatformDistinguisher
    {
        public string OSplatform  { get; private set; }

        public PlatformDistinguisher()
        {
#if IOS 

            OSplatform = "ios mobile";
#else
            OSplatform = "android mobile";
#endif

        }

    }
}
