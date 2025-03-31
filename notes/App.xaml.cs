using Microsoft.Maui.Controls;

namespace notes;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}
}