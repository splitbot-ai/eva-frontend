using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.Legacy
{
    public class Reference
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }


        [JsonPropertyName("description")]
        public string Description { get; set; }


        [JsonPropertyName("icon")]
        public string Icon { get; set; }


        [JsonPropertyName("url")]
        public string Url { get; set; }


        [JsonPropertyName("quelle")]
        public int Quelle { get; set; }

    }
}
