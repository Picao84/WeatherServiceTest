using Microsoft.Extensions.Logging;
using WeatherServiceTest.Services;
using WeatherServiceTest.Services.Interfaces;
using WeatherServiceTest.Services.PartialMethods;
using WeatherServiceTest.ViewModels;

namespace WeatherServiceTest;

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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IWeatherAPIService, WeatherAPIService>();
        builder.Services.AddSingleton<PostNotificationService>();
        builder.Services.AddSingleton<GeoLocationService>();
        builder.Services.AddSingleton<BackgroundServiceManager>();
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<MainPage>();
        

        return builder.Build();
    }
}