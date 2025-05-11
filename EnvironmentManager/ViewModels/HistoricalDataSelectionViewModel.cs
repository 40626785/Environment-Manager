using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Views;

namespace EnvironmentManager.ViewModels
{
    /// <summary>
    /// Represents the HistoricalDataSelectionViewModel database context.
    /// </summary>
    public partial class HistoricalDataSelectionViewModel : ObservableObject
    {
        public ICommand SelectTableCommand { get; }

        public HistoricalDataSelectionViewModel()
        {
            SelectTableCommand = new RelayCommand<string>(NavigateToViewer);
        }

        private async void NavigateToViewer(string tableName)
        {
            try
            {
                Debug.WriteLine($"inside NavigateToViewer {tableName}");
                var route = $"{nameof(HistoricalDataViewerPage)}?tableName={tableName}";
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                // Log the exception and display an alert for user feedback
                Debug.WriteLine($"Navigation error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to navigate to the viewer: {ex.Message}", "OK");
            }
        }


    }
}