using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
	public class CustomInstruction
	{
		[JsonPropertyName("userID")]
		public string UserId { get; set; }

		[JsonPropertyName("customInstructions")]
		public List<Instruction> CunstInst {  get; set; } = new List<Instruction>();

		[JsonPropertyName("responseInstructions")]
		public List<Response> CunstRes { get; set; } = new List<Response>();

		[JsonPropertyName("customIndex")]
		public int InstIndex { get; set; }

		[JsonPropertyName("responseIndex")]
		public int ResIndex { get; set; }
	}
}
