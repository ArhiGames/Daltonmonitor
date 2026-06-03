using CommunityToolkit.Mvvm.ComponentModel;

namespace Daltonmonitor.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";
}