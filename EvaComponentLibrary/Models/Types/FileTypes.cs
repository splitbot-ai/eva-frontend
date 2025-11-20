using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FileTypes
{
	NONE,
	[EnumMember(Value = "Upload")]
	UPLOAD,
	[EnumMember(Value = "Onedrive")]
	ONEDRIVE,
	[EnumMember(Value = "Nextcloud")]
	NEXTCLOUD
}