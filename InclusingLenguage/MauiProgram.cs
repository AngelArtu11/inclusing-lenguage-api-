using Microsoft.Extensions.Logging;
using InclusingLenguage._04_Services;

namespace InclusingLenguage
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrar servicios
            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddSingleton<IMongoDBService, MongoDBService>();
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
            builder.Services.AddSingleton<ILessonService, LessonService>();
            builder.Services.AddSingleton<IProfileService, ProfileService>();

            // Registrar helpers y factories
            builder.Services.AddSingleton<_05_Helpers.LessonPageFactory>();

            // Registrar páginas para navegación con inyección de dependencias
            builder.Services.AddTransient<_03_Views.LoginPage>();
            builder.Services.AddTransient<_03_Views.RegisterPage>();
            builder.Services.AddTransient<_03_Views.HomePage>();
            builder.Services.AddTransient<_03_Views.ProfilePage>();
            builder.Services.AddTransient<_03_Views.LessonPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
