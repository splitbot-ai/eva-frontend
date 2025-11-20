namespace EvaComponentLibrary.Services
{
    public class SaveStringServices
    {
        private readonly IStringStorage _storage;

        public SaveStringServices(IStringStorage storage)
            => _storage = storage;

        public Task<string> GetStringByKeyAsync(string key) 
            => _storage.GetStringByKeyAsync(key);


        public Task RemoveRefreshStringByKeyAsync(string key)
            => _storage.RemoveRefreshStringByKeyAsync(key);

        public Task SaveStringByKeyAsync(string key, string value)
            => _storage.SaveStringByKeyAsync(key, value);
        
    }
}
