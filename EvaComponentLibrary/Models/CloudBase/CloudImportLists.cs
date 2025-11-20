using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.CloudBase
{
    public class CloudImportLists
    {
        //[JsonPropertyName("included_items")]
        [JsonPropertyName("included_items")]
        public List<CloudFileImport> IncludedItems { get; set; } = new();

        //[JsonPropertyName("excluded_items")]
        [JsonPropertyName("excluded_items")]
        public List<CloudFileImport> ExcludedItems { get; set; } = new();
    }
}
