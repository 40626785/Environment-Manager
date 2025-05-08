using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views
{
	public partial class AlertPage : ContentPage
	{
		public AlertPage(AlertViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}

