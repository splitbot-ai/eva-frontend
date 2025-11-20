using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.CloudBase
{
    public class CloudFileImport
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("remote_id")]
        public string RemoteId { get; set; }

        [JsonPropertyName("drive_id")]
        public string DriveId { get; set; }
    }
}
