using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Views;

namespace EnvironmentManager.ViewModels
{
    public partial class HistoricalDataSelectionViewModel : BaseViewModel
    {
        public Command NavigateToAirQualityPageCommand { get; }
        public Command NavigateToWaterQualityPageCommand { get; }
        public Command NavigateToWeatherDataPageCommand { get; }

        public HistoricalDataSelectionViewModel()
        {
            NavigateToAirQualityPageCommand = new Command(NavigateToAirQualityPage);
            NavigateToWaterQualityPageCommand = new Command(NavigateToWaterQualityPage);
            NavigateToWeatherDataPageCommand = new Command(NavigateToWeatherDataPage);
        }

        private async void NavigateToAirQualityPage()
        {
            await Shell.Current.GoToAsync("HistoricalAirQualityPage");
        }

        private async void NavigateToWaterQualityPage()
        {
            await Shell.Current.GoToAsync("HistoricalWaterQualityPage");
        }

        private async void NavigateToWeatherDataPage()
        {
            await Shell.Current.GoToAsync("HistoricalWeatherDataPage");
        }

    }
}
