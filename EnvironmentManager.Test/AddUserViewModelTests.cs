using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Test
{
    public class AddUserViewModelTests
    {
        private DbContextOptions<UserDbContext> CreateOptions() =>
            new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase($"UserDb_{System.Guid.NewGuid()}")
                .Options;

        [Fact]
        public async Task SaveAsync_CreatesNewUser_WhenValid()
        {
            var options = CreateOptions();
            var dialogMock = new Mock<IUserDialogService>();

            bool alertShown = false, navigated = false;

            dialogMock.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Callback(() => alertShown = true)
                      .Returns(Task.CompletedTask);
            dialogMock.Setup(d => d.NavigateBackAsync())
                      .Callback(() => navigated = true)
                      .Returns(Task.CompletedTask);

            using var context = new UserDbContext(options);
            var viewModel = new AddUserViewModel(context, dialogMock.Object)
            {
                NewUser = new User { Username = "testuser", Password = "pass123", Role = 1 }
            };

            await viewModel.SaveAsync();

            var user = await context.Users.FindAsync("testuser");
            Assert.NotNull(user);
            Assert.Equal(1, user.Role);
            Assert.True(alertShown);
            Assert.True(navigated);
        }

        [Fact]
        public async Task SaveAsync_ShowsAlert_WhenUsernameOrPasswordMissing()
        {
            var options = CreateOptions();
            var dialogMock = new Mock<IUserDialogService>();
            bool alertShown = false;

            dialogMock.Setup(d => d.ShowAlert("Validation", It.IsAny<string>(), "OK"))
                      .Callback(() => alertShown = true)
                      .Returns(Task.CompletedTask);

            using var context = new UserDbContext(options);
            var viewModel = new AddUserViewModel(context, dialogMock.Object)
            {
                NewUser = new User { Username = "", Password = "", Role = 0 }
            };

            await viewModel.SaveAsync();

            Assert.True(alertShown);
            Assert.Equal(0, await context.Users.CountAsync());
        }

        [Fact]
        public async Task SaveAsync_ShowsDuplicateAlert_IfUsernameExists()
        {
            var options = CreateOptions();

            using (var setup = new UserDbContext(options))
            {
                await setup.Users.AddAsync(new User { Username = "existing", Password = "x", Role = 0 });
                await setup.SaveChangesAsync();
            }

            var dialogMock = new Mock<IUserDialogService>();
            bool alertShown = false;

            dialogMock.Setup(d => d.ShowAlert("Duplicate", It.IsAny<string>(), "OK"))
                      .Callback(() => alertShown = true)
                      .Returns(Task.CompletedTask);

            using var context = new UserDbContext(options);
            var viewModel = new AddUserViewModel(context, dialogMock.Object)
            {
                NewUser = new User { Username = "existing", Password = "x", Role = 0 }
            };

            await viewModel.SaveAsync();

            Assert.True(alertShown);
            Assert.Equal(1, await context.Users.CountAsync()); // No new user added
        }
    }
}

