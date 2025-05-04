using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Views;
using LocationModel = EnvironmentManager.Models.Location;
using System.Diagnostics;

namespace EnvironmentManager.ViewModels
{
    public class AdminLocationViewModel : BaseViewModel
    {
        private readonly IDbContextFactory<LocationDbContext> _dbContextFactory;
        private readonly IUserDialogService _dialogService;

        public ObservableCollection<LocationModel> TableData { get; } = new();

        public ICommand LoadDataCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand DeleteFilteredCommand { get; }
        public ICommand ExportToCsvCommand { get; }
        public ICommand RowTappedCommand { get; }
        public ICommand ToggleFilterVisibilityCommand { get; }

        public string StartIdText { get; set; } = string.Empty;
        public string EndIdText { get; set; } = string.Empty;
        public string SiteNameFilter { get; set; } = string.Empty;

        private bool isFilterVisible = false;
        public bool IsFilterVisible
        {
            get => isFilterVisible;
            set
            {
                SetProperty(ref isFilterVisible, value);
                OnPropertyChanged(nameof(ToggleFilterText));
            }
        }

        public string ToggleFilterText => IsFilterVisible ? "Hide Filters ▲" : "Show Filters ▼";

        public AdminLocationViewModel(IDbContextFactory<LocationDbContext> dbContextFactory, IUserDialogService dialogService)
        {
            _dbContextFactory = dbContextFactory;
            _dialogService = dialogService;

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            ApplyFiltersCommand = new Command(async () => await ApplyFiltersAsync());
            DeleteFilteredCommand = new Command(async () => await DeleteFilteredAsync());
            ExportToCsvCommand = new Command(async () => await ExportToCsvAsync());
            RowTappedCommand = new Command<LocationModel>(async (loc) => await OnRowTapped(loc));
            ToggleFilterVisibilityCommand = new Command(() => IsFilterVisible = !IsFilterVisible);
        }

        public async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                TableData.Clear();

                using var context = _dbContextFactory.CreateDbContext();
                var data = await context.Locations
                    .OrderByDescending(l => l.LocationId)
                    .Take(100)
                    .ToListAsync();

                foreach (var item in data)
                    TableData.Add(item);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Failed to load locations: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        internal async Task ApplyFiltersAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                TableData.Clear();

                using var context = _dbContextFactory.CreateDbContext();
                IQueryable<LocationModel> query = context.Locations;

                if (!string.IsNullOrWhiteSpace(SiteNameFilter))
                    query = query.Where(l => l.SiteName.Contains(SiteNameFilter));

                if (int.TryParse(StartIdText, out int startId) &&
                    int.TryParse(EndIdText, out int endId) &&
                    startId <= endId)
                {
                    query = query.Where(l => l.LocationId >= startId && l.LocationId <= endId);
                }

                var results = await query.OrderByDescending(l => l.LocationId).Take(100).ToListAsync();
                foreach (var loc in results)
                    TableData.Add(loc);

                if (!TableData.Any())
                    await _dialogService.ShowAlert("No Results", "No locations match your filters.", "OK");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Filter failed: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        internal async Task DeleteFilteredAsync()
        {
            if (IsBusy || !TableData.Any()) return;

            bool confirm = await _dialogService.ShowConfirmation("Confirm Deletion", $"Delete {TableData.Count} location(s)?", "Yes", "Cancel");
            if (!confirm) return;

            try
            {
                IsBusy = true;

                using var context = _dbContextFactory.CreateDbContext();
                context.Locations.RemoveRange(TableData);
                await context.SaveChangesAsync();

                await _dialogService.ShowAlert("Deleted", $"{TableData.Count} location(s) removed.", "OK");
                TableData.Clear();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Delete failed: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExportToCsvAsync(string outputPath = null)
        {
            if (!TableData.Any())
            {
                await _dialogService.ShowAlert("No Data", "There is no data to export.", "OK");
                return;
            }

            try
            {
                var lines = new List<string>
                {
                    "LocationId,SiteName,Latitude,Longitude,Elevation,SiteType,Zone,Agglomeration,LocalAuthority,Country,UtcOffsetSeconds,Timezone,TimezoneAbbreviation"
                };

                lines.AddRange(TableData.Select(loc =>
                    $"{loc.LocationId},{loc.SiteName},{loc.Latitude},{loc.Longitude},{loc.Elevation},{loc.SiteType},{loc.Zone},{loc.Agglomeration},{loc.LocalAuthority},{loc.Country},{loc.UtcOffsetSeconds},{loc.Timezone},{loc.TimezoneAbbreviation}"
                ));

                var fileName = $"Locations_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = outputPath ?? Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

                await File.WriteAllLinesAsync(filePath, lines);
                await _dialogService.ShowAlert("Exported", $"CSV saved to:\n{filePath}", "OK");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Export failed: {ex.Message}", "OK");
            }
        }

        private async Task OnRowTapped(LocationModel record)
        {
            if (record == null) return;

            Debug.WriteLine($"Tapped record: {record?.LocationId} - {record?.SiteName}");
            await _dialogService.ShowAlert("Tapped", $"Editing {record.SiteName}", "OK");
            Services.NavigationDataStore.SelectedLocationRecord = record;
            await Shell.Current.GoToAsync(nameof(EditLocationPage));
        }
    }
}
