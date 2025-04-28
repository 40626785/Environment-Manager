using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

/// <summary>
/// Code-behind for the User Management page.
/// Provides the UI for listing, searching, adding, editing, and deleting users.
/// </summary>
public partial class UserManagementPage : ContentPage
{
    private readonly UserManagementViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserManagementPage"/> class.
    /// </summary>
    /// <param name="viewModel">The view model associated with this page.</param>
    public UserManagementPage(UserManagementViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// Called when the page is about to become visible.
    /// Refreshes the user list.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadUsers();
    }
} 