using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
    public class Response
    {
        [JsonPropertyName("inst")]
        public string Resp { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
