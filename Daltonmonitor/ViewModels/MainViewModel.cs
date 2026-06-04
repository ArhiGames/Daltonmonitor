using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Daltonmonitor.Application;

namespace Daltonmonitor.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly SubstitutionReader _substitutionReader = new();
    
    [RelayCommand]
    public void StartApplicationLogic()
    {
        _substitutionReader.StartProcess();
    }

    [RelayCommand]
    public void StopApplicationLogic()
    {
    }
}