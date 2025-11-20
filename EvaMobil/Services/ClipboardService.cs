
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaComponentLibrary.Services;

namespace EvaMobil.Services
{
    public class ClipboardService : IClipboardService
    {
        public async Task CopyToClipboard(string text)
        {
            await Clipboard.Default.SetTextAsync(text);
        }

    }
}
