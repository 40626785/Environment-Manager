using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views
{
	public partial class AddUserPage : ContentPage
	{
		public AddUserPage(AddUserViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
