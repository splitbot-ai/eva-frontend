using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
    public class FileWithAnnotations
    {
        [JsonPropertyName("file_name")]
        public string FileName { get; set; }
        
        [JsonPropertyName("base64")]
        public string Base64 { get; set; }

        [JsonPropertyName("first_page")]
        public int FirstPage { get; set; }

    }
}
