using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;
/// <summary>
/// Represents the HistoricalDataPage database context.
/// </summary>
public partial class HistoricalDataPage : ContentPage
{
	public HistoricalDataPage(HistoricalDataSelectionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}