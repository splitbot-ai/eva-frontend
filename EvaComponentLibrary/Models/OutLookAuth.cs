using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EvaComponentLibrary.Languages;

namespace EvaComponentLibrary.Models
{
    public class OutLookAuth
    {
        [JsonPropertyName("email")]
        [Required(
            ErrorMessageResourceType = typeof(ErrorMessage),
            ErrorMessageResourceName = nameof(ErrorMessage.UsernameRequired))]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        [Required(
            ErrorMessageResourceType = typeof(ErrorMessage),
            ErrorMessageResourceName = nameof(ErrorMessage.PassRequired))]
        public string Password { get; set; }

    }
}
