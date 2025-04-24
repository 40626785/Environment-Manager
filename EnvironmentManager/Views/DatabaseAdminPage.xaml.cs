using EnvironmentManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Data;

namespace EnvironmentManager.Views;

public partial class DatabaseAdminPage : ContentPage
{
	public DatabaseAdminPage(DatabaseAdminViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}


