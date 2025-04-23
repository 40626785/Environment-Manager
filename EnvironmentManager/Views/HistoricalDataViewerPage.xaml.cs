using EnvironmentManager.ViewModels;
using EnvironmentManager.Helpers;
using System.Diagnostics;

namespace EnvironmentManager.Views;

[QueryProperty(nameof(TableName), "tableName")]
public partial class HistoricalDataViewerPage : ContentPage
{
	private readonly HistoricalDataViewerViewModel _viewModel;

	public HistoricalDataViewerPage()
	{
		InitializeComponent();

		// Manually resolve the ViewModel from DI
		_viewModel = Ioc.Resolve<HistoricalDataViewerViewModel>();
		BindingContext = _viewModel;
	}

	private string _tableName;
	public string TableName
	{
		get => _tableName;
		set
		{
			_tableName = Uri.UnescapeDataString(value);
			Debug.WriteLine($"Table name received: {_tableName}");

			var normalized = _tableName.ToLowerInvariant();

			if (normalized == "archive_air_quality")
			{
				Task.Run(async () => await _viewModel.LoadAirQualityDataAsync(false));
			}
			else if (normalized == "archive_water_quality")
			{
				Task.Run(async () => await _viewModel.LoadWaterQualityDataAsync(false));
			}
			else if (normalized == "archive_weather_data")
			{
				Task.Run(async () => await _viewModel.LoadWeatherDataAsync(false));
			}

		}
	}

}
