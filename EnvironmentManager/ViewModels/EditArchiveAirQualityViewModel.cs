using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    public class EditArchiveAirQualityViewModel : BaseViewModel
    {
        private readonly IDbContextFactory<ArchiveAirQualityDbContext> _dbContextFactory;

        // Record bound to the UI
        public ArchiveAirQuality EditableRecord { get; set; }

        public ICommand SaveCommand { get; }

        public EditArchiveAirQualityViewModel(IDbContextFactory<ArchiveAirQualityDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            SaveCommand = new Command(async () => await SaveAsync());

            // Load record from shared service
            EditableRecord = Services.NavigationDataStore.SelectedRecord;
        }

        private async Task SaveAsync()
        {
            if (!await ValidateRecordAsync(EditableRecord))
                return;

            try
            {
                using var context = _dbContextFactory.CreateDbContext();

                context.ArchiveAirQuality.Update(EditableRecord);
                await context.SaveChangesAsync();

                await Application.Current.MainPage.DisplayAlert("Success", "Record updated successfully.", "OK");
                await Shell.Current.GoToAsync("..");  // Navigate back
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save: {ex.Message}", "OK");
                // Optionally log error here
            }
        }

        private async Task<bool> ValidateRecordAsync(ArchiveAirQuality record)
        {
            if (record == null)
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "No record loaded.", "OK");
                return false;
            }

            if (record.Nitrogen_dioxide < 0 || record.Sulphur_dioxide < 0 ||
                record.PM2_5_particulate_matter < 0 || record.PM10_particulate_matter < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Pollutant values cannot be negative.", "OK");
                return false;
            }

            if (record.Date == null)
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Date is required.", "OK");
                return false;
            }

            if (record.Time == null)
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Time is required.", "OK");
                return false;
            }

            return true;
        }
    }
}
