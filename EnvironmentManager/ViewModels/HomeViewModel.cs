using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.ViewModels;

/// <summary>
/// ViewModel for the home page dashboard.
/// Currently displays static demo data until full implementation.
/// </summary>
public partial class HomeViewModel : ObservableObject
{
    private ILoginNavService _loginNavService;

    // Basic properties for demo data display
    [ObservableProperty]
    private string _airQualityValue = "72";

    [ObservableProperty]
    private string _airQualityStatus = "Moderate";

    [ObservableProperty]
    private string _waterPhValue = "7.2";

    [ObservableProperty]
    private string _waterPhStatus = "Normal";

    [ObservableProperty]
    private string _temperatureValue = "24°C";

    [ObservableProperty]
    private string _weatherStatus = "Partly Cloudy";

    [ObservableProperty]
    private string _sensorCountValue = "42";

    [ObservableProperty]
    private string _sensorWarningCount = "7";

    public ICommand Logout { get; }

    public HomeViewModel(ILoginNavService loginNavService)
    {
        _loginNavService = loginNavService;
        Logout = new Command(NavigateLogout);
    }

    private void NavigateLogout()
    {
        _loginNavService.RouteOnLogout();
    }

    [RelayCommand]
    private void RefreshDashboard()
    {
        // In the future, this will load real data
        // For now, it's just a placeholder for the UI interaction
    }
}
