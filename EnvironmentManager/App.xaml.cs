namespace EnvironmentManager;
using System.Diagnostics;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(Views.NotePage), typeof(Views.NotePage));

        Routing.RegisterRoute(nameof(Views.MaintenancePage), typeof(Views.MaintenancePage));

        Trace.Listeners.Add(new DefaultTraceListener());

		MainPage = new AppShell();
	}
}
