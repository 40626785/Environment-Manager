using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;
/// <summary>
/// Represents the EditArchiveAirQualityPage database context.
/// </summary>
public partial class EditArchiveAirQualityPage : ContentPage
{
	public EditArchiveAirQualityPage(EditArchiveAirQualityViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
