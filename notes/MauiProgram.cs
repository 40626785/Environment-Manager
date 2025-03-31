using Microsoft.Extensions.Logging;
using notes.Views;
using notes.ViewModels;

namespace notes;

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

		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<NotePage>();
		builder.Services.AddTransient<AllNotesPage>();
		builder.Services.AddTransient<AboutPage>();

		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<NoteViewModel>();
		builder.Services.AddTransient<NotesViewModel>();
		builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
		builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
		builder.Services.AddSingleton<IMap>(Map.Default);

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
