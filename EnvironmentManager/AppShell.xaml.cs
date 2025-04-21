using EnvironmentManager.Views; // Add this using statement
using Microsoft.Maui.Controls;

namespace EnvironmentManager;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(AllMaintenancePage), typeof(AllMaintenancePage));
        Routing.RegisterRoute(nameof(SensorPage), typeof(SensorPage));
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
        Routing.RegisterRoute(nameof(EditSensorPage), typeof(EditSensorPage));
        Routing.RegisterRoute(nameof(AddSensorPage), typeof(AddSensorPage));
        Routing.RegisterRoute(nameof(SensorMonitoringPage), typeof(SensorMonitoringPage));
	}
}
