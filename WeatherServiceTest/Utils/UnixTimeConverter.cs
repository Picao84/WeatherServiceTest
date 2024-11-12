namespace WeatherServiceTest.Utils;

public static class UnixTimeConverter
{
    public static DateTime GetDateTimeFromUnixTime(long unixTime)
    {
        var offset = DateTimeOffset.FromUnixTimeSeconds(unixTime);
        return offset.LocalDateTime;
    }
}