using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.CloudBase.Ondrive
{
    public class OneDriveAccountInfo : CloudAccountInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonIgnore]
        public override string SearchableText => $" {Name} {UserName}";
    }
}
