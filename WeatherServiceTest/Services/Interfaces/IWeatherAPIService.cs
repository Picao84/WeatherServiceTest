using WeatherServiceTest.Models;

namespace WeatherServiceTest.Services.Interfaces;

public interface IWeatherAPIService
{
    Task<WeatherData> FetchWeather(double latitude, double longitude);

    event EventHandler<WeatherUpdateArgs>? OnWeatherUpdated;
    
}