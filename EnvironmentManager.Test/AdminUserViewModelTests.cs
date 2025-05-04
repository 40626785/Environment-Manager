using Xunit;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Test
{
    public class AdminUserViewModelTests
    {
        private DbContextOptions<UserDbContext> CreateOptions() =>
    new DbContextOptionsBuilder<UserDbContext>()
        .UseInMemoryDatabase(databaseName: $"UserDb_{Guid.NewGuid()}")
        .Options;


        private async Task<UserDbContext> SeedTestDataAsync(DbContextOptions<UserDbContext> options)
        {
            var context = new UserDbContext(options);
            await context.Users.AddRangeAsync(new[]
            {
                new User { Username = "admin", Password = "pass1", Role = 1 },
                new User { Username = "guest", Password = "pass2", Role = 0 },
                new User { Username = "super", Password = "pass3", Role = 2 }
            });
            await context.SaveChangesAsync();
            return context;
        }


        [Fact]
        public async Task LoadDataAsync_LoadsAllUsers()
        {
            var options = CreateOptions();

            var factory = new Mock<IDbContextFactory<UserDbContext>>();
            factory.Setup(f => f.CreateDbContext()).Returns(() =>
            {
                var ctx = new UserDbContext(options);
                if (!ctx.Users.Any())
                {
                    ctx.Users.AddRange(
                        new User { Username = "admin", Password = "pass1", Role = 1 },
                        new User { Username = "guest", Password = "pass2", Role = 0 },
                        new User { Username = "super", Password = "pass3", Role = 2 }
                    );
                    ctx.SaveChanges();
                }
                return new UserDbContext(options);
            });

            var mockDialog = new Mock<IUserDialogService>();
            var viewModel = new AdminUserViewModel(factory.Object, mockDialog.Object);

            await viewModel.LoadDataAsync();

            Assert.Equal(3, viewModel.TableData.Count);
        }


        [Fact]
        public async Task ApplyFiltersAsync_ByUsernameAndRole()
        {
            var options = CreateOptions();
            await SeedTestDataAsync(options);

            var factory = new Mock<IDbContextFactory<UserDbContext>>();
            factory.Setup(f => f.CreateDbContext()).Returns(() => new UserDbContext(options));

            var dialogMock = new Mock<IUserDialogService>();
            var viewModel = new AdminUserViewModel(factory.Object, dialogMock.Object)
            {
                UsernameFilter = "admin",
                RoleFilterText = "1"
            };

            await viewModel.ApplyFiltersAsync();

            Assert.Single(viewModel.TableData);
            Assert.Equal("admin", viewModel.TableData.First().Username);
        }

        [Fact]
        public async Task DeleteFilteredAsync_RemovesUsers_IfConfirmed()
        {
            var options = CreateOptions();
            await SeedTestDataAsync(options);

            var factory = new Mock<IDbContextFactory<UserDbContext>>();
            factory.Setup(f => f.CreateDbContext()).Returns(() => new UserDbContext(options));

            var dialogMock = new Mock<IUserDialogService>();
            dialogMock.Setup(d => d.ShowConfirmation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .ReturnsAsync(true);
            dialogMock.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);

            var viewModel = new AdminUserViewModel(factory.Object, dialogMock.Object);
            await viewModel.LoadDataAsync();

            Assert.Equal(3, viewModel.TableData.Count);

            await viewModel.DeleteFilteredAsync();

            using var verify = new UserDbContext(options);
            Assert.Empty(verify.Users);
        }

        [Fact]
        public async Task ExportToCsvAsync_CreatesCsvFile()
        {
            var options = CreateOptions();
            var context = await SeedTestDataAsync(options);

            var factory = new Mock<IDbContextFactory<UserDbContext>>();
            factory.Setup(f => f.CreateDbContext()).Returns(() => new UserDbContext(options));

            var dialogMock = new Mock<IUserDialogService>();
            dialogMock.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);

            var viewModel = new AdminUserViewModel(factory.Object, dialogMock.Object);
            await viewModel.LoadDataAsync();

            var tempPath = Path.Combine(Path.GetTempPath(), $"TestExport_Users_{Guid.NewGuid()}.csv");
            await viewModel.ExportToCsvAsync(tempPath);

            Assert.True(File.Exists(tempPath));
            File.Delete(tempPath);
        }

    }
}
