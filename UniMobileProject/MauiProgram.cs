using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using UniMobileProject.src.Services.Database;
using UniMobileProject.src.Services.Http;
using UniMobileProject.src.Services.Serialization;

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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddScoped<IHttpServiceFactory, HttpServiceFactory>();
            builder.Services.AddScoped<ISerializationFactory, SerializationFactory>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
