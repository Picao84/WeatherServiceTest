using System.Text.Json;
using WeatherServiceTest.Models;
using WeatherServiceTest.Services.Interfaces;

namespace WeatherServiceTest.Services;

public class WeatherUpdateArgs : EventArgs
{
    public WeatherData WeatherData { get; }

    public WeatherUpdateArgs(WeatherData weatherData)
    {
        WeatherData = weatherData;
    }
}

public class WeatherAPIService : IWeatherAPIService
{
    public event EventHandler<WeatherUpdateArgs>? OnWeatherUpdated;
    
    public async Task<WeatherData> FetchWeather(double latitude, double longitude)
    {
        try
        {

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(
                    $"{Constants.WeatherApiUrl}lat={latitude}&lon={longitude}&&appid={Constants.OpenWeatherApiKey}");

                if (response.IsSuccessStatusCode)
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrEmpty(stringResponse))
                    {
                        var weatherData = JsonSerializer.Deserialize<WeatherData>(stringResponse);
                        
                        MainThread.BeginInvokeOnMainThread(() => OnWeatherUpdated?.Invoke(this, new WeatherUpdateArgs(weatherData)));
                        
                        return weatherData;
                    }
                    
                    return null;
                }

                return null;
            }

        }
        catch (Exception ex)
        {
            return null;
        }
    }
}