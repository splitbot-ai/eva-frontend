using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Forms;

namespace EvaComponentLibrary.Models
{
    public class FileUploadInfo
    {
        public byte[] Data { get; set; }
        public string Name { get; set; }
        public double Size { get; set; }
        public string ContentType {  get; set; }
        public bool? Success{get;set;} = null;
        public bool? IsSending { get; set; } = false;
        public double UploadedBytes { get; set; } = 0;
        public double UploadedPercentage => (Math.Sqrt((double)UploadedBytes / (double)Size) * 100d);
    }
}
