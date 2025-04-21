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
        // Bind specific implementation to DBContext abstraction
        builder.Services.AddSingleton<IMaintenanceDataStore, MaintenanceDataStore>();
        builder.Services.AddSingleton<IUserDataStore, UserDataStore>();

		// Register App and AppShell
		builder.Services.AddSingleton<App>(sp =>
{
	var dbInitService = sp.GetRequiredService<IDatabaseInitializationService>();
	return new App(sp, dbInitService); // 👈 this now matches your 2-parameter constructor
});

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

        // Configure UserDbContext
		builder.Services.AddDbContext<UserDbContext>(options =>
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
    
		//Historical Data DB
		builder.Services.AddDbContext<HistoricalDataDbContext>(options =>
		{
			try
			{
				var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
				Debug.WriteLine("Configuring historical data database");
				options.UseSqlServer(connectionString);
				options.EnableSensitiveDataLogging();
				options.EnableDetailedErrors();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error configuring historical data database context: {ex.Message}");
				throw;
			}
		});

	}

	private static void RegisterServices(MauiAppBuilder builder)
	{
		// Register DatabaseInitializationService
		builder.Services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();
    
		// Add other services here
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<ILoginNavService, LoginNavService>();
        builder.Services.AddSingleton<ISessionService, SessionService>();
        builder.Services.AddSingleton<IRunOnMainThread, RunOnMainThread>();
        builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();
	}

	private static void RegisterViewModels(MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<HomeViewModel>();
		builder.Services.AddSingleton<AllMaintenanceViewModel>();
		builder.Services.AddTransient<MaintenanceViewModel>();
		builder.Services.AddTransient<SensorViewModel>();
		builder.Services.AddTransient<AddSensorViewModel>();
		builder.Services.AddTransient<EditSensorViewModel>();
		builder.Services.AddTransient<HistoricalDataSelectionViewModel>();
		builder.Services.AddTransient<HistoricalDataViewerViewModel>();
    builder.Services.AddTransient<LoginViewModel>();
	}

	private static void RegisterPages(MauiAppBuilder builder)
	{
		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<MaintenancePage>();
		builder.Services.AddTransient<AllMaintenancePage>();
		builder.Services.AddTransient<SensorPage>();
		builder.Services.AddTransient<AddSensorPage>();
		builder.Services.AddTransient<EditSensorPage>();
		builder.Services.AddTransient<HistoricalData>();
		builder.Services.AddTransient<HistoricalDataViewerPage>();
    builder.Services.AddTransient<LoginPage>();
	}
}
