using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using LocationModel = EnvironmentManager.Models.Location;
using System.Diagnostics; // Alias to fix ambiguity

namespace EnvironmentManager.ViewModels
{
    public class EditLocationViewModel : BaseViewModel
    {
        private readonly LocationDbContext _context;
        private readonly IUserDialogService _dialogService;

        public LocationModel EditableRecord { get; set; }

        public ICommand SaveCommand { get; }

        public EditLocationViewModel(LocationDbContext context, IUserDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            EditableRecord = Services.NavigationDataStore.SelectedLocationRecord ?? new LocationModel();
            Debug.WriteLine($"Editing Location: {EditableRecord.SiteName}");
            SaveCommand = new Command(async () => await SaveAsync());
        }

        public async Task SaveAsync()
        {
            try
            {
                var existing = await _context.Locations.FindAsync(EditableRecord.LocationId);

                if (existing != null)
                {
                    _context.Entry(existing).CurrentValues.SetValues(EditableRecord);
                }
                else
                {
                    await _context.Locations.AddAsync(EditableRecord);
                }

                await _context.SaveChangesAsync();
                await _dialogService.ShowAlert("Saved", "Location updated successfully.", "OK");
                await _dialogService.NavigateBackAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Failed to save changes: {ex.Message}", "OK");
            }
        }
    }
}
