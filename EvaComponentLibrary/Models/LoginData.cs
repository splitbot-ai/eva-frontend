

using System.Text.Json.Serialization;

namespace EvaComponentLibrary.Models
{
	public class LoginData
	{
		[JsonPropertyName("grant_type")]
		public readonly string GrantType = "password";

		[JsonPropertyName("client_id")]

		public readonly string Clientd = "EvaApp";

		[JsonPropertyName("username")]
		public string? UserName { get; set; }

		[JsonPropertyName("password")]
		public string? Password { get; set; }
	}
}
