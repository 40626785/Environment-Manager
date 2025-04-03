﻿using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using EnvironmentManager.Data;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Views;

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

		var a = Assembly.GetExecutingAssembly();
		using var stream = a.GetManifestResourceStream("EnvironmentManager.appsettings.json");

		var config = new ConfigurationBuilder()
			.AddJsonStream(stream)
			.Build();

		builder.Configuration.AddConfiguration(config);

		builder.Services.AddDbContext<MaintenanceDbContext>(options =>
		{
			try
			{
				var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
				Console.WriteLine($"Attempting to connect to database with connection string: {connectionString}");
				options.UseSqlServer(connectionString);
				Console.WriteLine("Database context configured successfully");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error configuring database context: {ex.Message}");
				Console.WriteLine($"Stack trace: {ex.StackTrace}");
				throw;
			}
		});

		// Register ViewModels
		builder.Services.AddSingleton<HomeViewModel>();
		builder.Services.AddSingleton<AllMaintenanceViewModel>();
		builder.Services.AddTransient<MaintenanceViewModel>();
		builder.Services.AddTransient<AboutViewModel>();
        builder.Services.AddTransient<SensorViewModel>(); // Register SensorViewModel

		// Register Pages
		builder.Services.AddSingleton<HomePage>();
		builder.Services.AddSingleton<AllMaintenancePage>();
		builder.Services.AddTransient<MaintenancePage>();
		builder.Services.AddTransient<AboutPage>();
        builder.Services.AddTransient<SensorPage>(); // Register SensorPage

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
