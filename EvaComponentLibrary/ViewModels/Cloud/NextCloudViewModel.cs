using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EvaComponentLibrary.Models;
using EvaComponentLibrary.Models.CloudBase;
using EvaComponentLibrary.Models.CloudBase.NextCloud;
using EvaComponentLibrary.Models.CloudBase.Ondrive;
using EvaComponentLibrary.Services;

namespace EvaComponentLibrary.ViewModels.Cloud
{
    public class NextCloudViewModel
    {
        
        public List<NextCloudFile> NextCloudDirectories { get; set; } = new List<NextCloudFile>();
        public string NextCloudAccountId { get; set; }


        public readonly WebServices _ws;
        public readonly CloudBaseViewModel _cbvm;
        public NextCloudViewModel(WebServices service, CloudBaseViewModel cloudBaseViewModel)
        {
            _ws = service ?? throw new ArgumentNullException(nameof(service));
            _cbvm = cloudBaseViewModel ?? throw new ArgumentNullException(nameof(cloudBaseViewModel));

        }
        public void OnSelectNextCloudFile(NextCloudFile file)
        {
            _cbvm.FilesToImport.IncludedItems.RemoveAll(i => i.Path == file.Path);
            _cbvm.FilesToImport.ExcludedItems.RemoveAll(i => i.Path == file.Path);

            switch (file)
            {
                case { Selected: true }:
                    _cbvm.FilesToImport.IncludedItems.Add(new() { Path = file.Path});
                    break;
                case { Excluded: true }:
                    _cbvm.FilesToImport.ExcludedItems.Add(new() { Path = file.Path });
                    break;
            }
        }

        public void cleanTime()
        {
            _cbvm.CloudAccounts.Clear();
        }

        public async Task GetAllNextCloudAccountsAsync()
        {

            try
            {
                var response = await _ws.GetAllNextCloudAccountsAsync();

                var account = JsonSerializer.Deserialize<List<NextCloudAccountInfo>>(response);
                Console.WriteLine(_cbvm.CloudAccounts.Count);
                foreach(var i in account)
                {
                    if (!_cbvm.CloudAccounts.Any(x => x is NextCloudAccountInfo nci && nci.AccountId.Equals(i.AccountId)))
                    {
                        i.Url = StripUrlPrefix(i.Url);
                        _cbvm.CloudAccounts.Add(i);

                    }
                    Console.WriteLine(_cbvm.CloudAccounts.Count);

                }
                return;

            }
            catch (Exception ex)
            {
                //(ex);
            }
        }
        string StripUrlPrefix(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            return Regex.Replace(input.Trim(),
                @"^(?:https?:\/\/)?(?:www\.)?",
                "",
                RegexOptions.IgnoreCase);
        }
        public async Task<string> PostNextcloudAccountAsync(NextCloudAccountRegistration data)
        {
            return await _ws.PostNextcloudAccountAsync(data);
        }


        public async Task<string> PostSelectedItemsAsync()
        {
            string result = await _ws.PostSelectedItemsAsync(_cbvm.FilesToImport, NextCloudAccountId);
            if (result.Equals("OK"))
            {
                _cbvm.FilesToImport.ExcludedItems.Clear();
                _cbvm.FilesToImport.IncludedItems.Clear();
            }
            return result;

        }
        public async Task<string> StartNextCloudSyncAsync(string accountId)
        {
            var result = await _ws.StartNextCloudSyncAsync(accountId);

            return result switch
            {
                "{\"status\":\"start syncing\"}" => "OK",
                null or "" => "Error" ,
                "Unauthorized" =>  "Unauthorized",
                string re when CloudBaseViewModel.TryDeserialize(result, out var status) => status,
                _ => "Error"
            };
        }
        
        public async Task<string> GetSelectedItemsAsync(string accountId)
        {
            return await _ws.GetSelectedItemsAsync(accountId);
        }

        public async Task<string> FetchNextcloudDirectoryAsync(string accountId, string folder)
        {
            var result =  await _ws.GetNextcloudFolderAsync(accountId, folder);
            //(result);
            if (!string.IsNullOrEmpty(result) || !result.Equals("Unauthorized"))
            {
                NextCloudDirectories = JsonSerializer.Deserialize<List<NextCloudFile>>(result);
            }
            if (NextCloudDirectories.Any())
            {
                TrackuserSelectedItems();

            }

            return result;

        }
        public void ChangeAllitemStateNextCloud(bool? newState)
        {
            foreach(var item in NextCloudDirectories)
            {
                item.CheckBoxState = newState;
                if(newState == true)
                {
                    if (_cbvm.ExcludedPathsHashSet.Any(x => x.Path.Equals(item.Path,StringComparison.OrdinalIgnoreCase)))
                    {
                        _cbvm.FilesToImport.ExcludedItems.RemoveAll(x => x.Path.Equals(item.Path));

                    }
                    _cbvm.FilesToImport.IncludedItems.Add(new CloudFileImport
                    {
                            Path = item.Path
                    });
                }
                if (newState == false)
                {
                    if (_cbvm.IncludedPathsHashSet.Any(x => x.Path.Equals(item.Path, StringComparison.OrdinalIgnoreCase)))
                    {
                        _cbvm.FilesToImport.IncludedItems.RemoveAll(x => x.Path.Equals(item.Path));

                    }
                    _cbvm.FilesToImport.ExcludedItems.Add(new CloudFileImport
                    {
                        Path = item.Path
                    });
                    
                }
            }
        }

        private void TrackuserSelectedItems()
        {
            foreach(var item in NextCloudDirectories)
            {
                if (_cbvm.IncludedPathsHashSet.Any(x => x.Path.Equals(item.Path, StringComparison.OrdinalIgnoreCase)))
                {
                    item.CheckBoxState = true;
                }
                else if (_cbvm.ExcludedPathsHashSet.Any(x => x.Path.Equals(item.Path, StringComparison.OrdinalIgnoreCase)))
                {
                    item.CheckBoxState = false;
                }
            }
        }
        public async Task<string> DeleteNextCloudAccountAsync(string accountId)
        {
            string result = await _ws.DeleteNextCloudAccountAsync(accountId);
            if (result.Equals("OK"))
            {
                var itemToRemove = _cbvm.CloudAccounts.FirstOrDefault(x => x is NextCloudAccountInfo nci && nci.AccountId.Equals(accountId));
                _cbvm.CloudAccounts?.Remove(itemToRemove);

            }
            return result;
        }
    }
}
