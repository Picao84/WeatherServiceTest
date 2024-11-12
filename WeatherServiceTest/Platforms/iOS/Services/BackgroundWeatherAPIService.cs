using System.Text.Json;
using System.Text.Json.Nodes;
using Foundation;
using MapKit;
using UIKit;
using WeatherServiceTest.Services.Interfaces;
using WeatherServiceTest.Models;

namespace WeatherServiceTest.Services;

public class BackgroundWeatherAPIService : NSObject, INSUrlSessionDownloadDelegate
{
    public void DidFinishDownloading(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
    {
        if (location != null)
        {
            var stringResponse = NSData.FromUrl(location);
            
            var weatherData = JsonSerializer.Deserialize<WeatherData>(stringResponse.AsStream());
        
            NSUserDefaults.StandardUserDefaults.SetString(JsonSerializer.Serialize(weatherData), Constants.NotificationsSettingKey);
            
            DisplayWeatherNotification(weatherData);
        }
    }

    public void FetchWeather(double latitude, double longitude)
    {
        try
        {
            var config = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration("weatherTask");
            
            using (NSUrlSession client = NSUrlSession.FromConfiguration(config, this, NSOperationQueue.CurrentQueue))
            {
                var request = client.CreateDownloadTask(new NSUrl(
                    $"{Constants.WeatherApiUrl}lat={latitude}&lon={longitude}&&appid={Constants.OpenWeatherApiKey}"));
            }

        }
        catch (Exception ex)
        {
            
        }
        
    }
    
    private static void DisplayWeatherNotification(WeatherData weatherData)
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UILocalNotification weatherNotification = new UILocalNotification();
                weatherNotification.FireDate = NSDate.Now;
                weatherNotification.AlertAction = "Weather Update!";
                weatherNotification.AlertBody = $"Temperature now is {weatherData.Current.Temperature} and feels like {weatherData.Current.FeelsLike}";
                UIApplication.SharedApplication.ScheduleLocalNotification(weatherNotification);
            });
        }
        catch(Exception ex)
        {

        }
    }
}