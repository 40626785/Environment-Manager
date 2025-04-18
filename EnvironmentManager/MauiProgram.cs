using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using EnvironmentManager.Data;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Views;
using System.Diagnostics;
using EnvironmentManager.Services;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Load configuration
		ConfigureAppSettings(builder);

		// Register database contexts
		RegisterDatabaseContexts(builder);

		// Register services
		RegisterServices(builder);

		// Register ViewModels
		RegisterViewModels(builder);

		// Register pages
		RegisterPages(builder);

        // Bind specific implementation to DBContext abstraction
        builder.Services.AddSingleton<IMaintenanceDataStore, MaintenanceDataStore>();

		// Register App and AppShell
		builder.Services.AddSingleton<App>();
		builder.Services.AddSingleton<AppShell>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	private static void ConfigureAppSettings(MauiAppBuilder builder)
	{
		try
		{
			var assembly = Assembly.GetExecutingAssembly();
			using var stream = assembly.GetManifestResourceStream("EnvironmentManager.appsettings.json");

			if (stream != null)
			{
				var config = new ConfigurationBuilder()
					.AddJsonStream(stream)
					.Build();

				builder.Configuration.AddConfiguration(config);
				Debug.WriteLine("Configuration loaded successfully");
			}
			else
			{
				Debug.WriteLine("Warning: Could not find appsettings.json as embedded resource");
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error loading configuration: {ex.Message}");
		}
	}

	private static void RegisterDatabaseContexts(MauiAppBuilder builder)
	{
		// Configure MaintenanceDbContext
		builder.Services.AddDbContext<MaintenanceDbContext>(options =>
		{
			try
			{
				var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
				Debug.WriteLine($"Configuring maintenance database");
				options.UseSqlServer(connectionString);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error configuring maintenance database context: {ex.Message}");
				throw;
			}
		});

		// Configure LocationDbContext
		builder.Services.AddDbContext<LocationDbContext>(options =>
		{
			try
			{
				var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
				Debug.WriteLine($"Configuring location database");
				options.UseSqlServer(connectionString);
				options.EnableSensitiveDataLogging();
				options.EnableDetailedErrors();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error configuring location database context: {ex.Message}");
				throw;
			}
		});

		// Configure SensorDbContext
		builder.Services.AddDbContext<SensorDbContext>(options =>
		{
			try
			{
				var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
				Debug.WriteLine($"Configuring sensor database");
				options.UseSqlServer(connectionString);
				options.EnableSensitiveDataLogging();
				options.EnableDetailedErrors();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error configuring sensor database context: {ex.Message}");
				throw;
			}
		});

	}

	private static void RegisterServices(MauiAppBuilder builder)
	{
		// Register DatabaseInitializationService
		builder.Services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();
		
		// Add other services here
	}

	private static void RegisterViewModels(MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<HomeViewModel>();
		builder.Services.AddSingleton<AllMaintenanceViewModel>();
		builder.Services.AddTransient<MaintenanceViewModel>();
		builder.Services.AddTransient<SensorViewModel>();
		builder.Services.AddTransient<AddSensorViewModel>();
		builder.Services.AddTransient<EditSensorViewModel>();
	}

	private static void RegisterPages(MauiAppBuilder builder)
	{
		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<MaintenancePage>();
		builder.Services.AddTransient<AllMaintenancePage>();
		builder.Services.AddTransient<SensorPage>();
		builder.Services.AddTransient<AddSensorPage>();
		builder.Services.AddTransient<EditSensorPage>();
	}
}
