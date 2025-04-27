using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class EditArchiveAirQualityPage : ContentPage
{
	public EditArchiveAirQualityPage(EditArchiveAirQualityViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
