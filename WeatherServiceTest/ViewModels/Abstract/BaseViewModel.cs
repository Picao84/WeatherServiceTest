using WeatherServiceTest.Models.Abstract;
using WeatherServiceTest.Services.Interfaces;
using WeatherServiceTest.Services.PartialMethods;

namespace WeatherServiceTest.ViewModels.Abstract;

public class BaseViewModel : NotifiableClass
{
    protected BackgroundServiceManager _backgroundServiceManager;
    protected GeoLocationService _geoLocationService;
    protected IWeatherAPIService _weatherAPIService;
    protected PostNotificationService PostNotificationService;

    public BaseViewModel(BackgroundServiceManager backgroundServiceManager, IWeatherAPIService weatherApiService, GeoLocationService geoLocationService,
        PostNotificationService postNotificationService)
    {
        _backgroundServiceManager = backgroundServiceManager;
        _geoLocationService = geoLocationService;
        _weatherAPIService = weatherApiService;
        PostNotificationService = postNotificationService;
    }
}