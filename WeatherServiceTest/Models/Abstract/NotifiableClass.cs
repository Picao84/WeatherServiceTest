using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WeatherServiceTest.Models.Abstract;

public abstract class NotifiableClass : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this, new PropertyChangedEventArgs(propertyName));
    }

    protected void SetProperty<T>(ref T backingField,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(
                backingField, value)) return;
        backingField = value;
        OnPropertyChanged(propertyName: propertyName);
    }

    protected void RaisePropertyChangedEvent(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }
}