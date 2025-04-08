namespace EnvironmentManager;
using System.Diagnostics;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(Views.NotePage), typeof(Views.NotePage));

        Routing.RegisterRoute(nameof(Views.MaintenancePage), typeof(Views.MaintenancePage)); //register route for MaintenancePage as its not contained in AppShell

        Trace.Listeners.Add(new DefaultTraceListener());

		MainPage = new AppShell();
	}
}
