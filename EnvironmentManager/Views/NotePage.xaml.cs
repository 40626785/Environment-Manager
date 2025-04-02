using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class NotePage : ContentPage
{
	public NotePage(NoteViewModel viewModel)
	{
		this.BindingContext = viewModel;
		InitializeComponent();
	}

}