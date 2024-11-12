using BackgroundTasks;
using CoreFoundation;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;
using UserNotifications;
using WeatherServiceTest.Models;
using WeatherServiceTest.Services;
using WeatherServiceTest.Services.Interfaces;
using WeatherServiceTest.Services.iOS;
using WeatherServiceTest.Services.PartialMethods;

namespace WeatherServiceTest;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        BGTaskScheduler.Shared.Register("com.tiagoInteractive.weathertestapp-updateweather", null, (task) =>
        {
            HandleWeatherRefresh(task as BGAppRefreshTask);
        });
        
        if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))  
        {  
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) => {  
            });  
            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate(); //set delegate    
        } 
        
        return base.FinishedLaunching(application, launchOptions);
    }


    public override void DidEnterBackground(UIApplication application)
    {
        base.DidEnterBackground(application);
        
        WeatherNotificationService.ScheduleWeatherRefresh();
    }

    private void HandleWeatherRefresh(BGAppRefreshTask task)
    {
        if (Preferences.Get(Constants.NotificationsSettingKey, false) == false)
            return;

        WeatherNotificationService.ScheduleWeatherRefresh();

        var operation = new WeatherRefresh();
        
        task.ExpirationHandler = operation.Cancel;
        
        operation.CompletionBlock = () =>
        {
            WeatherNotificationService.CheckWeather();

            task.SetTaskCompleted(!operation.IsCancelled);
        };
        
        NSOperationQueue.MainQueue.AddOperation(operation);

    }

   
}

public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
{   
    [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]// pay attention to this line
    public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
    {
        completionHandler(UNNotificationPresentationOptions.Banner);
    }
}

public class WeatherRefresh : NSOperation
{
    
}

