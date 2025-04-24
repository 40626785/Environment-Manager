using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    public class ArchiveAirQualityViewModel : BaseViewModel
    {
        private readonly ArchiveAirQualityDbContext _dbContext;

        public ObservableCollection<ArchiveAirQuality> TableData { get; set; } = new ObservableCollection<ArchiveAirQuality>();

        public DateTime SelectedDate { get; set; } = DateTime.Now;

        public ICommand LoadDataCommand { get; }
        public ICommand DeleteByDateCommand { get; }

        public ArchiveAirQualityViewModel(ArchiveAirQualityDbContext dbContext)
        {
            Debug.WriteLine($"Inside ArchiveAirQualityViewModel with ArchiveAirQualityDbContext");
            _dbContext = dbContext;

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            DeleteByDateCommand = new Command(async () => await DeleteByDateAsync());

            // Auto-load data when ViewModel initializes
            Task.Run(async () => await LoadDataAsync());
        }

        private async Task LoadDataAsync()
        {
            try
            {
                TableData.Clear();

                var data = await _dbContext.ArchiveAirQuality
                                           .OrderByDescending(a => a.Date)
                                           .Take(100)
                                           .ToListAsync();

                foreach (var item in data)
                {
                    TableData.Add(item);

                }

                if (TableData.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Info", "No records found.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
            }
            Debug.WriteLine($"[DEBUG] Loaded {TableData.Count} records into TableData.");
            foreach (var item in TableData)
            {
                Debug.WriteLine($"Record: ID={item.Id}, Date={item.Date}, Time={item.Time}");
            }

        }

        private async Task DeleteByDateAsync()
        {
            try
            {
                var recordsToDelete = await _dbContext.ArchiveAirQuality
                    .Where(a => a.Date == SelectedDate.Date)
                    .ToListAsync();

                if (recordsToDelete.Any())
                {
                    _dbContext.ArchiveAirQuality.RemoveRange(recordsToDelete);
                    await _dbContext.SaveChangesAsync();

                    await Application.Current.MainPage.DisplayAlert("Success", $"Deleted {recordsToDelete.Count} records for {SelectedDate:yyyy-MM-dd}.", "OK");

                    await LoadDataAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Info", $"No records found for {SelectedDate:yyyy-MM-dd}.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Delete failed: {ex.Message}", "OK");
            }
        }
    }
}
