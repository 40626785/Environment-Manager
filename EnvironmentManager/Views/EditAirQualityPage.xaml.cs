using EnvironmentManager.ViewModels;
using Microsoft.Maui.Controls;

namespace EnvironmentManager.Views
{    /// <summary>
	 /// Represents the EditAirQualityPage database context.
	 /// </summary>
	public partial class EditAirQualityPage : ContentPage
	{
		public EditAirQualityPage(EditAirQualityViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
