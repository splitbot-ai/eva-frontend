using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
    public  class Role
    {
        [JsonPropertyName("roleName")]
        public string RoleName { get; set; }
        [JsonPropertyName("roleID")]
        public string RoleId { get; set; }
    }
}
