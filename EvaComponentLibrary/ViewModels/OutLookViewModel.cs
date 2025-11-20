using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EvaComponentLibrary.Models;
using EvaComponentLibrary.Models.OutLook;
using EvaComponentLibrary.Services;


namespace EvaComponentLibrary.ViewModels
{
    public class OutLookViewModel(WebServices _ws)
    {
        public OutLookAccount Account { get; set; } = new();
        public async Task<string> GetKalenderAuth()
        {
            var result = await _ws.GetKalenderAuth();

            return result switch
            {
                null or "" => "Error",
                "NotFound" => "NotFound",
                "Unauthorized" => "Unauthorized",
                _ => TrySetAccount(result) ? "OK" : "Error"
            };
        }

        bool TrySetAccount(string json)
        {
            try
            {
                Account = JsonSerializer.Deserialize<OutLookAccount>(json)
                            ?? throw new JsonException("Deserialized null account.");
                return true;
            }
            catch (JsonException)
            {
                // log if you want
                return false;
            }
        }


        public async Task<string> PostAuthOutLook(OutLookAuth data)
        {
            var result = await _ws.PostAuthOutLook(data);
            await GetKalenderAuth();
            return result;
        }

        public async Task<bool> DeleteKalenderAuth()
        {
            var result = await _ws.DeleteKalenderAuth();
            if (result)
            {
                Account.Email = string.Empty;
                return true;
            }
            return false;
        }
    }
}
