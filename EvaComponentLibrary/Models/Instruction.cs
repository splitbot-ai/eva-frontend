using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
    public class Instruction
    {
        [JsonPropertyName("inst")]
        public string Inst { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
