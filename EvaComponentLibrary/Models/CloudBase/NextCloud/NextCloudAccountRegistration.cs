using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EvaComponentLibrary.Languages;

namespace EvaComponentLibrary.Models.CloudBase.NextCloud
{
    public class NextCloudAccountRegistration
    {
        [JsonPropertyName("url")]
        [Required(
            ErrorMessageResourceType = typeof(ErrorMessage),
            ErrorMessageResourceName = nameof(ErrorMessage.HostRequired)
        )]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("username")]
        [Required(
           ErrorMessageResourceType = typeof(ErrorMessage),
           ErrorMessageResourceName = nameof(ErrorMessage.UsernameRequired))]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        [Required(
            ErrorMessageResourceType = typeof(ErrorMessage),
            ErrorMessageResourceName = nameof(ErrorMessage.PassRequired))]
        public string Password {get; set;} = string.Empty;
    }
}
