using System.Text.Json.Serialization;

namespace WeatherServiceTest.Models;

public class WeatherDataCurrent
{
    [JsonPropertyName("temp")]
    public float Temperature { get; set; }
    
    [JsonPropertyName("feels_like")]
    public float FeelsLike { get; set; }
    
    [JsonPropertyName("wind_speed")]
    public float WindSpeed { get; set; }
    
    [JsonPropertyName("weather")]
    public WeatherDataCurrentWeather[] Weather { get; set; }
    
    [JsonPropertyName("dt")]
    public long DT { get; set; }
}