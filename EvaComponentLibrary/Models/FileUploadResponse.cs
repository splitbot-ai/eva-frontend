using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
    public class FileUploadResponse
    {
        [JsonPropertyName("fileID")]

        public string FileId { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("fileSize")]
        public int FileSize { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
