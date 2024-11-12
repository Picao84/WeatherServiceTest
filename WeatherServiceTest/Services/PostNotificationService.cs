namespace WeatherServiceTest.Services.PartialMethods;

public partial class PostNotificationService
{
    public partial Task<PermissionStatus> CheckStatusAsync();

    public partial Task<PermissionStatus> RequestAsync();
}