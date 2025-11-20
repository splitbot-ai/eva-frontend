using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.CloudBase
{
    public class CloudAccountInfo
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonIgnore]
        public virtual string SearchableText => UserName;
    }
}
