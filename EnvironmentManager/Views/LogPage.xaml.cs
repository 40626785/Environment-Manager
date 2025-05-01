using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views
{
	public partial class LogPage : ContentPage
	{
		public LogPage(LogViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext is LogViewModel vm)
				await vm.LoadDataAsync();
		}
	}
}
