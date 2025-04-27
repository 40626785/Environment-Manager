using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using EnvironmentManager.Data;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Views;
using EnvironmentManager.Models;
using System.Diagnostics;
using EnvironmentManager.Services;
using EnvironmentManager.Interfaces;
using SkiaSharp.Views.Maui.Controls.Hosting;
using EnvironmentManager.Rules;

namespace EnvironmentManager;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp(true)
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
        builder.Services.AddSingleton<IUserDataStore, UserDataStore>();
        builder.Services.AddSingleton<ISensorDataStore, SensorDataStore>();
        builder.Services.AddSingleton<IUserManagementDataStore, UserManagementDataStore>();
        builder.Services.AddSingleton<IUserLogService, UserLogService>();

        // Register App and AppShell
        builder.Services.AddSingleton<App>(sp =>
        {
            var dbInitService = sp.GetRequiredService<IDatabaseInitializationService>();
            return new App(dbInitService, sp);
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
        string connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");

        void ConfigureContext<TContext>(string dbName) where TContext : DbContext
        {
            builder.Services.AddDbContext<TContext>(options =>
            {
                try
                {
                    Debug.WriteLine($"Configuring {dbName} database");
                    options.UseSqlServer(connectionString);
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error configuring {dbName} database context: {ex.Message}");
                    throw;
                }
            });
        }

        ConfigureContext<MaintenanceDbContext>("maintenance");
        ConfigureContext<UserDbContext>("user");
        ConfigureContext<LocationDbContext>("location");
        ConfigureContext<SensorDbContext>("sensor");
        ConfigureContext<HistoricalDataDbContext>("historical data");
        ConfigureContext<UserManagementDbContext>("user management");
        ConfigureContext<UserLogDbContext>("user log");
    }

    private static void RegisterServices(MauiAppBuilder builder)
    {
        builder.Services.AddScoped<IDatabaseInitializationService>(sp =>
        {
            var sensorContext = sp.GetRequiredService<SensorDbContext>();
            var locationContext = sp.GetRequiredService<LocationDbContext>();
            var maintenanceContext = sp.GetRequiredService<MaintenanceDbContext>();
            var userManagementContext = sp.GetRequiredService<UserManagementDbContext>();
            var userLogContext = sp.GetRequiredService<UserLogDbContext>();

            return new DatabaseInitializationService(
                sensorContext,
                locationContext,
                maintenanceContext,
                userManagementContext,
                userLogContext);
        });

        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<ILoginNavService, LoginNavService>();
        builder.Services.AddSingleton<ISessionService, SessionService>();
        builder.Services.AddSingleton<IRunOnMainThread, RunOnMainThread>();
        builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();
        builder.Services.AddSingleton<ISensorThresholdService, SensorThresholdService>();
        builder.Services.AddSingleton<IThresholdRules<Sensor>, ActiveOnlineThreshold>();
        builder.Services.AddSingleton<IThresholdRules<Sensor>>(sp => new BatteryPercentageThreshold(10));
        builder.Services.AddSingleton<IAnomalyDetectionService, AnomalyDetectionService>();
    }

    private static void RegisterViewModels(MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<HomeViewModel>();
        builder.Services.AddSingleton<UserManagementViewModel>();

        builder.Services.AddTransient<MaintenanceViewModel>();
        builder.Services.AddTransient<AllMaintenanceViewModel>();
        builder.Services.AddTransient<SensorViewModel>();
        builder.Services.AddTransient<AddSensorViewModel>();
        builder.Services.AddTransient<EditSensorViewModel>();
        builder.Services.AddTransient<SensorMonitoringViewModel>();
        builder.Services.AddTransient<AboutViewModel>();
        builder.Services.AddTransient<HistoricalDataSelectionViewModel>();
        builder.Services.AddTransient<HistoricalDataViewerViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ThresholdMapViewModel>();
        builder.Services.AddTransient<EditUserViewModel>();
        builder.Services.AddTransient<AnomalyDetectionViewModel>();
        builder.Services.AddTransient<SensorAnomaliesViewModel>();
        builder.Services.AddTransient<FirmwareUpdateViewModel>();
    }

    private static void RegisterPages(MauiAppBuilder builder)
    {
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<MaintenancePage>();
        builder.Services.AddTransient<AllMaintenancePage>();
        builder.Services.AddTransient<SensorPage>();
        builder.Services.AddTransient<AddSensorPage>();
        builder.Services.AddTransient<EditSensorPage>();
        builder.Services.AddTransient<SensorMonitoringPage>();
        builder.Services.AddTransient<AboutPage>();
        builder.Services.AddTransient<HistoricalData>();
        builder.Services.AddTransient<HistoricalDataViewerPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ThresholdMapPage>();
        builder.Services.AddTransient<AnomalyPage>();
        builder.Services.AddTransient<SensorAnomaliesPage>();
        builder.Services.AddTransient<FirmwareUpdatePage>();
        builder.Services.AddSingleton<UserManagementPage>();
        builder.Services.AddTransient<EditUserPage>();
    }
}
