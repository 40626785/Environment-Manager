using EnvironmentManager.Views; // Add this using statement
using Microsoft.Maui.Controls;

namespace EnvironmentManager;

public partial class AppShell : Shell
{
    private IServiceProvider _serviceProvider;
	public AppShell(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
        InitializeComponent();
        RoleBasedNavigation();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(AllMaintenancePage), typeof(AllMaintenancePage));
        Routing.RegisterRoute(nameof(SensorPage), typeof(SensorPage));
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
        Routing.RegisterRoute(nameof(EditSensorPage), typeof(EditSensorPage));
        Routing.RegisterRoute(nameof(AddSensorPage), typeof(AddSensorPage));
	}

    /// <summary>
    /// Conditionally creates tabs to access protected views based on roles
    /// </summary>
    private void RoleBasedNavigation()
    {
        TabBar tabBar = ShellTabBar;
        string role = Preferences.Get("role","");
        switch(role){
            case "OperationsManager":
                var maintenance = new Tab
                {
                    Title = "MAINTENANCE"
                };

                maintenance.Items.Add(new ShellContent
                {
                    Title = "MAINTENANCE",
                    Content = _serviceProvider.GetRequiredService<AllMaintenancePage>()
                });
                tabBar.Items.Add(maintenance);
                break;
            case "EnvironmentalScientist":
                var sensors = new Tab
                {
                    Title = "SENSORS"
                };

                sensors.Items.Add(new ShellContent
                {
                    Title = "SENSORS",
                    Content = _serviceProvider.GetRequiredService<SensorPage>()
                });
                tabBar.Items.Add(sensors);
                break;
        }
    }
}
