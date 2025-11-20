using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaComponentLibrary.Models;

namespace EvaComponentLibrary.Services
{
    public class Logistic
    {

        public Action? update { get; set; }
        public Action? UpdateLanguage { get; set; }
        public Action? onSwipe { get; set; }
        
        public Action<bool>? TriggerSourcePullable { get; set; }
        public Action? UpdateSourcePullable { get; set; }
        public bool SourcePullableIsLoading { get; set; }
        
        public int NumberOfSelectedSources { get; set; }
        public Action<bool>? TriggerSourcesList { get; set; }
        
        public string? MessageIdSourcesList { get; set; }
        public string? RenameRoomName { get; set; }
        public string? RenameRoomId { get; set; }

        public Message? FeedBackMessage { get; set; }
        public string? FeedBackMessageId { get; set; }
        public Action<bool>? TriggerFeedBack {get; set;}
        public Action? UpdateFeedBack { get; set; }
        public bool Connected {  get; set; }

        public Action? ActivateTab { get; set; }

        public string InfoMessage_1 { get; set; } = string.Empty;
        public string InfoMessage_2 {  get; set; } = string.Empty;
        public bool OnInfoModalToggle { get; set; }
        public bool IsInfoButtonHide { get; set; } = true;
        public Action? UpdateInfoModal { get; set; }

        public Action? UpdateSidebarforTask { get; set; }
        public ScheduledTask EditTask { get; set; } = new();
        public Action? SetValueForEdit { get; set; } 
        public Action? UpdateTrashAndArchive { get; set; }

        public Action<bool>? TriggerMonitoringModalAgent { get; set; }
        public Action<bool>? TriggerGeneralModalAgent { get; set; }
    }
}
