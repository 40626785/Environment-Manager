using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class AllMaintenancePage : ContentPage
{
	public AllMaintenancePage(AllMaintenanceViewModel viewModel)
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
