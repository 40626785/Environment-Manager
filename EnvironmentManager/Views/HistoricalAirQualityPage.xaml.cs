using EnvironmentManager.ViewModels;
using Microsoft.Maui.Controls;

namespace EnvironmentManager.Views
{
	public partial class HistoricalAirQualityPage : ContentPage
	{
		private readonly HistoricalAirQualityViewModel _viewModel;

		public HistoricalAirQualityPage(HistoricalAirQualityViewModel viewModel)
		{
			InitializeComponent();

			_viewModel = viewModel;
			BindingContext = _viewModel;
		}

		// Event handler for Year Picker selection
		private void OnYearSelected(object sender, EventArgs e)
		{
			_viewModel.ApplyFilterAsync();  // Reapply filters when year is selected
		}

		// Event handler for Month Picker selection
		private void OnMonthSelected(object sender, EventArgs e)
		{
			_viewModel.ApplyFilterAsync();  // Reapply filters when month is selected
		}

		// Event handler for Sort Picker selection
		private void OnSortOptionSelected(object sender, EventArgs e)
		{
			_viewModel.ApplySortAsync();  // Reapply sorting when sort option is selected
		}
	}
}
