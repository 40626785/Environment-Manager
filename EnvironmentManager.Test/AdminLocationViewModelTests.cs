using Xunit;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using EnvironmentManager.ViewModels;
using LocationModel = EnvironmentManager.Models.Location;

namespace EnvironmentManager.Test
{
    public class AdminLocationViewModelTests
    {
        private DbContextOptions<LocationDbContext> CreateOptions() =>
            new DbContextOptionsBuilder<LocationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        private async Task<LocationDbContext> SeedTestDataAsync(DbContextOptions<LocationDbContext> options)
        {
            var context = new LocationDbContext(options);
            await context.Locations.AddRangeAsync(new[]
            {
                new LocationModel { LocationId = 1, SiteName = "Central Station", Latitude = 1, Longitude = 1, Elevation = 1, SiteType = "Urban", Zone = "North", Agglomeration = "Metro", LocalAuthority = "City Council", Country = "UK", UtcOffsetSeconds = 0, Timezone = "GMT", TimezoneAbbreviation = "GMT" },
                new LocationModel { LocationId = 2, SiteName = "Parkview", Latitude = 2, Longitude = 2, Elevation = 2, SiteType = "Rural", Zone = "South", Agglomeration = "Valley", LocalAuthority = "District Council", Country = "UK", UtcOffsetSeconds = 0, Timezone = "GMT", TimezoneAbbreviation = "GMT" }
            });
            await context.SaveChangesAsync();
            return context;
        }

        [Fact]
        public async Task LoadDataAsync_LoadsLocations()
        {
            var options = CreateOptions();
            var context = await SeedTestDataAsync(options);

            var factory = new Mock<IDbContextFactory<LocationDbContext>>();
            factory.Setup(f => f.CreateDbContext()).Returns(() => new LocationDbContext(options));

            var mockDialog = new Mock<IUserDialogService>();
            var viewModel = new AdminLocationViewModel(factory.Object, mockDialog.Object);

            await viewModel.LoadDataAsync();

            Assert.Equal(2, viewModel.TableData.Count);
        }

        [Fact]
        public async Task ApplyFiltersAsync_FiltersByIdAndName()
        {
            var options = CreateOptions();
            await SeedTestDataAsync(options);

            var factory = new Mock<IDbContextFactory<LocationDbContext>>();
            factory.Setup(f => f.CreateDbContext()).Returns(() => new LocationDbContext(options));

            var mockDialog = new Mock<IUserDialogService>();
            var viewModel = new AdminLocationViewModel(factory.Object, mockDialog.Object)
            {
                StartIdText = "2",
                EndIdText = "2",
                SiteNameFilter = "Parkview"
            };

            await viewModel.ApplyFiltersAsync();

            Assert.Single(viewModel.TableData);
            Assert.Equal("Parkview", viewModel.TableData.First().SiteName);
        }

        [Fact]
        public async Task DeleteFilteredAsync_DeletesAfterConfirmation()
        {
            var options = CreateOptions();
            await SeedTestDataAsync(options);

            var factory = new Mock<IDbContextFactory<LocationDbContext>>();
            factory.Setup(f => f.CreateDbContext()).Returns(() => new LocationDbContext(options));

            var mockDialog = new Mock<IUserDialogService>();
            mockDialog.Setup(d => d.ShowConfirmation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .ReturnsAsync(true);
            mockDialog.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);

            var viewModel = new AdminLocationViewModel(factory.Object, mockDialog.Object);
            await viewModel.LoadDataAsync();
            await viewModel.DeleteFilteredAsync();

            using var verifyContext = new LocationDbContext(options);
            Assert.Empty(verifyContext.Locations);
        }

        [Fact]
        public async Task ExportToCsvAsync_CreatesFile()
        {
            var options = CreateOptions();
            await SeedTestDataAsync(options);

            var factory = new Mock<IDbContextFactory<LocationDbContext>>();
            factory.Setup(f => f.CreateDbContext()).Returns(() => new LocationDbContext(options));

            var mockDialog = new Mock<IUserDialogService>();
            mockDialog.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);

            var viewModel = new AdminLocationViewModel(factory.Object, mockDialog.Object);
            await viewModel.LoadDataAsync();

            var filePath = Path.Combine(Path.GetTempPath(), $"Locations_TestExport.csv");
            await viewModel.ExportToCsvAsync(filePath);

            Assert.True(File.Exists(filePath));
            File.Delete(filePath);
        }
    }
}
