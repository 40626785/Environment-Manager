using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class EditUserPage : ContentPage
{
    public EditUserPage(EditUserViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
} 