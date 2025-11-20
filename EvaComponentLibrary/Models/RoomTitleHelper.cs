using System.Text.Json.Serialization;

namespace EvaComponentLibrary.Models
{
	public class RoomTitleHelper
	{
		[JsonPropertyName("roomID")]
		public string RoomId { get; set; }
		[JsonPropertyName("title")]
		public string Title { get; set; }
	}
}
