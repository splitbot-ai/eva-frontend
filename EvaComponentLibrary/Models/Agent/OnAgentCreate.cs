using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.Agent
{
    public class OnAgentCreate
    {
        [JsonPropertyName("message")]

        public string Message { get; set; }

        [JsonPropertyName("roomID")]
        public string RoomId { get; set; }


    }
}
