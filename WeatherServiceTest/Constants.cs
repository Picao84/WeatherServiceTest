namespace WeatherServiceTest;

public static class Constants
{
    public static string OpenWeatherApiKey = "53567b4e07dfa4dfe8b7737adbd4ec4a";

    public static string WeatherApiUrl = "https://api.openweathermap.org/data/3.0/onecall?&units=metric&";

    public static TimeSpan MINIMUM_FETCH_TIME = TimeSpan.FromMinutes(5);
    
    public static string LastWeatherUpdateKey = "lastUpdatedKey";
    
    public static string NotificationsSettingKey = "notificationsSettingKey";
}