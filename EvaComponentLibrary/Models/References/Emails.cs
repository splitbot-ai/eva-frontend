using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.References
{
    public class Emails : Reference
    {
        [JsonPropertyName("subject")]
        public string Subject { get; set; } = string.Empty;
        
        [JsonPropertyName("send_from")]
        public string SendFrom { get; set; } = string.Empty;

        [JsonPropertyName("send_to")]
        public string SendTo { get; set; } = string.Empty;
        
        [JsonPropertyName("date")]
        public string Date {  get; set; } = string.Empty;

    }
}
