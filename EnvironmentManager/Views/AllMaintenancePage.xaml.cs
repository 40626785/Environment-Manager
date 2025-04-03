using EnvironmentManager.ViewModels;
using Microsoft.Maui.Controls;

namespace EnvironmentManager.Views;

public partial class AllMaintenancePage : ContentPage
{
	public AllMaintenancePage(AllMaintenanceViewModel viewModel)
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
}
