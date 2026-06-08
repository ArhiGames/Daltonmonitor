using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Daltonmonitor.Application;

namespace Daltonmonitor.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Application.Daltonmonitor _daltonmonitor = new();

    [ObservableProperty] private string _isRunningButtonText = "Start";
    [ObservableProperty] private string _isRunningStateText = "Not running";
    [ObservableProperty] private string _isRunningStateColor = "RED";

    public MainViewModel()
    {
        _daltonmonitor.OnRunningStateChanged += OnRunningStateChanged;
    }
    
    [RelayCommand]
    public void ToggleRunningApplicationLogic()
    {
        _daltonmonitor.ToggleRunningApplicationLogic();
    }

    private void OnRunningStateChanged(bool newState)
    {
        IsRunningButtonText = newState ? "Stop" : "Start";
        IsRunningStateText = newState ? "Is running" : "Is not running";
        IsRunningStateColor = newState ? "GREEN" : "RED";
    }
}