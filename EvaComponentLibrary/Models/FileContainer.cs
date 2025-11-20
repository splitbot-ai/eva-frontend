using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
    public class FileContainer
    {
        [JsonPropertyName("fileName")]
        public string Name { get; set; }
        
        [JsonPropertyName("fileID")]
        public string Id { get; set; }

        [JsonPropertyName("roleID")]
        public string? RoleId { get; set; }

        [JsonPropertyName("roleName")]
        public string? RoleName { get; set; }

        [JsonPropertyName("fileSource")]
        public FileTypes FileType { get; set; }

        [JsonPropertyName("fileLength")]
        public long FileLength { get; set; }
        
    }
}
