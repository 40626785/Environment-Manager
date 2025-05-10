using Microsoft.Maui.Controls;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using EnvironmentManager.Services;

namespace EnvironmentManager;

public partial class App : Application
{
	private readonly IDatabaseInitializationService _dbInitService;

	// Static property to expose the IServiceProvider
	public static IServiceProvider Services { get; private set; }

	public App(IServiceProvider serviceProvider, IDatabaseInitializationService dbInitService)
	{
		InitializeComponent();

		// Assign the provided service provider to the static property
		Services = serviceProvider;

		_dbInitService = dbInitService;

		// Register routes for navigation
		Routing.RegisterRoute(nameof(Views.MaintenancePage), typeof(Views.MaintenancePage));
		Routing.RegisterRoute(nameof(Views.HomePage), typeof(Views.HomePage));
		Routing.RegisterRoute(nameof(Views.AllMaintenancePage), typeof(Views.AllMaintenancePage));
		Routing.RegisterRoute(nameof(Views.SensorPage), typeof(Views.SensorPage));
		Routing.RegisterRoute(nameof(Views.AddSensorPage), typeof(Views.AddSensorPage));
		Routing.RegisterRoute(nameof(Views.EditSensorPage), typeof(Views.EditSensorPage));
		Routing.RegisterRoute(nameof(Views.AboutPage), typeof(Views.AboutPage));
		Routing.RegisterRoute(nameof(Views.DatabaseAdminPage), typeof(Views.DatabaseAdminPage));
		Routing.RegisterRoute(nameof(Views.AirQualityPage), typeof(Views.AirQualityPage));
		Routing.RegisterRoute(nameof(Views.HistoricalDataPage), typeof(Views.HistoricalDataPage));
		Routing.RegisterRoute(nameof(Views.HistoricalAirQualityPage), typeof(Views.HistoricalAirQualityPage));

		Trace.Listeners.Add(new DefaultTraceListener());
		MainPage = new AppShell();

		// Initialize database asynchronously without blocking the UI
		InitializeDatabaseAsync();
	}

	private async void InitializeDatabaseAsync()
	{
		try
		{
			// Just verify connections - don't perform extensive testing
			await _dbInitService.VerifyDatabaseConnectionsAsync();

			// Load test data if needed - only in development
#if DEBUG
			await _dbInitService.LoadTestDataIfNeededAsync();
#endif
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error during database initialization: {ex.Message}");
			// Don't crash the app if database initialization fails
		}
	}
}
