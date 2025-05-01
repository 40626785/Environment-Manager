using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using EnvironmentManager.Services;
using Microsoft.Maui.Controls;

namespace EnvironmentManager.ViewModels
{
    public class EditArchiveAirQualityViewModel : BaseViewModel
    {
        private readonly ArchiveAirQualityDbContext _context;
        private readonly IUserDialogService _dialogService;

        public ArchiveAirQuality EditableRecord { get; set; }

        public ICommand SaveCommand { get; }

        public EditArchiveAirQualityViewModel(
            ArchiveAirQualityDbContext context,
            IUserDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
            EditableRecord = Services.NavigationDataStore.SelectedRecord;
            SaveCommand = new Command(async () => await SaveAsync());
        }

        internal async Task SaveAsync()
        {
            if (!await ValidateRecordAsync(EditableRecord))
                return;

            try
            {
                _context.ArchiveAirQuality.Update(EditableRecord);
                await _context.SaveChangesAsync();

                await _dialogService.ShowAlert("Success", "Record updated successfully.", "OK");
                await _dialogService.NavigateBackAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Failed to save: {ex.Message}", "OK");
            }
        }

        private async Task<bool> ValidateRecordAsync(ArchiveAirQuality record)
        {
            if (record == null)
            {
                await _dialogService.ShowAlert("Validation Error", "No record loaded.", "OK");
                return false;
            }

            if (record.Nitrogen_dioxide < 0 || record.Sulphur_dioxide < 0 ||
                record.PM2_5_particulate_matter < 0 || record.PM10_particulate_matter < 0)
            {
                await _dialogService.ShowAlert("Validation Error", "Pollutant values cannot be negative.", "OK");
                return false;
            }

            if (record.Date == null)
            {
                await _dialogService.ShowAlert("Validation Error", "Date is required.", "OK");
                return false;
            }

            if (record.Time == null)
            {
                await _dialogService.ShowAlert("Validation Error", "Time is required.", "OK");
                return false;
            }

            return true;
        }
    }
}
