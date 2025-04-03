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

    private async void OnMaintenanceClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//maintenance");
    }

    private async void OnAboutClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//about");
    }

    private async void OnDashboardClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//dashboard");
    }
}
