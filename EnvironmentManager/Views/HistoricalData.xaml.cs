using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class HistoricalData : ContentPage
{
	public HistoricalData(HistoricalDataSelectionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
