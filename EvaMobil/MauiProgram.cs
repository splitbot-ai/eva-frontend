using CommunityToolkit.Maui;
using EvaComponentLibrary.Services;
using EvaComponentLibrary.Models;
using EvaMobil.Services;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using System.Reflection;


using Plugin.Firebase.CloudMessaging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Extensions.Configuration;
using EvaComponentLibrary.ViewModels.Cloud;
#if IOS || ANDROID
using Firebase;

using EvaComponentLibrary.ViewModels;
using EvaComponentLibrary.ViewModels.Cloud;



#endif


#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
#elif ANDROID
using Firebase.Messaging;
using Plugin.Firebase.Core.Platforms.Android;
using EvaComponentLibrary.ViewModels;
using EvaComponentLibrary.ViewModels.Cloud;
#endif


namespace EvaMobil
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
				.RegisterFirebaseServices()
				.UseMauiCommunityToolkit()
                
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://Eva.splitbot.de/") });
            builder.Services.AddMauiBlazorWebView();
			builder.Services.AddMudServices();
            builder.Services.AddMudMarkdownServices();
            builder.Services.AddSingleton<PlatformDistinguisher>();

            builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.MainViewModel>();
            builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.LoginViewModel>();
			builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.PreloadViewModel>();
            builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.UploadManagerController>();
            builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.CustomInstViewModel>();
            builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.EMailViewModel>();
            builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.ScheduledTaskViewModel>();
            builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.MonitoringViewModel>();

            var resourceName = Assembly.GetExecutingAssembly().GetName().Name;

            //($"resourceName: {resourceName}");

            using var appsettingsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{resourceName}.appsettings.json")
                ?? throw new InvalidOperationException($"Resource {resourceName} not found.");
         
            var config = new ConfigurationBuilder().AddJsonStream(appsettingsStream!).Build();
            builder.Configuration.AddConfiguration(config);
            builder.Services.Configure<EvaComponentLibrary.Models.HostSettings>(builder.Configuration.GetSection(EvaComponentLibrary.Models.HostSettings.Settings));
            
            builder.Services.AddSingleton<EvaComponentLibrary.ViewModels.OutLookViewModel>();
            builder.Services.AddSingleton<CloudBaseViewModel>();
            builder.Services.AddSingleton<NextCloudViewModel>();
            builder.Services.AddSingleton<OneDriveViewModel>();
            builder.Services.AddSingleton<IOpenLinkInBrowser,IOpenLink>();
            builder.Services.AddSingleton<WebServices>();
            
            builder.Services.AddSingleton<Logistic>();
            builder.Services.AddScoped<MySnackBar>();
            builder.Services.AddSingleton<IThreadInvoker, MainThreadInvoker>();

			builder.Services.AddSingleton<IStringStorage, MauiSafeStringManager>();
            builder.Services.AddSingleton<IThemeService, SafeAreaThemeManager>();
            builder.Services.AddSingleton<IClipboardService, ClipboardService>();
            builder.Services.AddSingleton<IDownloadPDF, DownloadPDF>();

            builder.Services.AddSingleton<IPushNotificationService, PushNotificationService>();

            builder.Services.AddLocalization();
            builder.Services.AddSingleton(typeof(SaveStringServices));
#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

		private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
		{
			//builder.Services.AddSingleton(CrossFirebaseCloudMessaging.Current);
			//builder.Services.AddSingleton<FcmTokenService>();

			builder.ConfigureLifecycleEvents(events =>
			{
#if IOS
                events.AddiOS(iOS => iOS.WillFinishLaunching((_, __) =>
                {
                    CrossFirebase.Initialize();
                    FirebaseCloudMessagingImplementation.Initialize();
                    
                    return true;
                }));
#elif ANDROID
                events.AddAndroid(android => android.OnCreate((activity, _) =>
                {
                    // Explicit initialization of Firebase
                    var context = activity.ApplicationContext;
                    var app = FirebaseApp.InitializeApp(context);

                    if (app == null)
                    {
                        Android.Util.Log.Error("FirebaseInit", "FirebaseApp.InitializeApp returned null");
                    }

                    // Now initialize the plugin
                    CrossFirebase.Initialize(activity);
                }));
#endif
            });

			return builder;
		}

	}
}
