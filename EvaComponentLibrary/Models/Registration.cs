using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
	public class Registration
	{
		[JsonPropertyName("given_name")]
		public string GivenName {  get; set; }

		[JsonPropertyName("family_name")]
		public string FamilyName { get; set; }

		[JsonPropertyName("email")]
		public string Email {  get; set; }

		[JsonPropertyName("password")]
		public string Password { get; set; }

		[JsonPropertyName("iss")]
		public string Iss {  get; set; }

	}

}
