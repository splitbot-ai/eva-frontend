using System.Text.Json.Serialization;

namespace EvaComponentLibrary.Models
{
    public class Website
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("depth")]
        public int Depth { get; set; }
        [JsonPropertyName("refresh")]
        public bool ShouldRefresh { get; set; }
        [JsonPropertyName("last_update")]
        public string LastUpdated { get; set; }
        [JsonPropertyName("subsites")]
        public List<string> SubSites { get; set; } = new List<string>();
    }
}
