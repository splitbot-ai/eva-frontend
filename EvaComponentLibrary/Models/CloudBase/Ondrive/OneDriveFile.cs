using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EvaComponentLibrary.Models.CloudBase;

namespace EvaComponentLibrary.Models.CloudBase.Ondrive
{
    public class OneDriveFile : CloudFileBase
    {
        [JsonPropertyName("remote_id")]
        public string RemoteId { get; set; }

        [JsonPropertyName("drive_id")]
        public string DriveId { get; set; }

    }
}

    
