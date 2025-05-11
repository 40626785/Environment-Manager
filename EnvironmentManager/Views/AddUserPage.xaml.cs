using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views
{  /// <summary>
   /// Represents the AddUserPage database context.
   /// </summary>
	public partial class AddUserPage : ContentPage
	{
		public AddUserPage(AddUserViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
