using EnvironmentManager.ViewModels;
using Microsoft.Maui.Controls;

namespace EnvironmentManager.Views
{
	public partial class AirQualityPage : ContentPage
	{
		public AirQualityPage(AirQualityAdminViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}


		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext is AirQualityAdminViewModel vm)
			{
				await vm.LoadDataAsync();
			}
		}
	}
}
