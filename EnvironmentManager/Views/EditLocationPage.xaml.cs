using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views
{
	public partial class EditLocationPage : ContentPage
	{
		public EditLocationPage(EditLocationViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
