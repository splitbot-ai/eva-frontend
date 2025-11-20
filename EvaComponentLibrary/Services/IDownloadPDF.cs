using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Services
{
    public interface IDownloadPDF
    {
        Task<bool> Save(byte[] pdfByte, string fileName);
    }
}
