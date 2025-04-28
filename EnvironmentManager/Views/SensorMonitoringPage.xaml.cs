using EnvironmentManager.ViewModels;
using EnvironmentManager.Data;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EnvironmentManager.Views;

/// <summary>
/// Code-behind for the Sensor Monitoring Page.
/// Displays sensor status information and provides interaction points.
/// </summary>
public partial class SensorMonitoringPage : ContentPage
{
    private readonly SensorMonitoringViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="SensorMonitoringPage"/> class.
    /// This constructor attempts to resolve the DbContext via the service provider,
    /// which might be less reliable than constructor injection.
    /// </summary>
    public SensorMonitoringPage()
    {
        InitializeComponent();
        
        try
        {
            // Attempt to resolve the ViewModel and its dependencies (DbContext)
            // This approach using Service Locator pattern is generally discouraged in favor of constructor injection
            var context = App.Services?.GetService<SensorDbContext>(); 
            if (context != null)
            {
                _viewModel = new SensorMonitoringViewModel(context);
            }
            else
            {
                // Handle the case where the context couldn't be resolved
                // Create a view model that might show an error state or limited functionality
                System.Diagnostics.Debug.WriteLine("Error: Could not resolve SensorDbContext in SensorMonitoringPage constructor.");
                _viewModel = new SensorMonitoringViewModel(); // Use parameterless constructor which handles null context
                // Optionally, display an error message to the user here
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error resolving dependencies in SensorMonitoringPage: {ex.Message}");
            _viewModel = new SensorMonitoringViewModel(); // Fallback
        }
        
        BindingContext = _viewModel;
    }

    // NOTE: This constructor with direct DbContext injection is preferable but might not 
    // be used if the page is navigated to via Shell without explicit ViewModel/dependency construction.
    // Consider using a dependency injection framework that supports view model injection into views.
    /*
    public SensorMonitoringPage(SensorMonitoringViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    */

    /// <summary>
    /// Called when the page is about to become visible.
    /// Starts the data refresh.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Request data refresh when the page appears
        _viewModel?.RefreshNowCommand.Execute(null); // Use RefreshNow to trigger immediate load
        _viewModel?.StartAutoRefreshCommand.Execute(null); // Ensure auto-refresh is started
    }
    
    /// <summary>
    /// Called when the page is about to cease being visible.
    /// Stops the auto-refresh background task to conserve resources.
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Stop the auto-refresh task when navigating away from the page
        _viewModel?.StopAutoRefreshCommand.Execute(null);
    }
} 