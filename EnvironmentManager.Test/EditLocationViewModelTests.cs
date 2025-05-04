using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Interfaces;
using LocationModel = EnvironmentManager.Models.Location;

namespace EnvironmentManager.Test
{
    public class EditLocationViewModelTests
    {
        private DbContextOptions<LocationDbContext> CreateOptions() =>
            new DbContextOptionsBuilder<LocationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        [Fact]
        public async Task SaveAsync_UpdatesExistingLocation()
        {
            // Arrange
            var options = CreateOptions();
            var context = new LocationDbContext(options);

            var existing = new LocationModel
            {
                LocationId = 1,
                SiteName = "OldName",
                Latitude = 1,
                Longitude = 1,
                Elevation = 1,
                SiteType = "Urban",
                Zone = "ZoneA",
                Agglomeration = "Metro",
                LocalAuthority = "Council A",
                Country = "UK",
                UtcOffsetSeconds = 0,
                Timezone = "GMT",
                TimezoneAbbreviation = "GMT"
            };

            await context.Locations.AddAsync(existing);
            await context.SaveChangesAsync();

            // Inject into navigation
            Services.NavigationDataStore.SelectedLocationRecord = new LocationModel
            {
                LocationId = 1,
                SiteName = "NewName",
                Latitude = 5,
                Longitude = 5,
                Elevation = 10,
                SiteType = "Urban",
                Zone = "ZoneA",
                Agglomeration = "Metro",
                LocalAuthority = "Council A",
                Country = "UK",
                UtcOffsetSeconds = 0,
                Timezone = "GMT",
                TimezoneAbbreviation = "GMT"
            };

            var mockDialog = new Mock<IUserDialogService>();
            bool alertCalled = false, navCalled = false;
            mockDialog.Setup(x => x.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Callback(() => alertCalled = true)
                      .Returns(Task.CompletedTask);
            mockDialog.Setup(x => x.NavigateBackAsync())
                      .Callback(() => navCalled = true)
                      .Returns(Task.CompletedTask);

            var viewModel = new EditLocationViewModel(context, mockDialog.Object);

            // Act
            await viewModel.SaveAsync();

            // Assert
            var updated = await context.Locations.FindAsync(1);
            Assert.Equal("NewName", updated?.SiteName);
            Assert.True(alertCalled);
            Assert.True(navCalled);
        }

        [Fact]
        public async Task SaveAsync_AddsNewLocation()
        {
            var options = CreateOptions();
            var context = new LocationDbContext(options);

            Services.NavigationDataStore.SelectedLocationRecord = new LocationModel
            {
                LocationId = 100,
                SiteName = "NewSite",
                Latitude = 10,
                Longitude = 20,
                Elevation = 15,
                SiteType = "Rural",
                Zone = "East",
                Agglomeration = "Town",
                LocalAuthority = "LA",
                Country = "UK",
                UtcOffsetSeconds = 0,
                Timezone = "GMT",
                TimezoneAbbreviation = "GMT"
            };

            var mockDialog = new Mock<IUserDialogService>();
            mockDialog.Setup(x => x.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);
            mockDialog.Setup(x => x.NavigateBackAsync())
                      .Returns(Task.CompletedTask);

            var viewModel = new EditLocationViewModel(context, mockDialog.Object);
            await viewModel.SaveAsync();

            var saved = await context.Locations.FindAsync(100);
            Assert.NotNull(saved);
            Assert.Equal("NewSite", saved.SiteName);
        }

        [Fact]
        public async Task SaveAsync_ShowsError_OnException()
        {
            // Arrange: broken context
            var mockContext = new Mock<LocationDbContext>();
            mockContext.Setup(c => c.Locations.FindAsync(It.IsAny<int>()))
                       .ThrowsAsync(new Exception("Test DB failure"));

            var mockDialog = new Mock<IUserDialogService>();
            bool alertShown = false;

            mockDialog.Setup(x => x.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Callback(() => alertShown = true)
                      .Returns(Task.CompletedTask);

            Services.NavigationDataStore.SelectedLocationRecord = new LocationModel
            {
                LocationId = 1,
                SiteName = "Failing Site",
                Latitude = 0,
                Longitude = 0,
                Elevation = 0,
                SiteType = "X",
                Zone = "X",
                Agglomeration = "X",
                LocalAuthority = "X",
                Country = "X",
                UtcOffsetSeconds = 0,
                Timezone = "X",
                TimezoneAbbreviation = "X"
            };

            var viewModel = new EditLocationViewModel(mockContext.Object, mockDialog.Object);

            // Act
            await viewModel.SaveAsync();

            // Assert
            Assert.True(alertShown);
        }
    }
}

