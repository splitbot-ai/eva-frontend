using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EvaComponentLibrary.Languages;
using Microsoft.AspNetCore.Components;

namespace EvaComponentLibrary.Models
{
    public class SaveNextCloudAuthentication
    {
        [JsonPropertyName("ncServerUrl")]
        [Required(
            ErrorMessageResourceType = typeof(ErrorMessage),
            ErrorMessageResourceName = nameof(ErrorMessage.HostRequired)
        )]
        public string ServerUrl { get; set; }

        [JsonPropertyName("ncUserName")]
        [Required(
           ErrorMessageResourceType = typeof(ErrorMessage),
           ErrorMessageResourceName = nameof(ErrorMessage.UsernameRequired))]
        public string UserName {  get; set; }

        [JsonPropertyName("ncAppPassword")]
        [Required(
            ErrorMessageResourceType = typeof(ErrorMessage),
            ErrorMessageResourceName = nameof(ErrorMessage.PassRequired))]
        public string Password { get; set; }

    }
}
