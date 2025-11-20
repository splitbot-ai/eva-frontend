using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.CloudBase.NextCloud
{
    public class NextCloudAccountInfo : CloudAccountInfo
    {
        [JsonPropertyName("accountID")]
        public string AccountId { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonIgnore]
        public override string SearchableText => $"{Url} {UserName} {AccountId}";
    }
}
