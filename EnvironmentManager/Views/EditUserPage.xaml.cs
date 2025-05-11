using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views
{
	/// <summary>
	/// Represents the EditUserPage database context.
	/// </summary>
	public partial class EditUserPage : ContentPage
	{
		public EditUserPage(EditUserViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;
		}
	}
}
