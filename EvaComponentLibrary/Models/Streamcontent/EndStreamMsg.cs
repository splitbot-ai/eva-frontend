using System.Text.Json.Serialization;
using EvaComponentLibrary.Models.References;
namespace EvaComponentLibrary.Models.Streamcontent
{
	public class EndStreamMsg : BaseStreamMsg
	{
		//[JsonPropertyName("$type")]
		//public string Type = "final";
		[JsonPropertyName("message")]
		public string Message { get; set; }
		[JsonPropertyName("references")]
		public List<Reference> References { get; set; }
	}
}
