using notes.Views;

namespace notes;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new NavigationPage(new HomePage(new ViewModels.HomeViewModel()));
	}
}