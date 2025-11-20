using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
    public class RoomTitle
    {
        [JsonPropertyName("roomID")]
        public string RoomId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

    }
}
