using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class ResolvedAlertsPage : ContentPage
{
	public ResolvedAlertsPage(ResolvedAlertsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
