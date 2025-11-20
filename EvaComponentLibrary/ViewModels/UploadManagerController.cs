using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.RegularExpressions;
using EvaComponentLibrary.Models;
using EvaComponentLibrary.Services;
using Microsoft.AspNetCore.Components.Forms;
using static MudBlazor.CategoryTypes;

namespace EvaComponentLibrary.ViewModels
{
    public class UploadManagerController
    {
        private MainViewModel _mainViewModel;

        public MainViewModel MainViewModel { get { return _mainViewModel; } set { _mainViewModel = value; } }
        public Dictionary<string, FileUploadInfo> Files { get; set; } = new Dictionary<string,FileUploadInfo>();
        public ObservableCollection<FileContainer> FilesContainer { get; set; } = new ObservableCollection<FileContainer>();

        public List<FileUploadResponse> FilesHelper { get; set; }
        public byte[] PDFBase64 { get; set; }
        public string PDFName { get; set; } = string.Empty;
        public int PageNum { get; set; } = 1;
        public User UserInfo { get; set; } = new User();
        
        public event Action? UpdateUi;

        public event Action<string>? FileUpload;

        public event Action<bool>? IsUploading;
        public int FilesToUpload = 0;

        public event Action cleaningTimeUploadManagerController;
		public readonly WebServices _ws;
		public readonly MainViewModel _vm;

        public void cleanTime()
        {
            Files.Clear();
            FilesContainer.Clear();

        }
		public UploadManagerController(WebServices service, MainViewModel mainViewModel)
		{
			_ws = service ?? throw new ArgumentNullException(nameof(service));
			_vm = mainViewModel ?? throw new ArgumentNullException(nameof(service));
		}
		public async Task InitFiles()
        {
            await GetAllUploadedFiles().ConfigureAwait(false);
        }

        public async Task Upload(Dictionary<string, FileUploadInfo> files)
        {
            Timer? timer = null;
            try
            {

                const int TickIntervalMs = 100;
                const double KilobytesPerSecond = 150; // slower simulated speed
                const double BytesPerTick =
                    KilobytesPerSecond * 1024 * (TickIntervalMs / 1000d);

                double biggestFileBytes = files.Values.Max(f => f.Size);
                double ticksNeeded = Math.Ceiling(biggestFileBytes * 0.9 / BytesPerTick);
                double totalMs = ticksNeeded * TickIntervalMs;

                var rand = new Random();
                timer = new Timer(_ =>
                {
                    var uiNeedsRefresh = false;

                    foreach (var file in files.Values)
                    {
                        var projectedTotal = file.UploadedBytes + (BytesPerTick * (0.1 + rand.NextDouble() * 0.4));

                        file.UploadedBytes = Math.Min((file.Size * 0.9), projectedTotal);
                        uiNeedsRefresh = true;
                    }
                    if (uiNeedsRefresh)
                        UpdateUi?.Invoke();

                }, state: null, dueTime: 0, period: TickIntervalMs);

                string result = await _ws.UploadAsync(files).ConfigureAwait(false);
                FilesHelper = JsonSerializer.Deserialize<List<FileUploadResponse>>(result);


                int total = FilesHelper.Count;
                int falseCount = FilesHelper.Count(f => !f.Success);
                int correctCount = total - falseCount;
                string message = $"{correctCount}/{total}";

                foreach (var elem in FilesHelper)
                {
                    Files[elem.FileName].Success = elem.Success;
                    Files[elem.FileName].IsSending = false;
                    Files[elem.FileName].UploadedBytes = Files[elem.FileName].Size;                    
                }

                timer?.Dispose();
                UpdateUi?.Invoke();
                await Task.Delay(500);
                Files = Files.Where(x => x.Value.Success.Equals(false)).ToDictionary(x => x.Key, x => x.Value);
                FilesToUpload = Files.Count();
                UpdateUi?.Invoke();
                FileUpload?.Invoke(message);
                IsUploading?.Invoke(FilesToUpload == 0);
            }catch(Exception ex)
            {

            }

        }

        //---------------------------------------------------------- temprol ---------------------------------------------------------
        public static IEnumerable<FileUploadResponse> ParseLines(IEnumerable<string> lines)
        {
            foreach (var raw in lines)
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;

                var parts = raw.Split(':', 2, StringSplitOptions.TrimEntries);
                if (parts.Length != 2) continue;

                var name = parts[0].Trim().Trim('"');  
                var isSucc = parts[1].Trim().Trim('"'); 

                name = Regex.Replace(name, @"\s+\.pdf\b", ".pdf", RegexOptions.IgnoreCase);

                if (bool.TryParse(isSucc, out var succ))
                    yield return new FileUploadResponse() { 
                        FileName = name,
                        Success = succ
                    };
            }

        }
        //---------------------------------------------------------- temprol ---------------------------------------------------------


        public async Task GetAllUploadedFiles()
        {
            var stringContent = await _ws.GetAllFilesAsync();
            List<FileContainer> files = JsonSerializer.Deserialize<List<FileContainer>>(stringContent);

            if (files.Count != 0)
            {
                FilesContainer = new ObservableCollection<FileContainer>(files.Reverse<FileContainer>());
                UpdateUi?.Invoke();
            }
            UpdateUi?.Invoke();
        }

        public async Task<bool> DeleteFileAsync(string id)
        {
            var result = await _ws.DeleteFilesAsync(id,_vm.Role).ConfigureAwait(false);
            if(result)
            {
                var obj = FilesContainer.FirstOrDefault(e => e.Id.Equals(id));
                if(obj != null)
                {
                    FilesContainer.Remove(obj);
                    _mainViewModel.DeleteFileSource(id);
                    UpdateUi?.Invoke();
                }
                return true;
            }
            return false;

        }

        public async Task<User> GetUserInfoAsync()
        {
            var result = await _ws.GetUserInfoAsync();
            if(!string.IsNullOrEmpty(result))
            {
                var user = JsonSerializer.Deserialize<User>(result);
                return user;
            }
            return null;
        }

        public async Task<bool> GetPDFBase64(string id)
        {
            var result = await _ws.GetPDFBase64(id);
            if (result.Any())
            {
                PDFBase64 = result;
                return true  ;
            }
            return false;
        }

        public List<string> ChunksNormalizer(List<string> chunk)
        {
            var CleanChunks = chunk
                .SelectMany(s => s.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Trim())
                    .Where(line => !string.IsNullOrEmpty(line) && line.Length > 4))
                .ToList();
            return CleanChunks;
        }

        public async Task<bool> PostFileWithAnnotation(string id, List<string> chunks)
        {
            var contentPDF = await _ws.PostFileWithAnnotation(id, chunks);
            //(contentPDF);
            if(!string.IsNullOrEmpty(contentPDF))
            {
                var PDF = JsonSerializer.Deserialize<FileWithAnnotations>(contentPDF);
                PDFBase64 = Convert.FromBase64String(PDF.Base64);
                PageNum = PDF.FirstPage;
                PDFName = PDF.FileName;

                return true;
            }
            return false;

        }
    }
}
