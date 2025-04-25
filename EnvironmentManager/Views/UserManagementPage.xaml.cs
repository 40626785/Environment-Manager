using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class UserManagementPage : ContentPage
{
    private UserManagementViewModel _viewModel;

    public UserManagementPage(UserManagementViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadUsers();
    }
} 