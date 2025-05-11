using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;
/// <summary>
/// Represents the ResolvedAlertsPage database context.
/// </summary>
public partial class ResolvedAlertsPage : ContentPage
{
	public ResolvedAlertsPage(ResolvedAlertsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
