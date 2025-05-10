using EnvironmentManager.ViewModels;
namespace EnvironmentManager.Views;

public partial class HistoricalDataPage : ContentPage
{
	private readonly HistoricalDataSelectionViewModel _viewModel;
	public HistoricalDataPage()
	{
		InitializeComponent();
		_viewModel = new HistoricalDataSelectionViewModel();
		BindingContext = _viewModel;
	}
}