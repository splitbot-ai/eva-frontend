using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.References
{
    [JsonDerivedType(typeof(Files), typeDiscriminator: "file")]
    [JsonDerivedType(typeof(Links), typeDiscriminator: "website")]
    [JsonDerivedType(typeof(Emails), typeDiscriminator: "email")]
	[JsonDerivedType(typeof(Chat), typeDiscriminator: "chat")]

	public class Reference
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("number")]
        public double Number { get; set; }

        [JsonPropertyName("chunks")]
        public List<string> Chunks { get; set; } = new();

        [JsonIgnore]
        public bool IsExtended { get; set; }

    }
}
