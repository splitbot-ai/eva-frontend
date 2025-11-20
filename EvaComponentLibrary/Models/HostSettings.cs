
namespace EvaComponentLibrary.Models
{
    public class HostSettings
    {
        public const string Settings = "HostSettings";

        public string? Brandname {get; set;} = string.Empty;

        public bool isBeta { get; set; } = false;

        public string? SupportMailTo {get; set;} = string.Empty;

        public string? DsgvoLink {get; set;} = string.Empty;
        public string? Impressum {get; set;} = string.Empty;

        public string? DeleteAcc {get; set;} = string.Empty;

        public string? ResetPassword { get; set; } = string.Empty;


        
        public EndpointSettings Prod { get; set; }
        public EndpointSettings Dev { get; set; }

    }

}