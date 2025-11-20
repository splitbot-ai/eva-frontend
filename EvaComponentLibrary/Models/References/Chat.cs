using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models.References
{
	public class Chat : Reference
	{
		[JsonPropertyName("room_id")]
		public string RoomId { get; set; }
	}
}
