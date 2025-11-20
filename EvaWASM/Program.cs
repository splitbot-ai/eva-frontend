using EvaComponentLibrary.Services;
using EvaComponentLibrary.ViewModels.Cloud;
using EvaWASM;
using EvaWASM.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton<PlatformDistinguisher>();

builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.MainViewModel>();
builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.LoginViewModel>();	
builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.PreloadViewModel>();
builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.UploadManagerController>();
builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.CustomInstViewModel>();
builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.EMailViewModel>();
builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.ScheduledTaskViewModel>();
builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.OutLookViewModel>();
builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.MonitoringViewModel>();
builder.Services.AddSingleton<NextCloudViewModel>();
builder.Services.AddSingleton<CloudBaseViewModel>();
builder.Services.AddSingleton<OneDriveViewModel> ();
builder.Services.AddSingleton<IOpenLinkInBrowser,IOpenLink>();
builder.Services.AddSingleton<EvaComponentLibrary.Services.IPushNotificationService, PushNotificationService>();	

builder.Services.Configure<EvaComponentLibrary.Models.HostSettings>(
    builder.Configuration.GetSection(EvaComponentLibrary.Models.HostSettings.Settings));
builder.Services.AddSingleton<WebServices>();
builder.Services.AddScoped<MySnackBar>();
builder.Services.AddSingleton<Logistic>();
builder.Services.AddScoped<MySnackBar>();
builder.Services.AddSingleton<IThreadInvoker, MainThreadInvokerWeb>();
builder.Services.AddSingleton<IDownloadPDF, DownloadPDF>();
builder.Services.AddMudServices();


builder.Services.AddMudMarkdownServices();
builder.Services.AddLocalization();

builder.Services.AddSingleton<IStringStorage, WSMSafeStringManager>();
builder.Services.AddSingleton<IThemeService, SafeAreaThemeManager>();
builder.Services.AddSingleton<IClipboardService, ClipboardService>();
builder.Services.AddScoped<SaveStringServices>();

//var BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
var Environment = builder.HostEnvironment.Environment;
//var test = builder.HostEnvironment.IsEnvironment().

//Console.WriteLine($"BaseAddress:{BaseAddress}");
//($"Environment:{Environment}");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


await builder.Build().RunAsync();


