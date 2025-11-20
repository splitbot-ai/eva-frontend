using System.Net.NetworkInformation;
using EvaComponentLibrary.Services;
using Microsoft.JSInterop;


namespace EvaWASM.Services
{
    public class DownloadPDF(IJSRuntime js) : IDownloadPDF
    {
        
        public Task<bool> Save(byte[] pdfByte, string fileName)
        {
            try
            {
                js.InvokeVoidAsync("DownloadPDF").ConfigureAwait(false);
                return Task.FromResult(true);

            }
            catch (Exception e)
            { 
               //(e);
                return Task.FromResult(false);
            }
        }
    }
}
