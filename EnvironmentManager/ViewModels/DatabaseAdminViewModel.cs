using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Views;

namespace EnvironmentManager.ViewModels
{
    public class DatabaseAdminViewModel : BaseViewModel
    {
        private readonly DatabaseAdminDbContext _dbContext;

        public ObservableCollection<string> TableOptions { get; } = new();
        public string SelectedTable { get; set; }

        public ICommand NavigateToTableCommand { get; }

        public DatabaseAdminViewModel(DatabaseAdminDbContext dbContext)
        {
            _dbContext = dbContext;
            NavigateToTableCommand = new Command(async () => await NavigateToTableAsync());

            LoadTables();
        }

        private void LoadTables()
        {
            try
            {
                var tables = _dbContext.GetAllTableNames();

                TableOptions.Clear();
                foreach (var table in tables)
                {
                    TableOptions.Add(table);
                }

                if (TableOptions.Count == 0)
                {
                    Application.Current.MainPage.DisplayAlert("Info", "No tables found in the database.", "OK");
                }
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", $"Failed to load tables: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToTableAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedTable))
                {
                    await Application.Current.MainPage.DisplayAlert("Warning", "Please select a table first.", "OK");
                    return;
                }

                string route = SelectedTable switch
                {
                    "Archive_Air_Quality" => nameof(ArchiveAirQualityPage),
                    "Archive_Water_Quality" => nameof(ArchiveWaterQualityPage),
                    "Air_Quality" => nameof(AirQualityPage),
                    _ => null
                };

                if (route != null)
                {
                    await Shell.Current.GoToAsync(route);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"No page defined for table: {SelectedTable}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Navigation Error", $"Failed to navigate: {ex.Message}", "OK");
            }
        }
    }
}
