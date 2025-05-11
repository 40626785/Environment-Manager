using EnvironmentManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Data;

namespace EnvironmentManager.Views;
/// <summary>
/// Represents the DatabaseAdminPage database context.
/// </summary>
public partial class DatabaseAdminPage : ContentPage
{
	public DatabaseAdminPage(DatabaseAdminViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}


