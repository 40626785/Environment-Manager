using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    public class EditArchiveAirQualityViewModel : BaseViewModel
    {
        private readonly ArchiveAirQualityDbContext _dbContext;

        public ArchiveAirQuality EditableRecord { get; set; }

        public ICommand SaveCommand { get; }

        public EditArchiveAirQualityViewModel(ArchiveAirQualityDbContext dbContext)
        {
            _dbContext = dbContext;
            SaveCommand = new Command(async () => await SaveAsync());

            // Load the record from the shared service
            EditableRecord = Services.NavigationDataStore.SelectedRecord;
        }

        private async Task SaveAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                _dbContext.ArchiveAirQuality.Update(EditableRecord);
                await _dbContext.SaveChangesAsync();

                await Application.Current.MainPage.DisplayAlert("Success", "Record updated.", "OK");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

}
