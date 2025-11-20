using System.Text.Json.Serialization;

namespace EvaComponentLibrary.Models.Streamcontent
{

	public class Token : BaseStreamMsg
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();

		[JsonPropertyName("next_token")]
		public string NextToken { get; set; }
	}

}
