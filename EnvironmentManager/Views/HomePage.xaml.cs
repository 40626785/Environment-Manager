using EnvironmentManager.ViewModels;
using Microsoft.Maui.Controls;

namespace EnvironmentManager.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//home");
    }

    private async void OnMaintenanceClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//maintenance");
    }

    private async void OnSensorsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//sensors");
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//about");
    }

    private async void OnDatabaseAdminClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(DatabaseAdminPage));
    }


}
