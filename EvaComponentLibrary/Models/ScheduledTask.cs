using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
    public class ScheduledTask
    {
        [JsonPropertyName("jobID")]
        public string? JobId { get; set; } = string.Empty;

        [JsonPropertyName("userID")]
        public string UserID { get; set; }

        [JsonPropertyName("roomID")]
        public string RoomId { get; set; }


        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }


        [JsonPropertyName("websearch")]
        public string Websearch { get; set; }

        [JsonPropertyName("cron")]
        public string? Cron { get; set; }

        [JsonPropertyName("trigger")]
        public List<string> Trigger { get; set; } = new List<string>(); // V6

    }
}
