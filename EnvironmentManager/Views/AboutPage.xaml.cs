using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class AboutPage : ContentPage
{
	public AboutPage(AboutViewModel viewModel)
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
