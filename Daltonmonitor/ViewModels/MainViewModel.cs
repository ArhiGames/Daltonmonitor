using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Daltonmonitor.Application;

namespace Daltonmonitor.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting = "Welcome to Avalonia!";

    public SubstitutionReader SubstitutionReader = new();
    
    [RelayCommand]
    public void StartApplicationLogic()
    {
        SubstitutionReader.StartProcess();
    }

    [RelayCommand]
    public void StopApplicationLogic()
    {
        Greeting = "Welcome to Avalonia";
    }
}