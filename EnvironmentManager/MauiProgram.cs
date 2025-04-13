using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using EnvironmentManager.Data;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Views;
using System.Diagnostics;

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

		// Configure MaintenanceDbContext
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

		// Configure SensorDbContext
		builder.Services.AddDbContext<SensorDbContext>(options =>
		{
			try
			{
				var connectionString = builder.Configuration.GetConnectionString("DevelopmentConnection");
				Debug.WriteLine($"Configuring sensor database with connection string: {connectionString}");
				options.UseSqlServer(connectionString);
				options.EnableSensitiveDataLogging();
				options.EnableDetailedErrors();

				// Create a temporary context to verify database
				using var tempContext = new SensorDbContext(
					new DbContextOptionsBuilder<SensorDbContext>()
						.UseSqlServer(connectionString)
						.EnableSensitiveDataLogging()
						.EnableDetailedErrors()
						.Options);

				var canConnect = tempContext.Database.CanConnect();
				Debug.WriteLine($"Can connect to database: {canConnect}");

				if (canConnect)
				{
					try
					{
						// Check database tables
						Debug.WriteLine("Checking database tables...");
						var tables = new[] { "Locations", "Sensors", "SensorReadings", "SensorSettings", "EnvironmentalParameters" };
						foreach (var table in tables)
						{
							try
							{
								var checkCmd = tempContext.Database.GetDbConnection().CreateCommand();
								checkCmd.CommandText = $"SELECT COUNT(*) FROM {table}";
								tempContext.Database.OpenConnection();
								var count = Convert.ToInt32(checkCmd.ExecuteScalar());
								Debug.WriteLine($"Table {table} has {count} records");

								// Check for triggers on this table
								checkCmd = tempContext.Database.GetDbConnection().CreateCommand();
								checkCmd.CommandText = $@"
									SELECT tr.name AS TriggerName, 
										   OBJECT_DEFINITION(tr.object_id) AS TriggerDefinition
									FROM sys.triggers tr
									JOIN sys.tables t ON tr.parent_id = t.object_id
									WHERE t.name = '{table}'";
								
								using (var reader = checkCmd.ExecuteReader())
								{
									while (reader.Read())
									{
										var triggerName = reader.GetString(0);
										var triggerDef = reader.GetString(1);
										Debug.WriteLine($"Found trigger '{triggerName}' on table {table}:");
										Debug.WriteLine(triggerDef);
									}
								}
							}
							catch (Exception ex)
							{
								Debug.WriteLine($"Error checking table {table}: {ex.Message}");
							}
						}

						// Check if Locations table exists and has data
						var cmd = tempContext.Database.GetDbConnection().CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM Locations";
						tempContext.Database.OpenConnection();
						var locationCount = Convert.ToInt32(cmd.ExecuteScalar());
						Debug.WriteLine($"Number of locations in database: {locationCount}");

						if (locationCount == 0)
						{
							Debug.WriteLine("No locations found. Loading test data...");
							
							// Try to find test_data.sql
							var possiblePaths = new[]
							{
								Path.Combine(AppContext.BaseDirectory, "sql", "test_data.sql"),
								Path.Combine(FileSystem.AppDataDirectory, "sql", "test_data.sql"),
								Path.Combine(FileSystem.CacheDirectory, "sql", "test_data.sql"),
								"sql/test_data.sql",
								"../sql/test_data.sql"
							};

							string testDataContent = null;
							string usedPath = null;

							foreach (var path in possiblePaths)
							{
								Debug.WriteLine($"Checking for test data at: {path}");
								if (File.Exists(path))
								{
									testDataContent = File.ReadAllText(path);
									usedPath = path;
									Debug.WriteLine($"Found test data file at: {path}");
									break;
								}
							}

							if (testDataContent != null)
							{
								Debug.WriteLine($"Executing test data from {usedPath}");
								
								// Split and execute each command separately
								var commands = testDataContent.Split(new[] { "GO", ";" }, StringSplitOptions.RemoveEmptyEntries);
								foreach (var command in commands)
								{
									if (!string.IsNullOrWhiteSpace(command))
									{
										Debug.WriteLine($"Executing SQL command:");
										Debug.WriteLine(command.Trim());
										using var splitCmd = tempContext.Database.GetDbConnection().CreateCommand();
										splitCmd.CommandText = command;
										splitCmd.ExecuteNonQuery();
									}
								}
								Debug.WriteLine("Test data loaded successfully");

								// Check what was loaded
								Debug.WriteLine("Checking loaded data:");
								
								// Check locations
								cmd.CommandText = @"
									SELECT SiteName, SiteType, Zone 
									FROM Locations 
									ORDER BY SiteType, SiteName";
								using (var reader = cmd.ExecuteReader())
								{
									while (reader.Read())
									{
										Debug.WriteLine($"Location: {reader.GetString(0)}, Type: {reader.GetString(1)}, Zone: {reader.GetString(2)}");
									}
								}

								// Check if any sensors were automatically created
								cmd = tempContext.Database.GetDbConnection().CreateCommand();
								cmd.CommandText = @"
									SELECT s.SensorName, s.SensorType, l.SiteName, l.SiteType
									FROM Sensors s
									JOIN Locations l ON s.LocationId = l.LocationId
									ORDER BY s.SensorId";
								using (var reader = cmd.ExecuteReader())
								{
									while (reader.Read())
									{
										Debug.WriteLine($"Sensor: {reader.GetString(0)}, Type: {reader.GetString(1)}, " +
													  $"Location: {reader.GetString(2)}, LocationType: {reader.GetString(3)}");
									}
								}

								// Verify data was loaded
								cmd.CommandText = "SELECT COUNT(*) FROM Locations";
								locationCount = Convert.ToInt32(cmd.ExecuteScalar());
								Debug.WriteLine($"Number of locations after loading test data: {locationCount}");
							}
							else
							{
								Debug.WriteLine("Could not find test_data.sql file in any of the expected locations");
							}
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine($"Error checking/loading test data: {ex.Message}");
						Debug.WriteLine($"Stack trace: {ex.StackTrace}");
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error configuring sensor database: {ex.Message}");
				Debug.WriteLine($"Stack trace: {ex.StackTrace}");
				throw;
			}
		}, ServiceLifetime.Scoped);

		// Register ViewModels
		builder.Services.AddSingleton<HomeViewModel>();
		builder.Services.AddSingleton<AllMaintenanceViewModel>();
		builder.Services.AddTransient<MaintenanceViewModel>();
		builder.Services.AddTransient<AboutViewModel>();
		builder.Services.AddTransient<SensorViewModel>();
		builder.Services.AddTransient<AddSensorViewModel>();
		builder.Services.AddTransient<EditSensorViewModel>();

		// Register Pages
		builder.Services.AddSingleton<HomePage>();
		builder.Services.AddSingleton<AllMaintenancePage>();
		builder.Services.AddTransient<MaintenancePage>();
		builder.Services.AddTransient<AboutPage>();
		builder.Services.AddTransient<SensorPage>();
		builder.Services.AddTransient<AddSensorPage>();
		builder.Services.AddTransient<EditSensorPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
