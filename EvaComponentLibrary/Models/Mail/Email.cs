using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace EvaComponentLibrary.Models.Mail
{
    public class Email
    {
        [JsonPropertyName("accountID")]
        public string? AccountID { get; set; }

        [JsonPropertyName("userID")]
        public string? UserID { get; set; }
        
        [JsonPropertyName("provider")]
        public string? Provider { get; set; }

        [JsonPropertyName("email")]
        public string? EMail { get; set; }

        [JsonPropertyName("expiresIn")]
        public double? ExpiresIn { get; set; }

        [JsonPropertyName("lastCrawl")]
        public double? LastCrawl { get; set; }
        
        [JsonPropertyName("crawledMails")]
        public int CrawledMails { get; set; }
        
        [JsonPropertyName("crawlRetention")]
        public int CrawlRetention { get; set; }

        [JsonPropertyName("attachment")]
        public bool Attachment { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }




        [JsonIgnore]
        public bool IsModified { get; set; }

        [JsonIgnore]
        public bool expand { get; set; }
    }
}
