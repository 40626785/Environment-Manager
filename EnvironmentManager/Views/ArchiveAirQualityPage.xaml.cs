using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;


public partial class ArchiveAirQualityPage : ContentPage
{
	public ArchiveAirQualityPage(ArchiveAirQualityViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
