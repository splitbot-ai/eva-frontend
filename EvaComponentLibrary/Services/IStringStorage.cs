namespace EvaComponentLibrary.Services
{
    public interface IStringStorage
    {
        Task SaveStringByKeyAsync(string key, string value);
        Task<String> GetStringByKeyAsync(string key);
        Task RemoveRefreshStringByKeyAsync(string key);
    }
}
