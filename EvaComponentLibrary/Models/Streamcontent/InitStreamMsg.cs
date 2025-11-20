using System.Text.Json.Serialization;

namespace EvaComponentLibrary.Models.Streamcontent
{
	public class InitStreamMsg : BaseStreamMsg
	{
		//[JsonPropertyName("$type")]
		//public string Type = "first";
		[JsonPropertyName("userID")]
		public string UserId { get; set; }
		[JsonPropertyName("roomID")]
		public string RoomId { get; set; }
		[JsonPropertyName("isHuman")]
		public bool IsHuman { get; set; }
		[JsonPropertyName("counter")]
		public int Counter { get; set; }
	}
}
