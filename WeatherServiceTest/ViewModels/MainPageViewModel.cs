using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Input;

using WeatherServiceTest.Models;
using WeatherServiceTest.Models.Abstract;
using WeatherServiceTest.Services;
using WeatherServiceTest.Services.Interfaces;
using WeatherServiceTest.Services.PartialMethods;
using WeatherServiceTest.Utils;
using WeatherServiceTest.ViewModels.Abstract;

namespace WeatherServiceTest.ViewModels;

public class MainPageViewModel : BaseViewModel
{
    bool _notificationsEnabled; 
    float _temperature = 100;
    float _feelsLike = 100;
    private DateTime _lastUpdated;
    private string _description = "Expect Rain!";

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }
    
    public float Temperature
    {
        get => _temperature;
        set => SetProperty(ref _temperature, value);
    }
    
    public float FeelsLike
    {
        get => _feelsLike;
        set => SetProperty(ref _feelsLike, value);
    }

    public DateTime LastUpdated
    {
        get => _lastUpdated;
        set => SetProperty(ref _lastUpdated, value);
    }
    
    
    public bool NotificationsEnabled
    {
        get => _notificationsEnabled;
        set => SetProperty(ref _notificationsEnabled, value);
    }
    
    public ICommand EnableDisableCommand { get; }
    
    public MainPageViewModel(BackgroundServiceManager backgroundServiceManager, GeoLocationService geoLocationService, IWeatherAPIService weatherAPIService, PostNotificationService postNotificationService) : base(backgroundServiceManager, weatherAPIService, geoLocationService, postNotificationService)
    {
        EnableDisableCommand = new Command(ToggleNotifications);
        
        Init();

    }

    public void SubscribeToUpdates()
    {
        _weatherAPIService.OnWeatherUpdated += WeatherAPIServiceOnOnWeatherUpdated;
    }
    
    public void UnsubscribeToUpdates()
    {
        _weatherAPIService.OnWeatherUpdated -= WeatherAPIServiceOnOnWeatherUpdated;
    }

    private void WeatherAPIServiceOnOnWeatherUpdated(object? sender, WeatherUpdateArgs e)
    {
            Temperature = e.WeatherData.Current.Temperature;
            FeelsLike = e.WeatherData.Current.FeelsLike;
            Description = e.WeatherData.Current.Weather[0].Description;
            LastUpdated = UnixTimeConverter.GetDateTimeFromUnixTime(e.WeatherData.Current.DT);
    }

    public void GetLastUpdatedData()
    {
        try
        {
            var lastWeatherData = Preferences.Get(Constants.LastWeatherUpdateKey, string.Empty);
            if (!string.IsNullOrEmpty(lastWeatherData))
            {
                var weatherData = JsonSerializer.Deserialize<WeatherData>(lastWeatherData);
                Temperature = weatherData.Current.Temperature;
                FeelsLike = weatherData.Current.FeelsLike;
                Description = weatherData.Current.Weather[0].Description;
                LastUpdated = UnixTimeConverter.GetDateTimeFromUnixTime(weatherData.Current.DT);
            }
        }
        catch (Exception ex)
        {
            
        }
    }

    private async void Init()
    {
        NotificationsEnabled = Preferences.Get(Constants.NotificationsSettingKey, false);
        await CheckPermissions();
        await UpdateWeather();
    }

    private async Task CheckPermissions()
    {
        if (await PostNotificationService.CheckStatusAsync() != PermissionStatus.Granted)
        {
            await PostNotificationService.RequestAsync();
        }
    }

    private async Task UpdateWeather()
    {
        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
        {
            Tuple<double, double> location = await _geoLocationService.GetCurrentLocation();

            if (location != null)
            {
                var weatherData = await _weatherAPIService.FetchWeather(location.Item1, location.Item2);
                if (weatherData != null)
                {
                    Temperature = weatherData.Current.Temperature;
                    FeelsLike = weatherData.Current.FeelsLike;
                    Description = weatherData.Current.Weather[0].Description;
                    LastUpdated = UnixTimeConverter.GetDateTimeFromUnixTime(weatherData.Current.DT);
                }

                Preferences.Set(Constants.LastWeatherUpdateKey, JsonSerializer.Serialize(weatherData));
            }
        }
    }

    private void ToggleNotifications()
    {
        if (NotificationsEnabled)
        {
           _backgroundServiceManager.DisableNotifications();
           Preferences.Set(Constants.NotificationsSettingKey, false);
        }
        else
        {
            _backgroundServiceManager.EnableNotifications();
            Preferences.Set(Constants.NotificationsSettingKey, true);
        }
        
        NotificationsEnabled = !NotificationsEnabled;
    }
}