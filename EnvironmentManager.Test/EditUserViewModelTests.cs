using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Models;
using EnvironmentManager.Data;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Test
{
    public class EditUserViewModelTests
    {
        private DbContextOptions<UserDbContext> CreateOptions() =>
            new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: $"UserDb_{System.Guid.NewGuid()}")
                .Options;

        [Fact]
        public async Task SaveAsync_UpdatesExistingUser()
        {
            // Arrange
            var options = CreateOptions();
            using (var context = new UserDbContext(options))
            {
                context.Users.Add(new User { Username = "admin", Password = "old", Role = 1 });
                context.SaveChanges();
            }

            var dialogMock = new Mock<IUserDialogService>();
            bool alertShown = false, navigated = false;

            dialogMock.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Callback(() => alertShown = true)
                      .Returns(Task.CompletedTask);
            dialogMock.Setup(d => d.NavigateBackAsync())
                      .Callback(() => navigated = true)
                      .Returns(Task.CompletedTask);

            Services.NavigationDataStore.SelectedUserRecord = new User
            {
                Username = "admin",
                Password = "newpass",
                Role = 2
            };

            using var updateContext = new UserDbContext(options);
            var viewModel = new EditUserViewModel(updateContext, dialogMock.Object);

            // Act
            await viewModel.SaveAsync();

            // Assert
            var updated = await updateContext.Users.FindAsync("admin");
            Assert.Equal("newpass", updated?.Password);
            Assert.Equal(2, updated?.Role);
            Assert.True(alertShown);
            Assert.True(navigated);
        }

        [Fact]
        public async Task SaveAsync_InsertsNewUser_WhenNotFound()
        {
            // Arrange
            var options = CreateOptions();
            var dialogMock = new Mock<IUserDialogService>();

            bool alertShown = false;
            dialogMock.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Callback(() => alertShown = true)
                      .Returns(Task.CompletedTask);

            Services.NavigationDataStore.SelectedUserRecord = new User
            {
                Username = "newuser",
                Password = "secret",
                Role = 0
            };

            using var context = new UserDbContext(options);
            var viewModel = new EditUserViewModel(context, dialogMock.Object);

            // Act
            await viewModel.SaveAsync();

            // Assert
            var added = await context.Users.FindAsync("newuser");
            Assert.NotNull(added);
            Assert.Equal("secret", added.Password);
            Assert.True(alertShown);
        }
    }
}
