using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views
{
	public partial class AdminLocationPage : ContentPage
	{
		public AdminLocationPage(AdminLocationViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext is AdminLocationViewModel vm)
				await vm.LoadDataAsync();
		}
	}
}
