using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Services
{
    public interface IClipboardService
    {
        Task CopyToClipboard(string text);
    }
}
