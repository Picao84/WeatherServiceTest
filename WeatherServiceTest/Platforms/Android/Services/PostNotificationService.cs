using Android.Content;
using WeatherServiceTest.Permissions;

namespace WeatherServiceTest.Services.PartialMethods;

public partial class PostNotificationService
{
    private const string notificationService = Context.NotificationService;

    public partial async Task<PermissionStatus> CheckStatusAsync()
    {
        return await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<PostNotificationPermission>();
    }
    
    public partial async Task<PermissionStatus> RequestAsync()
    {
        return await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<PostNotificationPermission>();
    }
}