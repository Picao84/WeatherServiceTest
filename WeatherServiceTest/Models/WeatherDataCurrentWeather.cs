using System.Text.Json.Serialization;

namespace WeatherServiceTest.Models;

public class WeatherDataCurrentWeather
{
    [JsonPropertyName("description")]
    public string Description { get; set; }
}