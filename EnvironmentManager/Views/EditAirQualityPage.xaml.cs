using EnvironmentManager.ViewModels;
using Microsoft.Maui.Controls;

namespace EnvironmentManager.Views
{
	public partial class EditAirQualityPage : ContentPage
	{
		public EditAirQualityPage(EditAirQualityViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
