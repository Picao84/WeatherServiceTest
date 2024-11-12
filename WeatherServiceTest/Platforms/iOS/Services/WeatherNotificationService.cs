using System.Text.Json;
using BackgroundTasks;
using Foundation;
using UIKit;
using UserNotifications;
using WeatherServiceTest.Models;

namespace WeatherServiceTest.Services.iOS;

public static class WeatherNotificationService
{
    public static async Task CheckWeather()
    {
        try
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                Tuple<double, double> location = await App.GeoLocationService.GetCurrentLocation();

                if (location != null)
                {
                    BackgroundWeatherAPIService backgroundWeatherApiService = new BackgroundWeatherAPIService();
                    backgroundWeatherApiService.FetchWeather(location.Item1, location.Item2);
                }
                
            }
        }
        catch (Exception ex)
        {

        }

    }
    
    public static void StopWeatherNotification()
    {
        BGTaskScheduler.Shared.Cancel("com.tiagoInteractive.weathertestapp-updateweather");
    }
    
    public static void ScheduleWeatherRefresh()
    {
        var request = new BGAppRefreshTaskRequest("com.tiagoInteractive.weathertestapp-updateweather");
        request.EarliestBeginDate = NSCalendar.CurrentCalendar.DateByAddingUnit(NSCalendarUnit.Minute, (nint) Constants.MINIMUM_FETCH_TIME.TotalMinutes, NSDate.Now, NSCalendarOptions.MatchNextTime);

        try
        {
            NSError error;
            BGTaskScheduler.Shared.Submit(request, out error);
        }
        catch (Exception ex)
        {
            
        }
    }
}