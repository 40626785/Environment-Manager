using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views
{
	public partial class AlertPage : ContentPage
	{
		public AlertPage(AlertViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}


		private async void MaintenanceClicked(object sender, EventArgs e)
		{
			await Shell.Current.GoToAsync(nameof(MaintenancePage));
		}
	}
}

