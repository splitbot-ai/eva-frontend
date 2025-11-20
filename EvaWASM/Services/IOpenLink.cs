using System.Net.NetworkInformation;
using EvaComponentLibrary.Services;
using Microsoft.JSInterop;

namespace EvaWASM.Services
{
    public class IOpenLink(IJSRuntime _js) : IOpenLinkInBrowser
    {
        public async void OpenLink(string url)
        {
            await _js.InvokeVoidAsync("openLinkInNewTab", url);
        }
    }
}
