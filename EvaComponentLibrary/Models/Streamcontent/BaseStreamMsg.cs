using System.Text.Json.Serialization;

namespace EvaComponentLibrary.Models.Streamcontent
{
	[JsonDerivedType(typeof(BaseStreamMsg), typeDiscriminator: "base")]
	[JsonDerivedType(typeof(InitStreamMsg), typeDiscriminator: "first")]
	[JsonDerivedType(typeof(Token), typeDiscriminator: "token")]
	[JsonDerivedType(typeof(EndStreamMsg), typeDiscriminator: "final")]
	public class BaseStreamMsg{}
}
