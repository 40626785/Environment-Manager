using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace EnvironmentManager;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Register routes for navigation
		Routing.RegisterRoute(nameof(Views.MaintenancePage), typeof(Views.MaintenancePage)); //register route for MaintenancePage as its not contained in AppShell
		Routing.RegisterRoute(nameof(Views.HomePage), typeof(Views.HomePage));
		Routing.RegisterRoute(nameof(Views.AllMaintenancePage), typeof(Views.AllMaintenancePage));
		Routing.RegisterRoute(nameof(Views.SensorPage), typeof(Views.SensorPage));
		Routing.RegisterRoute("AddSensor", typeof(Views.AddSensorPage));
		Routing.RegisterRoute("EditSensor", typeof(Views.EditSensorPage));
		Routing.RegisterRoute(nameof(Views.AboutPage), typeof(Views.AboutPage));

		Trace.Listeners.Add(new DefaultTraceListener());

		MainPage = new AppShell();
	}
}
