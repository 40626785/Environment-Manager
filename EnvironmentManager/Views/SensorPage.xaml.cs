using EnvironmentManager.ViewModels;
namespace EnvironmentManager.Views;

/// <summary>
/// Represents the page for displaying and managing sensors.
/// </summary>
public partial class SensorPage : ContentPage
{
    /// <summary>
    /// Represents the page for displaying and managing sensors.
    /// </summary>
    public SensorPage(SensorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Set the BindingContext to the injected ViewModel
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is SensorViewModel vm)
        {
            vm.LoadSensorsCommand?.Execute(null);
        }
    }
}
