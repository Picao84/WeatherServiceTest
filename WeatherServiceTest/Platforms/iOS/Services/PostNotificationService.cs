using UIKit;
using UserNotifications;

namespace WeatherServiceTest.Services.PartialMethods;

public partial class PostNotificationService
{
    public partial Task<PermissionStatus> CheckStatusAsync()
    {
        try
        {
            var settings = UIApplication.SharedApplication.CurrentUserNotificationSettings.Types;
            return Task.FromResult(settings != UIUserNotificationType.None ? PermissionStatus.Granted : PermissionStatus.Denied);

        }catch(Exception ex)
        {           
          
            return Task.FromResult(PermissionStatus.Unknown);
        }
    }
    
    public partial async Task<PermissionStatus> RequestAsync()
    {
        var result = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge);

        if(result.Item2 != null)
        {
            return result.Item1 ? PermissionStatus.Granted : PermissionStatus.Denied;
        }

        return PermissionStatus.Unknown;
    }
    
}