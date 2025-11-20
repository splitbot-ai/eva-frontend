using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
    public class RoomList
    {
        [JsonPropertyName("userID")]
        public string UserId { get; set; }
        [JsonPropertyName("rooms")]
        public List<Room> Rooms { get; set; } = new List<Room>();

    }
}
