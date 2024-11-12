using BackgroundTasks;
using CoreFoundation;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;
using WeatherServiceTest.Models;
using WeatherServiceTest.Services.iOS;

namespace WeatherServiceTest.Services.PartialMethods;

public partial class BackgroundServiceManager
{
    public partial void EnableNotifications()
    {
        WeatherNotificationService.ScheduleWeatherRefresh();
        SetEnergySavingConditions();
    }

    public partial void DisableNotifications()
    {
        WeatherNotificationService.StopWeatherNotification();
        StopEnergySavingConditions();
    }

    public BackgroundServiceManager()
    {
        SetEnergySavingConditions();
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
           WeatherNotificationService.ScheduleWeatherRefresh();
        }
        else
        {
          WeatherNotificationService.StopWeatherNotification();
        }
    }

    private void BatteryOnBatteryInfoChanged(object? sender, BatteryInfoChangedEventArgs e)
    {
        if (e.ChargeLevel < 0.2f && e.State != BatteryState.Charging)
        {
            WeatherNotificationService.ScheduleWeatherRefresh();
        }
        else
        {
           WeatherNotificationService.StopWeatherNotification();
        }
    }

    private void ConnectivityOnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess != NetworkAccess.Internet)
        {
            WeatherNotificationService.ScheduleWeatherRefresh();
        }
        else
        {
           WeatherNotificationService.StopWeatherNotification();
        }
    }

    
}