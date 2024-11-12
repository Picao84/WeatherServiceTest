using WeatherServiceTest.Services.Droid;

namespace WeatherServiceTest.Services.PartialMethods;

public partial class BackgroundServiceManager
{
    public partial void EnableNotifications()
    {
        Android.Content.Intent intent = new Android.Content.Intent(Android.App.Application.Context,typeof(WeatherNotificationService));
        Android.App.Application.Context.StartForegroundService(intent);
    }

    public partial void DisableNotifications()
    {
        Android.Content.Intent intent = new Android.Content.Intent(Android.App.Application.Context,typeof(WeatherNotificationService));
        Android.App.Application.Context.StopService(intent);
    }
}