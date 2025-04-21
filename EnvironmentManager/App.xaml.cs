using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using EnvironmentManager.Services;
using EnvironmentManager.Views;

namespace EnvironmentManager;

public partial class App : Application
{
	public static IServiceProvider Services { get; private set; }

	private readonly IDatabaseInitializationService _dbInitService;


	public App(IDatabaseInitializationService dbInitService, IServiceProvider serviceProvider)
	{
		InitializeComponent();

		Services = serviceProvider;

		_dbInitService = dbInitService;

		// Register routes for navigation
		Routing.RegisterRoute(nameof(Views.MaintenancePage), typeof(Views.MaintenancePage)); //register route for MaintenancePage as its not contained in AppShell
		Routing.RegisterRoute(nameof(Views.HomePage), typeof(Views.HomePage));
		Routing.RegisterRoute(nameof(Views.AllMaintenancePage), typeof(Views.AllMaintenancePage));
		Routing.RegisterRoute(nameof(Views.SensorPage), typeof(Views.SensorPage));
		Routing.RegisterRoute(nameof(Views.AddSensorPage), typeof(Views.AddSensorPage));
		Routing.RegisterRoute(nameof(Views.EditSensorPage), typeof(Views.EditSensorPage));
		Routing.RegisterRoute(nameof(Views.AboutPage), typeof(Views.AboutPage));

		Trace.Listeners.Add(new DefaultTraceListener());


		MainPage = serviceProvider.GetRequiredService<LoginPage>(); //MainPage is login on startup, with app access only granted on successful login

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
