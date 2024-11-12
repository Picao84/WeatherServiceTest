using WeatherServiceTest.ViewModels;

namespace WeatherServiceTest;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public void TriggerOnAppearing()
    {
        OnAppearing();
    }
    
    public void TriggerOnDisappearing()
    {
        OnDisappearing();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((MainPageViewModel)BindingContext).GetLastUpdatedData();
        ((MainPageViewModel)BindingContext).SubscribeToUpdates();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ((MainPageViewModel)BindingContext).UnsubscribeToUpdates();
    }
}