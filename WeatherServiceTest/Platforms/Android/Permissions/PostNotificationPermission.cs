namespace WeatherServiceTest.Permissions;

public class PostNotificationPermission : Microsoft.Maui.ApplicationModel.Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>()
    {
        (Android.Manifest.Permission.PostNotifications, true),

    }.ToArray();
}