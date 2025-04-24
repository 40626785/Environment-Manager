using EnvironmentManager.Views; // Add this using statement
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace EnvironmentManager;

public partial class AppShell : Shell {
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
        Routing.RegisterRoute(nameof(SensorMonitoringPage), typeof(SensorMonitoringPage));
        Routing.RegisterRoute(nameof(HistoricalData), typeof(HistoricalData));
        Routing.RegisterRoute(nameof(HistoricalDataViewerPage), typeof(HistoricalDataViewerPage));
        Routing.RegisterRoute(nameof(UserManagementPage), typeof(UserManagementPage));
	}

    /// <summary>
    /// Conditionally creates tabs to access protected views based on roles
    /// </summary>
    private void RoleBasedNavigation()
    {
        TabBar tabBar = ShellTabBar;
        string role = Preferences.Get("role","");
        
        Debug.WriteLine($"Retrieved role from preferences: '{role}'");
        
        // Handle both string role names and numeric role values
        bool isOperationsManager = role == "OperationsManager" || role == "2";
        bool isEnvironmentalScientist = role == "EnvironmentalScientist" || role == "1";
        bool isAdministrator = role == "Administrator" || role == "0";
        
        Debug.WriteLine($"Role evaluation: OperationsManager={isOperationsManager}, EnvironmentalScientist={isEnvironmentalScientist}, Administrator={isAdministrator}");
        
        if (isOperationsManager)
        {
            // Add Maintenance tab
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

            // Add Monitor tab
            var monitor = new Tab
            {
                Title = "MONITOR"
            };
            monitor.Items.Add(new ShellContent
            {
                Title = "MONITOR",
                Content = _serviceProvider.GetRequiredService<SensorMonitoringPage>()
            });
            tabBar.Items.Add(monitor);
        }
        else if (isEnvironmentalScientist)
        {
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
        }
        else if (isAdministrator)
        {
            // Administrators get access to monitoring and maintenance
            var adminMaintenance = new Tab
            {
                Title = "MAINTENANCE"
            };
            adminMaintenance.Items.Add(new ShellContent
            {
                Title = "MAINTENANCE",
                Content = _serviceProvider.GetRequiredService<AllMaintenancePage>()
            });
            tabBar.Items.Add(adminMaintenance);
            
            var adminMonitor = new Tab
            {
                Title = "MONITOR"
            };
            adminMonitor.Items.Add(new ShellContent
            {
                Title = "MONITOR",
                Content = _serviceProvider.GetRequiredService<SensorMonitoringPage>()
            });
            tabBar.Items.Add(adminMonitor);
        }
        
        // Add Users tab for all users
        var users = new Tab
        {
            Title = "USERS"
        };
        users.Items.Add(new ShellContent
        {
            Title = "USERS",
            Content = _serviceProvider.GetRequiredService<UserManagementPage>()
        });
        tabBar.Items.Add(users);
    }
}
