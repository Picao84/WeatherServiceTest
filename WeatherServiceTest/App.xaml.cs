using Microsoft.Maui.LifecycleEvents;
using WeatherServiceTest.Services.Interfaces;

namespace WeatherServiceTest;

public partial class App : Application
{

    public static IWeatherAPIService WeatherAPIService;
    public static GeoLocationService GeoLocationService;
    
    public App(IWeatherAPIService weatherAPIService, GeoLocationService geoLocationService)
    {
        InitializeComponent();
        WeatherAPIService = weatherAPIService;
        GeoLocationService = geoLocationService;
        
        MainPage = new AppShell();
    }

    protected override void OnResume()
    {
        base.OnResume();

        if (((AppShell)MainPage).CurrentPage is MainPage main)
        {
            main.TriggerOnAppearing();
        }
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        if (((AppShell)MainPage).CurrentPage is MainPage main)
        {
            main.TriggerOnDisappearing();
        }
    }
}