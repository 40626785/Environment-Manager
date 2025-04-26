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
        public List<string> SortOptions { get; } = new()
        {
            "Date",
            "Nitrogen_dioxide",
            "PM2_5_particulate_matter"
        };

        private string selectedSortOption = "Date";
        public string SelectedSortOption
        {
            get => selectedSortOption;
            set => SetProperty(ref selectedSortOption, value);
        }

        public ICommand ApplySortCommand { get; }
        public DateTime SelectedDate { get; set; } = DateTime.Now;

        public ICommand LoadDataCommand { get; }
        public ICommand DeleteByDateCommand { get; }

        public ArchiveAirQualityViewModel(ArchiveAirQualityDbContext dbContext)
        {

            _dbContext = dbContext;

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            FilterByDateRangeCommand = new Command(async () => await FilterByDateRangeAsync());
            ApplySortCommand = new Command(async () => await ApplySortAsync());
            DeleteByDateCommand = new Command(async () => await DeleteByDateAsync());

            // Auto-load data when ViewModel initializes
            Task.Run(async () => await LoadDataAsync());
        }
        private async Task FilterByDateRangeAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                TableData.Clear();

                var data = await _dbContext.ArchiveAirQuality
                    .Where(a => a.Date >= StartDate && a.Date <= EndDate)
                    .OrderBy(a => a.Date)
                    .ToListAsync();

                foreach (var item in data)
                {
                    TableData.Add(item);
                }

                if (TableData.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Info", "No records found in selected date range.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Filter failed: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

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
            finally
            {
                IsBusy = false;
            }
        }

        private DateTime startDate = DateTime.Now.AddDays(-7);  // Default to past week
        public DateTime StartDate
        {
            get => startDate;
            set => SetProperty(ref startDate, value);
        }

        private DateTime endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => endDate;
            set => SetProperty(ref endDate, value);
        }

        public ICommand FilterByDateRangeCommand { get; }

        private async Task ApplySortAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                TableData.Clear();

                IQueryable<ArchiveAirQuality> query = _dbContext.ArchiveAirQuality;

                query = SelectedSortOption switch
                {
                    "Nitrogen_dioxide" => query.OrderByDescending(a => a.Nitrogen_dioxide),
                    "PM2_5_particulate_matter" => query.OrderByDescending(a => a.PM2_5_particulate_matter),
                    _ => query.OrderByDescending(a => a.Date)
                };

                var data = await query.Take(100).ToListAsync();

                foreach (var item in data)
                {
                    TableData.Add(item);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Sort failed: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
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
