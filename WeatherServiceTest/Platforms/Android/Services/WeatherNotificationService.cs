using System.Text.Json;
using System.Text.Json.Nodes;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using System.Timers;
using WeatherServiceTest.Models;
using WeatherServiceTest.Services.Interfaces;

namespace WeatherServiceTest.Services.Droid;

[Service(Name = "com.TiagoInteractive.WeatherTestApp.WeatherRefreshService")]
public class WeatherNotificationService : Service
{
    private string NOTIFICATION_CHANNEL_ID = "1000";
    private int PERSISTENT_NOTIFICATION_ID = 1;
    private int UPDATE_NOTIFICATION_ID = 2;
    private string NOTIFICATION_CHANNEL_NAME = "weather";
    private System.Timers.Timer nextCheck = new System.Timers.Timer(Constants.MINIMUM_FETCH_TIME.TotalMilliseconds);
    private const string CHECKING_WEATHER_UPDATES = "Checking for weather updates.";
    private const string NOT_CHECKING_NO_INTERNET = "Impossible to check for weather updates without internet.";
    private const string NOT_CHECKING_LOW_BATTERY = "Not checking for weather updates due to low battery.";
    private const string NOT_CHECKING_NO_LOCATION_PERMISSION = "Not checking for weather updates because background location is not permitted. Please check app permissions.";

    public override IBinder? OnBind(Intent? intent)
    {
        return null;
    }

    public override void OnDestroy()
    {
        nextCheck.Stop();
        StopEnergySavingConditions();
        
        base.OnDestroy();
    }
    
    private void DisplayWeatherNotification(WeatherData weatherData)
    {
        var notificationBuilder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);
        notificationBuilder.SetAutoCancel(true);
        notificationBuilder.SetSmallIcon(Resource.Mipmap.appicon);
        notificationBuilder.SetContentTitle("Weather Update");
        notificationBuilder.SetContentText($"Temperature is now {weatherData.Current.Temperature}, feels like {weatherData.Current.FeelsLike} and weather looks like {weatherData.Current.Weather}.");
        
        var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

        notificationManager?.Notify(UPDATE_NOTIFICATION_ID, notificationBuilder.Build());
    }

    private void ShowPersistentNotification(string message)
    {
        var notificationBuilder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);
        notificationBuilder.SetAutoCancel(false);
        notificationBuilder.SetOngoing(true);
        notificationBuilder.SetSmallIcon(Resource.Mipmap.appicon);
        notificationBuilder.SetContentTitle("Weather App");
        notificationBuilder.SetContentText(message);
        notificationBuilder.SetForegroundServiceBehavior(NotificationCompat.ForegroundServiceImmediate);
        
        var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

        notificationManager?.Notify(PERSISTENT_NOTIFICATION_ID, notificationBuilder.Build());
    }
    
    private void SetTimer()
    {
        nextCheck.Elapsed += async (sender, args) =>
        {
            await CheckWeather();
        };
        
        nextCheck.AutoReset = false;
        nextCheck.Start();
    }

    private void SetEnergySavingConditions()
    {
        Connectivity.ConnectivityChanged += ConnectivityOnConnectivityChanged;
        Battery.BatteryInfoChanged += BatteryOnBatteryInfoChanged;
        Battery.EnergySaverStatusChanged += EnergySaverStatusChanged;
    }
    
    private void StopEnergySavingConditions()
    {
        Connectivity.ConnectivityChanged -= ConnectivityOnConnectivityChanged;
        Battery.BatteryInfoChanged -= BatteryOnBatteryInfoChanged;
        Battery.EnergySaverStatusChanged -= EnergySaverStatusChanged;
    }

    private void EnergySaverStatusChanged(object? sender, EnergySaverStatusChangedEventArgs e)
    {
        if (e.EnergySaverStatus == EnergySaverStatus.On)
        {
            nextCheck.Stop();
            ShowPersistentNotification("Not checking for weather updates due to low battery.");
        }
        else
        {
            nextCheck.Start();
            ShowPersistentNotification(CHECKING_WEATHER_UPDATES);
        }
    }

    private void BatteryOnBatteryInfoChanged(object? sender, BatteryInfoChangedEventArgs e)
    {
        if (e.ChargeLevel < 0.2f && e.State != BatteryState.Charging)
        {
            nextCheck.Stop();
            ShowPersistentNotification(NOT_CHECKING_LOW_BATTERY);
        }
        else
        {
            nextCheck.Start();
            ShowPersistentNotification(CHECKING_WEATHER_UPDATES);
        }
    }

    private void ConnectivityOnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess != NetworkAccess.Internet)
        {
            nextCheck.Stop();
            ShowPersistentNotification(NOT_CHECKING_NO_INTERNET);
        }
        else
        {
            nextCheck.Start();
            ShowPersistentNotification(CHECKING_WEATHER_UPDATES);
        }
    }

    private void StartWeatherService()
    {
        var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            CreateWeatherNotificationChannel(notificationManager);
        }

        var notificationBuilder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);
        notificationBuilder.SetAutoCancel(false);
        notificationBuilder.SetOngoing(true);
        notificationBuilder.SetSmallIcon(Resource.Mipmap.appicon);
        notificationBuilder.SetContentTitle("Weather App");
        notificationBuilder.SetContentText(Connectivity.NetworkAccess == NetworkAccess.Internet ? "Checking for weather updates." : "Impossible to check for weather updates without internet.");
        notificationBuilder.SetForegroundServiceBehavior(NotificationCompat.ForegroundServiceImmediate);
        
        if (Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
        {
            StartForeground(PERSISTENT_NOTIFICATION_ID, notificationBuilder.Build());
        } else
        {
            StartForeground(PERSISTENT_NOTIFICATION_ID, notificationBuilder.Build(), ForegroundService.TypeDataSync);
        }

        SetTimer();
        SetEnergySavingConditions();
    }

    private async Task CheckWeather()
    {
        try
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                //GeoLocationService geolocationService = new GeoLocationService();
                Tuple<double, double> location = await App.GeoLocationService.GetCurrentLocation();

                if (location != null)
                {
                    ShowPersistentNotification(CHECKING_WEATHER_UPDATES);
                    
                    //WeatherAPIService weatherService = new WeatherAPIService();
                    WeatherData weatherData = await App.WeatherAPIService.FetchWeather(location.Item1, location.Item2);

                    if (weatherData != null)
                    {
                        DisplayWeatherNotification(weatherData);
                        Preferences.Set(Constants.LastWeatherUpdateKey, JsonSerializer.Serialize(weatherData));
                    }
                }
                else
                {
                    ShowPersistentNotification(NOT_CHECKING_NO_LOCATION_PERMISSION);
                }
            }
        }
        catch (Exception ex)
        {

        }

        finally
        {
            nextCheck.Start();
        }
    }
    
    private void CreateWeatherNotificationChannel(NotificationManager notificationMnaManager)
    {
        var channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, NOTIFICATION_CHANNEL_NAME,
            NotificationImportance.Low);
        notificationMnaManager.CreateNotificationChannel(channel);
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        StartWeatherService();
        return StartCommandResult.NotSticky;
    }
}