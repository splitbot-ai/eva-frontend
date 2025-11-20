using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.OutLook
{
    public class OutLookAccount
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }   
    }
}
