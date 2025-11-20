
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EvaComponentLibrary.Models
{
    public class Message
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; } = string.Empty;

        [JsonPropertyName("userID")]
        public string? UserId { get; set; } = string.Empty;

        [JsonPropertyName("roomID")]
        public string? RoomId { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string? Content { get; set; } = string.Empty;

        [JsonPropertyName("counter")]
        public int? Counter { get; set; } = 0;
        [JsonPropertyName("human")]
        public bool isHuman { get; set; }

        public List<References.Reference>? References = new();



		[JsonPropertyName("references")]
        public object? TempReferences
        {  
            get
            {
                return References;
            }
            set 
            {
                //References = new();
               //($"DAS IST VALUE: {value}");

                if (string.IsNullOrEmpty(value.ToString()))
                    return;

                if(value is JsonElement jsonElem)
                {
                    if (value.ToString().Contains("$type"))
                    {
                        References = JsonSerializer.Deserialize<List<References.Reference>>(jsonElem.GetRawText());
                    }
                    else if (jsonElem.ValueKind.Equals(JsonValueKind.Array))
                    {
                        if (jsonElem.EnumerateArray().Count() < 1)
                            return;

                        List<Legacy.Reference> legacy = JsonSerializer.Deserialize<List<Legacy.Reference>>(jsonElem.GetRawText()
                            , new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
                     

                        foreach (var item in legacy)
                        {
                            References.Add(new References.Links()
                            { 
                                Name = item.Title,
                                Number = (double)item.Quelle,
                                Url = item.Url,
                                Icon = item.Icon,
                            });
                        }
                    } 
                    else
                    {
                        References = new();
                    }
                }
            }
        }

        [JsonPropertyName("vote")]
        public int? Vote { get; set; } = 0;
        [JsonIgnore]
        private Feedback _feedback = Feedback.Nan;
		[JsonIgnore]
        public Feedback Feedback
        {
            get
            {
                if (Vote != 0)
                {
                    return Vote > 0 ? Feedback.Like : Feedback.Dislike;
                }

                return _feedback;
            }
            set
            {
                _feedback = value;
            }
        }

        [JsonPropertyName("websearch")]
        public string? DoWebsearch { get; set; }

        [JsonPropertyName("source_ids")]
		public List<string> SourceIds { get; set; } = new List<string>();

        [JsonPropertyName("all_keywords")]
		public List<string> AllKeywords{ get; set; } = new List<string>();
    
        [JsonPropertyName("any_keywords")]
		public List<string> AnyKeywords { get; set; } = new List<string>();

		[JsonPropertyName("version")]
		public string? Version { get; set; }

        [JsonPropertyName("roleIDs")]
        public List<Role> Roles { get; set; }

        [JsonIgnore]
        public bool IsFinished { get; set; } = true;

        [JsonIgnore]
        public string ImageBase64 { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsGeneratingImage { get; set; } = false;


    }
}
