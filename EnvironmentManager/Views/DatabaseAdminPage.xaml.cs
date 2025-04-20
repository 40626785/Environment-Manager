using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class DatabaseAdminPage : ContentPage
{
	public DatabaseAdminPage(DatabaseAdminViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}

