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

        // Register routes for navigation (combined from both versions)
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(AllMaintenancePage), typeof(AllMaintenancePage));
        Routing.RegisterRoute(nameof(SensorPage), typeof(SensorPage));
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
        Routing.RegisterRoute(nameof(EditSensorPage), typeof(EditSensorPage));
        Routing.RegisterRoute(nameof(AddSensorPage), typeof(AddSensorPage));
        Routing.RegisterRoute(nameof(SensorMonitoringPage), typeof(SensorMonitoringPage));
        Routing.RegisterRoute(nameof(HistoricalData), typeof(HistoricalData));
        Routing.RegisterRoute(nameof(HistoricalDataViewerPage), typeof(HistoricalDataViewerPage));
        Routing.RegisterRoute(nameof(ThresholdMapPage), typeof(ThresholdMapPage));
        Routing.RegisterRoute(nameof(UserManagementPage), typeof(UserManagementPage));
        Routing.RegisterRoute(nameof(AnomalyPage), typeof(AnomalyPage));
        Routing.RegisterRoute(nameof(SensorAnomaliesPage), typeof(SensorAnomaliesPage));
        Routing.RegisterRoute(nameof(FirmwareUpdatePage), typeof(FirmwareUpdatePage));
    }

    /// <summary>
    /// Conditionally creates tabs to access protected views based on roles
    /// </summary>
    private void RoleBasedNavigation()
    {
        TabBar tabBar = ShellTabBar;
        string role = Preferences.Get("role", "");
        string roleValue = Preferences.Get("roleValue", "");

        // Ensure roleValue fallback logic is preserved
        if (string.IsNullOrEmpty(role) && !string.IsNullOrEmpty(roleValue))
        {
            switch (roleValue)
            {
                case "0":
                    role = "Administrator";
                    break;
                case "1":
                    role = "EnvironmentalScientist";
                    break;
                case "2":
                    role = "OperationsManager";
                    break;
            }
        }

        switch (role)
        {
            case "OperationsManager":
                var maintenance = new Tab { Title = "MAINTENANCE" };
                maintenance.Items.Add(new ShellContent
                {
                    Title = "MAINTENANCE",
                    Content = _serviceProvider.GetRequiredService<AllMaintenancePage>()
                });
                tabBar.Items.Add(maintenance);

                var monitor = new Tab { Title = "MONITOR" };
                monitor.Items.Add(new ShellContent
                {
                    Title = "MONITOR",
                    Content = _serviceProvider.GetRequiredService<SensorMonitoringPage>()
                });
                tabBar.Items.Add(monitor);
                break;

            case "EnvironmentalScientist":
                var sensors = new Tab { Title = "SENSORS" };
                sensors.Items.Add(new ShellContent
                {
                    Title = "SENSORS",
                    Content = _serviceProvider.GetRequiredService<SensorPage>()
                });
                tabBar.Items.Add(sensors);

                var threshold = new Tab { Title = "THRESHOLD" };
                threshold.Items.Add(new ShellContent
                {
                    Title = "THRESHOLD",
                    Content = _serviceProvider.GetRequiredService<ThresholdMapPage>()
                });
                tabBar.Items.Add(threshold);

                var alerts = new Tab { Title = "ALERTS" };
                alerts.Items.Add(new ShellContent
                {
                    Title = "ALERTS",
                    Content = _serviceProvider.GetRequiredService<AnomalyPage>()
                });
                tabBar.Items.Add(alerts);
                break;

            case "Administrator":
                var adminMaintenance = new Tab { Title = "MAINTENANCE" };
                adminMaintenance.Items.Add(new ShellContent
                {
                    Title = "MAINTENANCE",
                    Content = _serviceProvider.GetRequiredService<AllMaintenancePage>()
                });
                tabBar.Items.Add(adminMaintenance);

                var adminMonitor = new Tab { Title = "MONITOR" };
                adminMonitor.Items.Add(new ShellContent
                {
                    Title = "MONITOR",
                    Content = _serviceProvider.GetRequiredService<SensorMonitoringPage>()
                });
                tabBar.Items.Add(adminMonitor);

                var users = new Tab { Title = "USERS" };
                users.Items.Add(new ShellContent
                {
                    Title = "USERS",
                    Content = _serviceProvider.GetRequiredService<UserManagementPage>()
                });
                tabBar.Items.Add(users);
                break;
        }
    }
}
