using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace EnvironmentManager;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(Views.MaintenancePage), typeof(Views.MaintenancePage));

		Trace.Listeners.Add(new DefaultTraceListener());

		MainPage = new AppShell();
	}
}
