using Xunit;
using Moq;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Services;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EnvironmentManager.Test
{
    public class EditAirQualityViewModelTests
    {
        [Fact]
        public async Task SaveAsync_ValidRecord_SavesAndNavigates()
        {
            // Arrange: Set up in-memory database
            var options = new DbContextOptionsBuilder<AirQualityDbContext>()
                .UseInMemoryDatabase("AirQualityTestDb_Save")
                .Options;

            var context = new AirQualityDbContext(options);

            // Mock dialog service
            var mockDialog = new Mock<IUserDialogService>();
            bool alertShown = false;
            bool navigated = false;

            mockDialog.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Callback(() => alertShown = true)
                      .Returns(Task.CompletedTask);

            mockDialog.Setup(d => d.NavigateBackAsync())
                      .Callback(() => navigated = true)
                      .Returns(Task.CompletedTask);

            // Create a valid AirQualityRecord
            var record = new AirQualityRecord
            {
                Id = 1,
                Date = DateTime.Today,
                Time = TimeSpan.FromHours(9),
                Nitrogen_dioxide = 12,
                Sulphur_dioxide = 6,
                PM2_5_particulate_matter = 18,
                PM10_particulate_matter = 22
            };

            // ✅ Add record to the in-memory DB so EF tracks it
            await context.AirQuality.AddAsync(record);
            await context.SaveChangesAsync();

            // Inject record into NavigationDataStore
            NavigationDataStore.SelectedAirQualityRecord = record;

            // Create the ViewModel
            var viewModel = new EditAirQualityViewModel(context, mockDialog.Object);

            // Act
            await viewModel.SaveAsync();

            // Assert
            var saved = await context.AirQuality.FindAsync(1);
            Assert.NotNull(saved);
            Assert.Equal(12, saved.Nitrogen_dioxide);
            Assert.Equal(22, saved.PM10_particulate_matter);
            Assert.True(alertShown, "Expected ShowAlert to be called.");
            Assert.True(navigated, "Expected NavigateBackAsync to be called.");
        }
    }
}
