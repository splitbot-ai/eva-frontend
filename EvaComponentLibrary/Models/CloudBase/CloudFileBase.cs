using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.CloudBase
{
    public class CloudFileBase
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Status";

        [JsonPropertyName("file_id")]
        public string FileId { get; set; }

        [JsonPropertyName("last_sync")]
        public string LastSync { get; set; }

        [JsonPropertyName("selected")]
        public bool Selected { get; set; }

        [JsonPropertyName("excluded")]
        public bool Excluded { get; set; }

        [JsonIgnore]
        public bool? CheckBoxState
        {
            get
            {
                if (this.Selected)
                {
                    return true;
                }
                if (this.Excluded)
                {
                    return false;
                }
                return null;
            }

            set
            {
                if (value.HasValue)
                {
                    this.Selected = value.Value;
                    this.Excluded = !value.Value;
                }
                else
                {
                    this.Selected = true;
                    this.Excluded = false;
                }

            }
        }
    }
}
