using System.Diagnostics;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Views;


public partial class ArchiveAirQualityPage : ContentPage
{
	public ArchiveAirQualityPage(ArchiveAirQualityViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}


	private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		Debug.WriteLine("Row tapped!");

		var selectedRecord = e.CurrentSelection.FirstOrDefault() as ArchiveAirQuality;

		if (selectedRecord != null)
		{
			Debug.WriteLine($"Navigating to edit record ID: {selectedRecord.Id}");
			await Shell.Current.GoToAsync(nameof(EditArchiveAirQualityPage),
				new Dictionary<string, object> { { "Record", selectedRecord } });

			((CollectionView)sender).SelectedItem = null;
		}
	}

}
