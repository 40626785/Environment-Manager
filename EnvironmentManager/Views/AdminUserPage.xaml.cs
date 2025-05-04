using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views
{
	public partial class AdminUserPage : ContentPage
	{
		public AdminUserPage(AdminUserViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext is AdminUserViewModel vm)
				await vm.LoadDataAsync();
		}
	}
}
