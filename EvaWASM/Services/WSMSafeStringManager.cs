using EvaComponentLibrary.Services;
using Microsoft.JSInterop;

namespace EvaWASM.Services
{
    public class WSMSafeStringManager : IStringStorage
    {


        private readonly IJSRuntime _jsRuntime;

        public WSMSafeStringManager(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        public async Task<string> GetStringByKeyAsync(string key)
            => await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

        public async Task RemoveRefreshStringByKeyAsync(string key)
            => await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);

        public async Task SaveStringByKeyAsync(string key, string value) =>
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
    }
}
