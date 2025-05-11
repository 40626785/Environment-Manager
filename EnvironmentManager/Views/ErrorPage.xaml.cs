using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views
{
	/// <summary>
	/// Represents the ErrorPage database context.
	/// </summary>
	public partial class ErrorPage : ContentPage
	{
		public ErrorPage(ErrorViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			if (BindingContext is ErrorViewModel vm)
				await vm.LoadDataAsync();
		}
	}
}
