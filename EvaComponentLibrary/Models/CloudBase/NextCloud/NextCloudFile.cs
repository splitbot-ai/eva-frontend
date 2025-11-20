using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.CloudBase.NextCloud
{
    public class NextCloudFile : CloudFileBase
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

    }
}
