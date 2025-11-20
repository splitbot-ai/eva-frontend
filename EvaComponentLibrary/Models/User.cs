using System.Text.Json.Serialization;

namespace EvaComponentLibrary.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("userID")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("maxStorage")]
        public long MaxStorage { get; set; } = 0;

        [JsonPropertyName("usedStorage")]
        public long UsedStorage { get; set; }
        [JsonPropertyName("roles")]
        public List<Role>Roles { get; set; }

        //[JsonPropertyName("roles")]               //in v6
        //public List<Role>Roles { get; set; }

    }
}
