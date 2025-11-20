using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.Agent
{
    public  class CreateAgent
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("urls")]
        public List<string> Urls { get; set; } = new();
    }
}
