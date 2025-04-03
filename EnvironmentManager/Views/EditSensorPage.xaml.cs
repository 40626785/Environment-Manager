using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class EditSensorPage : ContentPage
{
    public EditSensorPage(EditSensorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 