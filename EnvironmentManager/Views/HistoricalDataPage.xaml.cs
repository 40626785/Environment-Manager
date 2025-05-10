using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class HistoricalDataPage : ContentPage
{
	public HistoricalDataPage(HistoricalDataSelectionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}