using EvaComponentLibrary.Services;

namespace EvaMobil.Services
{
    public class MauiSafeStringManager : IStringStorage
    {
        public async Task<string> GetStringByKeyAsync(string key)
           => await SecureStorage.GetAsync(key) ?? string.Empty;

        public async Task RemoveRefreshStringByKeyAsync(string key)
            => SecureStorage.Remove(key);

        public async Task SaveStringByKeyAsync(string key, string value)
            => await SecureStorage.SetAsync(key, value);
    }
}
