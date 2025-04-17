using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Views;

namespace EnvironmentManager.ViewModels
{
    public partial class HistoricalDataSelectionViewModel : ObservableObject
    {
        public ICommand SelectTableCommand { get; }

        public HistoricalDataSelectionViewModel()
        {
            SelectTableCommand = new RelayCommand<string>(NavigateToViewer);
        }

        private async void NavigateToViewer(string tableName)
        {
            var route = $"{nameof(HistoricalDataViewerPage)}?tableName={tableName}";
            await Shell.Current.GoToAsync(route);
        }

    }
}

