using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class AddSensorPage : ContentPage
{
    public AddSensorPage(AddSensorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 