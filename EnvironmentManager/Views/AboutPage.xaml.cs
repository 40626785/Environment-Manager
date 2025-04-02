using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;

public partial class AboutPage : ContentPage
{
	public AboutPage()
	{
		this.BindingContext = new AboutViewModel();
		InitializeComponent();
	}
}
