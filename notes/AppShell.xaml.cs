using notes.Views;

namespace notes;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes for navigation
		Routing.RegisterRoute("NotePage", typeof(NotePage));
		Routing.RegisterRoute("AllNotesPage", typeof(AllNotesPage));
		Routing.RegisterRoute("AboutPage", typeof(AboutPage));
	}
}
