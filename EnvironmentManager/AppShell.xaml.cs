using EnvironmentManager.Views;
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
        Routing.RegisterRoute(nameof(DatabaseAdminPage), typeof(DatabaseAdminPage));
        Routing.RegisterRoute(nameof(AirQualityPage), typeof(AirQualityPage));
        Routing.RegisterRoute(nameof(ArchiveAirQualityPage), typeof(ArchiveAirQualityPage));
        Routing.RegisterRoute(nameof(ArchiveWaterQualityPage), typeof(ArchiveWaterQualityPage));
        Routing.RegisterRoute(nameof(EditArchiveAirQualityPage), typeof(EditArchiveAirQualityPage));
        Routing.RegisterRoute(nameof(EditAirQualityPage), typeof(EditAirQualityPage));
        Routing.RegisterRoute(nameof(LogPage), typeof(LogPage));
        Routing.RegisterRoute(nameof(ErrorPage), typeof(ErrorPage));




    }
}
