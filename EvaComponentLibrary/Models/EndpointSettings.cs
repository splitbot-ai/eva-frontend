using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
    public class EndpointSettings
    {
        public string Host { get; set; } = string.Empty;
        public string Realm { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
        public string iss { get; set; } = string.Empty;
    }
}
