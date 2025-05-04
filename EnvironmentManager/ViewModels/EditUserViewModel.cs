using System;
using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentManager.ViewModels
{
    public class EditUserViewModel : BaseViewModel
    {
        private readonly UserDbContext _context;
        private readonly IUserDialogService _dialogService;

        public User EditableRecord { get; set; }

        public ICommand SaveCommand { get; }

        public EditUserViewModel(UserDbContext context, IUserDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            EditableRecord = Services.NavigationDataStore.SelectedUserRecord ?? new User();
            SaveCommand = new Command(async () => await SaveAsync());
        }

        public async Task SaveAsync()
        {
            try
            {
                var existing = await _context.Users.FindAsync(EditableRecord.Username);

                if (existing != null)
                {
                    _context.Entry(existing).CurrentValues.SetValues(EditableRecord);
                }
                else
                {
                    await _context.Users.AddAsync(EditableRecord);
                }

                await _context.SaveChangesAsync();
                await _dialogService.ShowAlert("Saved", "User updated successfully.", "OK");
                await _dialogService.NavigateBackAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Save failed: {ex.Message}", "OK");
            }
        }
    }
}
