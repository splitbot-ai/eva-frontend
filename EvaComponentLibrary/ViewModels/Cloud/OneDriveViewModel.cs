using System.Text.Json;
using EvaComponentLibrary.Models;
using EvaComponentLibrary.Models.CloudBase;
using EvaComponentLibrary.Models.CloudBase.Ondrive;
using EvaComponentLibrary.Services;

namespace EvaComponentLibrary.ViewModels.Cloud
{
    public class OneDriveViewModel
    {
        public readonly WebServices _ws;
        public readonly CloudBaseViewModel _cbvm;
        public string OneDriveId { get; set; } = string.Empty;
        public OneDriveAccountInfo? Info;
        public List<OneDriveFile> OneDriveDirectories { get; set; } = new List<OneDriveFile>();

        public OneDriveViewModel(WebServices service, CloudBaseViewModel cloudBaseViewModel)
        {
            _ws = service ?? throw new ArgumentNullException(nameof(service));
            _cbvm = cloudBaseViewModel ?? throw new ArgumentNullException(nameof(cloudBaseViewModel));
        }

        public async Task GetAllOnedriveAccountsAsync()
        {
            try
            {
                var response = await _ws.GetOneDriveAccountsAsync();

                if (response.Equals("Unauthorized")) return;

                var account = JsonSerializer.Deserialize<OneDriveAccountInfo>(response);

                if (account == null) return;

                if (!_cbvm.CloudAccounts.Any(o => o is OneDriveAccountInfo oDrive &&
                   o.UserName.Equals(account.UserName)))
                {
                    Console.WriteLine($"OneDrive found: {account.UserName}");
                    _cbvm.CloudAccounts.Add(account);
					Info = account;

				}

            }
            catch (Exception ex)
            {
                Console.WriteLine("Getting OneDrive-Account went wrong");
            }
        }

        public void OnSelectOneDriveFile(OneDriveFile file) 
        {
            _cbvm.FilesToImport.IncludedItems.RemoveAll(i => i.RemoteId == file.RemoteId );
            _cbvm.FilesToImport.ExcludedItems.RemoveAll(i => i.RemoteId == file.RemoteId );

            switch (file)
            {
                case { Selected: true }:
                    _cbvm.FilesToImport.IncludedItems.Add(new() { RemoteId = file.RemoteId, DriveId = file.DriveId });
                    break;
                case { Excluded: true }:
                    _cbvm.FilesToImport.ExcludedItems.Add(new() { RemoteId = file.RemoteId, DriveId = file.DriveId });
                    break;
            }
        }
        public async Task<string> GetOneDriveAuth()
        {
            var result = await _ws.GetOneDriveAuth();
            if (!string.IsNullOrEmpty(result) || !result.Equals("Unauthorized", StringComparison.OrdinalIgnoreCase))
            {
                return result.Replace("\"", string.Empty);
            }
            return result;
        }
        public async Task<string> GetOneDriveDirectory(string remoteId)
        {
            var result = await _ws.GetOneDriveDirectory(OneDriveId, remoteId);
            if (result.Equals("Error"))
            {
                return result;
            }

            try
            {
                OneDriveDirectories = JsonSerializer.Deserialize<List<OneDriveFile>>(result);
            }
            catch (JsonException)
            {

                return "Error";
            }

            if (OneDriveDirectories == null || !OneDriveDirectories.Any())
            {
                return "OK";
            }
            if(OneDriveDirectories.Any())
            {
                TrackuserSelectedItems();
            }
            return "OK";
        }
        public async Task<string> PostOneDriveItems()
        {
            var result = await _ws.PostOneDriveItems(_cbvm.FilesToImport);
            return result;
        }
        public async Task<string> StartOneDriveSync()
        {
            var result = await _ws.StartOneDriveSync();
            return result switch
            {
                "done" => "OK",
                null or "" => "Error" ,
                "Unauthorized" => "Unauthorized" ,
                string re when CloudBaseViewModel.TryDeserialize(result, out var status) => status,
                _ => "Error"
            };
        }

		public async Task<string> DeleteOneDriveAccount()
		{
            
            _cbvm.CloudAccounts.Remove(Info);
            Info = null;
			var result = await _ws.DeleteOndriveAsync();
            return result;
		}

		public void ChangeAllitemStateOneDrive(bool? newState)
        {
            foreach (var item in OneDriveDirectories)
            {
                item.CheckBoxState = newState;
                if (newState == true)
                {
                    if (_cbvm.ExcludedPathsHashSet.Any(x => x.RemoteId.Equals(item.RemoteId)))
                    {
                        _cbvm.FilesToImport.ExcludedItems.RemoveAll(x => x.RemoteId.Equals(item.RemoteId));

                    }
                    _cbvm.FilesToImport.IncludedItems.Add(new CloudFileImport
                    {
                        RemoteId = item.RemoteId,
                        DriveId = OneDriveId
                    });
                }
                if (newState == false)
                {
                    if (_cbvm.IncludedPathsHashSet.Any( x => x.RemoteId.Equals(item.RemoteId)))
                    {
                        _cbvm.FilesToImport.IncludedItems.RemoveAll(x => x.RemoteId.Equals(item.RemoteId));

                    }
                    _cbvm.FilesToImport.ExcludedItems.Add(new CloudFileImport
                    {
                        RemoteId = item.RemoteId,
                        DriveId = OneDriveId
                    });
                }
            }
        }
        private void TrackuserSelectedItems() 
        {
            foreach (var item in OneDriveDirectories)
            {
                if (_cbvm.IncludedPathsHashSet.Any(x => x.RemoteId.Equals(item.RemoteId)))
                {
                    item.CheckBoxState = true;
                }
                else if (_cbvm.ExcludedPathsHashSet.Any(x => x.RemoteId.Equals(item.RemoteId)))
                {
                    item.CheckBoxState = false;
                }
            }
        }

        
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //private void SelectAllSubFiles(bool? include) // select all childfiles when the parent is selected
        //{
        //    if(include is not null)
        //    {
        //        foreach (var item in OneDriveDirectories)
        //        {
        //            item.CheckBoxState = (bool)include;
        //            if (include == true)
        //            {
        //                _cbvm.FilesToImport.IncludedItems.Add(new()
        //                {
        //                    RemoteId = item.RemoteId,
        //                    DriveId = item.Drive_id
        //                });

        //            }
        //            else
        //            {
        //                _cbvm.FilesToImport.ExcludedItems.Add(new()
        //                {
        //                    RemoteId = item.RemoteId,
        //                    DriveId = item.Drive_id
        //                });
        //            }
        //        }
        //    }
        //}
        //private bool? GetInclusionFlag(string remoteId)
        //{
        //    if (_cbvm.FilesToImport?.IncludedItems?.Any(x => x.RemoteId == remoteId) == true)
        //        return true;

        //    if (_cbvm.FilesToImport?.ExcludedItems?.Any(x => x.RemoteId == remoteId) == true)
        //        return false;

        //    return null;
        //}
    }
}