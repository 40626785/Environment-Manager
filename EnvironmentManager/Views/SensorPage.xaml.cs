using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class SensorPage : ContentPage
{
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
