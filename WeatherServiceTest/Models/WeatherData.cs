using System.Text.Json.Serialization;

namespace WeatherServiceTest.Models;

public class WeatherData
{
    [JsonPropertyName("current")]
    public WeatherDataCurrent Current { get; set; }
}