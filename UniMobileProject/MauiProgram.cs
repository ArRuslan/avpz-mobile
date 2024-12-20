using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using UniMobileProject.src.Services.Database;
using ZXing.Net.Maui;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Deserialization;
using ZXing.Net.Maui.Controls;
using Camera.MAUI;

namespace UniMobileProject
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            //var getAssembly = Assembly.GetExecutingAssembly();
            //using var stream = getAssembly.GetManifestResourceStream("UniMobileProject.appsettings.json");

            //var config = new ConfigurationBuilder().AddJsonStream(stream).Build();

            //builder.Configuration.AddConfiguration(config);

            builder
                .UseMauiApp<App>()
                .UseMauiCameraView()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.UseBarcodeReader();

            builder.Services.AddScoped<IHttpServiceFactory, HttpServiceFactory>();
            builder.Services.AddScoped<IDeserializationFactory, DeserializationFactory>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
