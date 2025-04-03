using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class SensorPage : ContentPage
{
	public SensorPage(SensorViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel; // Set the BindingContext to the injected ViewModel
	}

    // Optional: Add any view-specific logic here if needed,
    // but most logic should reside in the ViewModel.
    // For example, handling Appearing/Disappearing events to load/unload data.
    /*
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Example: Trigger data loading if not done in constructor
        // if (BindingContext is SensorViewModel vm && !vm.IsDataLoaded) // Assuming an IsDataLoaded flag
        // {
        //     vm.LoadSensorsCommand.Execute(null);
        // }
    }
    */
}
