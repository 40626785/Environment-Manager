using EnvironmentManager.ViewModels;
using EnvironmentManager.Data;
using Microsoft.Extensions.DependencyInjection;

namespace EnvironmentManager.Views;

public partial class SensorMonitoringPage : ContentPage
{
    private SensorMonitoringViewModel _viewModel;

    // Default parameterless constructor for XAML instantiation
    public SensorMonitoringPage()
    {
        InitializeComponent();
        
        // Get the SensorDbContext from the dependency injection container
        var context = App.Current.Handler.MauiContext.Services.GetService<SensorDbContext>();
        
        _viewModel = new SensorMonitoringViewModel(context);
        BindingContext = _viewModel;
    }

    public SensorMonitoringPage(SensorDbContext context)
    {
        InitializeComponent();
        
        _viewModel = new SensorMonitoringViewModel(context);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Refresh data when page appears
        _viewModel?.LoadSensorStatusesCommand.Execute(null);
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        
        // Clean up resources
        _viewModel?.StopAutoRefreshCommand.Execute(null);
    }
} 