using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Daltonmonitor.Application;

namespace Daltonmonitor.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Application.Daltonmonitor _daltonmonitor = new();
    
    [RelayCommand]
    public void StartApplicationLogic()
    {
        _daltonmonitor.Process();
    }

    [RelayCommand]
    public void StopApplicationLogic()
    {
    }
}