using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class AllMaintenancePage : ContentPage
{
	public AllMaintenancePage(AllMaintenanceViewModel viewModel)
	{
		this.BindingContext = viewModel;
		InitializeComponent();
	}
}
