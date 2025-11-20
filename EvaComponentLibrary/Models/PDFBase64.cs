using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace EvaComponentLibrary.Models
{
    public class PDFBase64
    {
        [JsonPropertyName("base64")]
        public string Base64 { get; set; }
    }
}
