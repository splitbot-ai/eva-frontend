using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.Agent
{
    public class MonitoringAgent
    {
        [JsonPropertyName("userID")]
        public string UserId { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("hashValue")]
        public string HashValue { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("lastCheck")]
        public double LastCheck { get; set; }

        [JsonPropertyName("roomID")]
        public string RoomId { get; set; }

        [JsonIgnore]
        public bool CheckBoxState { get; set; }

        [JsonIgnore]
        public LinkStatus Status { get; set; }

        [JsonIgnore]
        public string TimeInPast { get; set; }
    }
}
