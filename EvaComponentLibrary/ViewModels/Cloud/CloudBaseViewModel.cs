using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EvaComponentLibrary.Models;
using EvaComponentLibrary.Models.CloudBase;
using EvaComponentLibrary.Models.CloudBase.Ondrive;

namespace EvaComponentLibrary.ViewModels.Cloud
{
    public class CloudBaseViewModel
    {
        public ObservableCollection<CloudAccountInfo> CloudAccounts { get; set; } = new ObservableCollection<CloudAccountInfo>();
          
        public CloudImportLists FilesToImport { get; set; } = new();
        public HashSet<CloudFileImport> IncludedPathsHashSet => new HashSet<CloudFileImport>(FilesToImport.IncludedItems);
        public HashSet<CloudFileImport> ExcludedPathsHashSet => new HashSet<CloudFileImport>(FilesToImport.ExcludedItems);
        public static bool TryDeserialize(string json, out string status)
        {
            try
            {
                status = JsonSerializer.Deserialize<CloudSyncStatus>(json).Status;
                return status is not null;
            }
            catch (JsonException)
            {
                status = default!;
                return false;
            }
        }
    }
}
