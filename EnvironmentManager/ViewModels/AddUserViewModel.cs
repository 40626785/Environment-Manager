using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;

namespace EnvironmentManager.ViewModels
{
    public class AddUserViewModel : BaseViewModel
    {
        private readonly UserDbContext _context;
        private readonly IUserDialogService _dialogService;

        public User NewUser { get; set; } = new User();

        public List<int> RoleOptions { get; } = new() { 0, 1, 2 };
        public ICommand SaveCommand { get; }

        public AddUserViewModel(UserDbContext context, IUserDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
            SaveCommand = new Command(async () => await SaveAsync());
        }

        public async Task SaveAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewUser.Username) || string.IsNullOrWhiteSpace(NewUser.Password))
                {
                    await _dialogService.ShowAlert("Validation", "Username and password are required.", "OK");
                    return;
                }

                var exists = await _context.Users.FindAsync(NewUser.Username);
                if (exists != null)
                {
                    await _dialogService.ShowAlert("Duplicate", "Username already exists.", "OK");
                    return;
                }

                await _context.Users.AddAsync(NewUser);
                await _context.SaveChangesAsync();

                await _dialogService.ShowAlert("Success", "User added successfully.", "OK");
                await _dialogService.NavigateBackAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlert("Error", $"Failed to save user: {ex.Message}", "OK");
            }
        }
    }
}
