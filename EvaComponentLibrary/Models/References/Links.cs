using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.References
{
    class Links : Reference
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }
    }
}
