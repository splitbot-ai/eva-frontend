using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaComponentLibrary.Services;

namespace EvaMobil.Services
{
    public class IOpenLink : IOpenLinkInBrowser
    {
        public async void OpenLink(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
