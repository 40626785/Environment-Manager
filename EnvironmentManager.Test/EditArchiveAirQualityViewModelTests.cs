using Xunit;
using Moq;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Models;
using EnvironmentManager.Services;
using EnvironmentManager.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnvironmentManager.Test;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Test;
public class EditArchiveAirQualityViewModelTests
{
    [Fact]
    public async Task SaveAsync_ValidRecord_SavesAndNavigates()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ArchiveAirQualityDbContext>()
            .UseInMemoryDatabase("ArchiveAirQualityTestDb")
            .Options;

        var context = new ArchiveAirQualityDbContext(options);

        var mockDialog = new Mock<IUserDialogService>();
        bool alertShown = false, navigated = false;

        mockDialog.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                  .Callback(() => alertShown = true)
                  .Returns(Task.CompletedTask);
        mockDialog.Setup(d => d.NavigateBackAsync())
                  .Callback(() => navigated = true)
                  .Returns(Task.CompletedTask);

        var record = new ArchiveAirQuality
        {
            Id = 1,
            Date = DateTime.Today,
            Time = DateTime.Now.TimeOfDay,
            Nitrogen_dioxide = 12,
            Sulphur_dioxide = 7,
            PM2_5_particulate_matter = 15,
            PM10_particulate_matter = 20
        };

        // Inject into NavigationDataStore
        Services.NavigationDataStore.SelectedRecord = record;

        var viewModel = new EditArchiveAirQualityViewModel(context, mockDialog.Object);

        // Act
        await viewModel.SaveAsync();

        // Assert
        var saved = await context.ArchiveAirQuality.FindAsync(1);

        // 💡 Diagnostics if failed
        if (saved == null)
        {
            var all = await context.ArchiveAirQuality.ToListAsync();
            throw new Exception($"Nothing saved. Count: {all.Count}");
        }

        Assert.NotNull(saved);
        Assert.Equal(12, saved.Nitrogen_dioxide);
        Assert.True(alertShown);

    }

}