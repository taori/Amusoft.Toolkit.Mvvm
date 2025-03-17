using System;
using Amusoft.Toolkit.Mvvm.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TestApp.Wpf.ViewModels;

public partial class MainVM : ObservableObject
{
    private readonly INavigationService _navigationService;

    public MainVM(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _navigationService.OnHistoryChanged += OnNavigationServiceOnOnHistoryChanged;
    }

    private void OnNavigationServiceOnOnHistoryChanged(string name)
    {
        MainHasHistory = _navigationService.CanGoBack(name);
    }

    [ObservableProperty]
    private bool _mainHasHistory;
    
    [ObservableProperty]
    private object? _content;

    [RelayCommand]
    private void RunGC()
    {
        GC.Collect();
    }

    [RelayCommand]
    private void GoBack()
    {
        _ = _navigationService.GoBackAsync("Main");
    }

    [RelayCommand]
    private void DisplayViewA()
    {
        _ = _navigationService.PushAsync("Main", new ViewAVM());
    }

    [RelayCommand]
    private void DisplayViewB()
    {
        _ = _navigationService.PushAsync("Main", new ViewBVM());
    }

    [RelayCommand]
    private void DisplayViewANested()
    {
        _ = _navigationService.PushAsync("Main", new SubFolder.ViewAVM());
    }

    [RelayCommand]
    private void DisplayGcView()
    {
        _ = _navigationService.PushAsync<GcVM>("Main");
        // Content = new ViewBVM();
    }
}