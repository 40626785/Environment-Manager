using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using EnvironmentManager.Views;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    public class AdminUserViewModel : BaseViewModel
    {
        private readonly IDbContextFactory<UserDbContext> _dbContextFactory;
        private readonly IUserDialogService _dialogService;

        public ObservableCollection<User> TableData { get; set; } = new();

        public string UsernameFilter { get; set; } = string.Empty;
        public string RoleFilterText { get; set; } = string.Empty;

        public ICommand LoadDataCommand { get; }
        public ICommand ApplyFiltersCommand { get; }
        public ICommand DeleteFilteredCommand { get; }
        public ICommand ExportToCsvCommand { get; }
        public ICommand ToggleFilterVisibilityCommand { get; }
        public ICommand RowTappedCommand { get; }
        public ICommand AddUserCommand { get; }

        public bool IsFilterVisible { get; set; }
        public string ToggleFilterText => IsFilterVisible ? "Hide Filters ▲" : "Show Filters ▼";

        public AdminUserViewModel(IDbContextFactory<UserDbContext> dbContextFactory, IUserDialogService dialogService)
        {
            _dbContextFactory = dbContextFactory;
            _dialogService = dialogService;

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            ApplyFiltersCommand = new Command(async () => await ApplyFiltersAsync());
            DeleteFilteredCommand = new Command(async () => await DeleteFilteredAsync());
            ExportToCsvCommand = new Command(async () => await ExportToCsvAsync());
            ToggleFilterVisibilityCommand = new Command(() =>
            {
                IsFilterVisible = !IsFilterVisible;
                OnPropertyChanged(nameof(IsFilterVisible));
                OnPropertyChanged(nameof(ToggleFilterText));
            });
            RowTappedCommand = new Command<User>(async (user) => await OnRowTapped(user));
            AddUserCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(AddUserPage)));

            Task.Run(LoadDataAsync);
        }

        public async Task LoadDataAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                TableData.Clear();

                using var context = _dbContextFactory.CreateDbContext();
                var users = await context.Users.OrderBy(u => u.Username).ToListAsync();
                foreach (var user in users)
                    TableData.Add(user);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Failed to load users: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ApplyFiltersAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                TableData.Clear();

                using var context = _dbContextFactory.CreateDbContext();
                var query = context.Users.AsQueryable();

                if (!string.IsNullOrWhiteSpace(UsernameFilter))
                    query = query.Where(u => u.Username.Contains(UsernameFilter));

                if (int.TryParse(RoleFilterText, out int role))
                    query = query.Where(u => u.Role == role);

                var filtered = await query.ToListAsync();
                foreach (var user in filtered)
                    TableData.Add(user);

                if (!TableData.Any())
                    await _dialogService.ShowAlert("Info", "No users match the filters.", "OK");
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

        public async Task DeleteFilteredAsync()
        {
            if (IsBusy || !TableData.Any()) return;

            bool confirm = await _dialogService.ShowConfirmation("Confirm Delete", $"Delete {TableData.Count} user(s)?", "Yes", "Cancel");
            if (!confirm) return;

            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                context.Users.RemoveRange(TableData);
                await context.SaveChangesAsync();

                await _dialogService.ShowAlert("Deleted", $"{TableData.Count} users deleted.", "OK");
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Delete failed: {ex.Message}", "OK");
            }
        }

        public async Task ExportToCsvAsync(string? overridePath = null)
        {
            if (!TableData.Any())
            {
                await _dialogService.ShowAlert("Export", "No users to export.", "OK");
                return;
            }

            try
            {
                var csvLines = new List<string> { "Username,Password,Role" };
                foreach (var user in TableData)
                    csvLines.Add($"{user.Username},{user.Password},{user.Role}");

                var fileName = $"Users_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = overridePath ?? Path.Combine(FileSystem.AppDataDirectory, fileName);

                await File.WriteAllLinesAsync(filePath, csvLines);
                await _dialogService.ShowAlert("Exported", $"CSV saved to:\n{filePath}", "OK");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Export failed: {ex.Message}", "OK");
            }
        }

        private async Task OnRowTapped(User user)
        {
            if (user == null) return;

            Services.NavigationDataStore.SelectedUserRecord = user;
            await Shell.Current.GoToAsync(nameof(EditUserPage));
        }
    }
}
