using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EvaComponentLibrary.Languages;
using System.Globalization;
using Microsoft.Extensions.Localization;


namespace EvaComponentLibrary.Models.Mail
{
    public class IMAP
    {
        [JsonPropertyName("host")]
        [Required(
            ErrorMessageResourceType = typeof(ErrorMessage),
            ErrorMessageResourceName = nameof(ErrorMessage.HostRequired)
            )]
        [RegularExpression(
            @"^(?!\d{1,3}(?:\.\d{1,3}){3}$)(?!\[?[A-F0-9:]+\]?$)(?=.{1,253}$)(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?)(?:\.(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?))*\.?$",
            ErrorMessageResourceType = typeof(ErrorMessage),
            ErrorMessageResourceName = nameof(ErrorMessage.ValidHost)
            )]
        public string Host { get; set; }

        [JsonPropertyName("port")]
        [Required(
            ErrorMessageResourceType = typeof(ErrorMessage),
            ErrorMessageResourceName = nameof(ErrorMessage.PortRequired))]
        [Range(1, 65535, 
            ErrorMessageResourceType = typeof(ErrorMessage),
            ErrorMessageResourceName = nameof(ErrorMessage.PortRange))]
        public int Port { get; set; } = 993;


        [JsonPropertyName("username")]
        [Required(
            ErrorMessageResourceType = typeof(ErrorMessage),
            ErrorMessageResourceName = nameof(ErrorMessage.UsernameRequired))]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        [Required(
            ErrorMessageResourceType = typeof(ErrorMessage), 
            ErrorMessageResourceName = nameof(ErrorMessage.PassRequired))]
        public string Password { get; set; }

        [JsonPropertyName("crawlRetention")]
        [Range(1, 30,
            ErrorMessageResourceType = typeof(ErrorMessage),
            ErrorMessageResourceName = nameof(ErrorMessage.CrawlRetentionError))]
        public int CrawlRetention { get; set; } = 30;

        [JsonPropertyName("attachment")]
        public bool Attachment {  get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; } = true;
    }
}
