using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
    public class Room
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("userID")]
        public string UserId { get; set; }

        [JsonPropertyName("roomID")]
        public string RoomId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

		[JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("roomType")]
		public RoomTypes RoomTypes { get; set; }

        [JsonPropertyName("pinned")]
        public bool Pinned { get; set; }

        [JsonPropertyName("unreadMessage")]
        public bool UnreadMessage { get; set; }

        [JsonPropertyName("archived")]
        public bool Archived { get; set; }

        [JsonPropertyName("trackContext")]
        public bool TrackContext { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

        [JsonPropertyName("createdAt")]
        public double UnixCreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public double UnixupdatedAt { get; set; }

        [JsonIgnore]
        public DateOnly CreatedDate
        {
            get
            {
                var unixTimestampMilliseconds = (long)(UnixupdatedAt * 1000);
                DateTimeOffset utcTime = DateTimeOffset.FromUnixTimeMilliseconds(unixTimestampMilliseconds);
                return DateOnly.FromDateTime(utcTime.ToLocalTime().Date);
            } 
        }

        [JsonIgnore]
        public bool ActiveAnimation { get; set; }

    }
}