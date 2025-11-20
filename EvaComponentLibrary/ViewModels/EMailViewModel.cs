using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EvaComponentLibrary.Models.Mail;
using EvaComponentLibrary.Services;
namespace EvaComponentLibrary.ViewModels
{
    public class EMailViewModel
    {
        public WebServices ws;
        public ObservableCollection<Email> EmailList = new ObservableCollection<Email>();
        public string GoogleOauth { get; set; } = string.Empty;
        public async Task<bool> GetMailAccounts()
        {
            var result = await ws.GetMailAccountsAsync();
            EmailList.Clear();  
            if (!string.IsNullOrEmpty(result))
            {
                var content = JsonSerializer.Deserialize<List<Email>>(result);
                EmailList.Clear();
                foreach (var mail in content)
                {
                    EmailList.Add(mail);
                }
                return true;
            }
            return false;
        }

        public async Task<bool> SetCrawlRetention(string id, int retention)
        {
            var result = await ws.SetCrawlRetentionAsync(id,retention);
            if (result)
            {
                return true;
            }
            return false;
        }
        
        public async Task<bool> ActivateAccount(string id, bool active)
        {
            var result = await ws.ActivateAccountAsync(id,active);
            if (result)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAccount(string id) 
        {
            var result = await ws.DeleteAccountAsync(id);
            if (result)
            {
                var elemToDel = EmailList.FirstOrDefault(x => x.AccountID.Equals(id));
                if (elemToDel != null)
                    EmailList.Remove(elemToDel);
                return true;
            }
            return false;
        }
        public async Task<string> IMAPRegistration(IMAP data)
        {
            var result = await ws.IMAPRegistrationAsync(data);
            await ws.GetMailAccountsAsync();
            return result;
        }
        public async Task<bool> SetAttachment(string id, bool attachment)
        {
            var result = await ws.SetAttachment(id, attachment);
            if (result)
            {
                return true;
            }
            return false;
        }
    }
}