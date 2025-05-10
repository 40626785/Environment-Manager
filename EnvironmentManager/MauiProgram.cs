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
using CommunityToolkit.Mvvm.DependencyInjection;

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
		builder.Services.AddSingleton<App>(sp =>
{
	var dbInitService = sp.GetRequiredService<IDatabaseInitializationService>();
	return new App(sp, dbInitService);
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



		// alerts context
		builder.Services.AddDbContextFactory<AlertDbContext>(options =>
{
	try
	{
		var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
		Debug.WriteLine($"[DEBUG] Configuring AlertDbContext with connection string: {connectionString}");
		options.UseSqlServer(connectionString);
	}
	catch (Exception ex)
	{
		Debug.WriteLine($"[ERROR] Failed to configure AlertDbContext: {ex.Message}");
	}
});

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
		//AirQuality
		builder.Services.AddDbContextFactory<AirQualityDbContext>(options =>
{
	try
	{
		Debug.WriteLine($"inside AirQuality");
		var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
		Debug.WriteLine("Configuring AirQuality admin context");
		options.UseSqlServer(connectionString);
	}
	catch (Exception ex)
	{
		Debug.WriteLine($"Error configuring database AirQuality context: {ex.Message}");
		throw;
	}
});


		// Register DatabaseAdminDbContext 
		builder.Services.AddDbContext<DatabaseAdminDbContext>(options =>
		{
			try
			{
				Debug.WriteLine($"inside DatabaseAdminDbContext");
				var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
				Debug.WriteLine("Configuring database admin context");
				options.UseSqlServer(connectionString);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error configuring database admin context: {ex.Message}");
				throw;
			}
		});

		// configure error table context
		builder.Services.AddDbContextFactory<ErrorDbContext>(options =>
		{
			try
			{
				var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
				Debug.WriteLine($"Configuring error table database");
				options.UseSqlServer(connectionString);
				options.EnableSensitiveDataLogging();
				options.EnableDetailedErrors();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error configuring error table database context: {ex.Message}");
				throw;
			}
		});

		// Configure LocationDbContext
		builder.Services.AddDbContextFactory<LocationDbContext>(options =>
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

		//air DB
		builder.Services.AddDbContextFactory<ArchiveAirQualityDbContext>(options =>
		{
			var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
			options.UseSqlServer(connectionString);
		});

		// User DbContext SensorDbContext
		builder.Services.AddDbContextFactory<UserDbContext>(options =>
		{
			try
			{
				var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
				Debug.WriteLine($"Configuring User table database");
				options.UseSqlServer(connectionString);
				options.EnableSensitiveDataLogging();
				options.EnableDetailedErrors();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error configuring User table database context: {ex.Message}");
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
		// Configure Log DbContext
		builder.Services.AddDbContextFactory<LogDbContext>(options =>
		{
			try
			{
				var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
				Debug.WriteLine($"Configuring Log database");
				options.UseSqlServer(connectionString);
				options.EnableSensitiveDataLogging();
				options.EnableDetailedErrors();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error configuring Log database context: {ex.Message}");
				throw;
			}
		});
	}



	private static void RegisterServices(MauiAppBuilder builder)
	{
		// Register DatabaseInitializationService
		builder.Services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();

		// Add other services here
		builder.Services.AddSingleton<ILoggingService, DatabaseLoggingService>();


		builder.Services.AddSingleton<TableMetadataService>();
		builder.Services.AddSingleton<IUserDialogService, MauiUserDialogService>();
		builder.Services.AddSingleton<Ioc>();


	}

	private static void RegisterViewModels(MauiAppBuilder builder)
	{
		builder.Services.AddSingleton<HomeViewModel>();
		builder.Services.AddSingleton<AllMaintenanceViewModel>();
		builder.Services.AddTransient<MaintenanceViewModel>();
		builder.Services.AddTransient<SensorViewModel>();
		builder.Services.AddTransient<AddSensorViewModel>();
		builder.Services.AddTransient<EditSensorViewModel>();
		builder.Services.AddTransient<DatabaseAdminViewModel>();
		builder.Services.AddTransient<AirQualityAdminViewModel>();
		builder.Services.AddTransient<ErrorTableAdminViewModel>();
		builder.Services.AddSingleton<AirQualityAdminViewModel>();
		builder.Services.AddTransient<ArchiveAirQualityViewModel>();
		builder.Services.AddTransient<EditArchiveAirQualityViewModel>();
		builder.Services.AddTransient<EditAirQualityViewModel>();
		builder.Services.AddTransient<AirQualityAdminViewModel>();
		builder.Services.AddTransient<LogViewModel>();
		builder.Services.AddTransient<ErrorViewModel>();
		builder.Services.AddTransient<AdminLocationViewModel>();
		builder.Services.AddTransient<EditLocationViewModel>();
		builder.Services.AddTransient<EditUserViewModel>();
		builder.Services.AddTransient<AdminUserViewModel>();
		builder.Services.AddTransient<AddUserViewModel>();
		builder.Services.AddTransient<AlertViewModel>();
		builder.Services.AddTransient<ResolvedAlertsViewModel>();
		builder.Services.AddTransient<HistoricalDataSelectionViewModel>();
		builder.Services.AddTransient<HistoricalAirQualityViewModel>();
	}

	private static void RegisterPages(MauiAppBuilder builder)
	{
		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<MaintenancePage>();
		builder.Services.AddTransient<AllMaintenancePage>();
		builder.Services.AddTransient<SensorPage>();
		builder.Services.AddTransient<AddSensorPage>();
		builder.Services.AddTransient<EditSensorPage>();
		builder.Services.AddTransient<DatabaseAdminPage>();
		builder.Services.AddTransient<DatabaseAdminPage>();
		builder.Services.AddTransient<AirQualityPage>();
		builder.Services.AddTransient<ArchiveAirQualityPage>();
		builder.Services.AddTransient<EditArchiveAirQualityPage>();
		builder.Services.AddTransient<AirQualityPage>();
		builder.Services.AddTransient<EditAirQualityPage>();
		builder.Services.AddTransient<LogPage>();
		builder.Services.AddTransient<ErrorPage>();
		builder.Services.AddTransient<EditLocationPage>();
		builder.Services.AddTransient<AdminLocationPage>();
		builder.Services.AddTransient<AdminUserPage>();
		builder.Services.AddTransient<EditUserPage>();
		builder.Services.AddTransient<AddUserPage>();
		builder.Services.AddTransient<AlertPage>();
		builder.Services.AddTransient<ResolvedAlertsPage>();
		builder.Services.AddTransient<HistoricalDataPage>();
		builder.Services.AddTransient<HistoricalAirQualityPage>();
	}
}
