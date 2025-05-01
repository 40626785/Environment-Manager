using Xunit;
using Moq;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace EnvironmentManager.Test
{
    public class ArchiveAirQualityViewModelTests
    {
        [Fact]
        public async Task LoadDataAsync_PopulatesTableData_WhenRecordsExist()
        {
            // Arrange: create shared in-memory DB context
            var options = new DbContextOptionsBuilder<ArchiveAirQualityDbContext>()
                .UseInMemoryDatabase("ArchiveAirQualityTestDb")
                .Options;

            var context = new ArchiveAirQualityDbContext(options);

            var testData = new List<ArchiveAirQuality>
            {
                new ArchiveAirQuality
                {
                    Id = 1,
                    Date = DateTime.Today,
                    Time = TimeSpan.FromHours(9),
                    Nitrogen_dioxide = 15,
                    Sulphur_dioxide = 7,
                    PM2_5_particulate_matter = 12,
                    PM10_particulate_matter = 22
                },
                new ArchiveAirQuality
                {
                    Id = 2,
                    Date = DateTime.Today.AddDays(-1),
                    Time = TimeSpan.FromHours(10),
                    Nitrogen_dioxide = 20,
                    Sulphur_dioxide = 6,
                    PM2_5_particulate_matter = 14,
                    PM10_particulate_matter = 25
                }
            };

            await context.ArchiveAirQuality.AddRangeAsync(testData);
            await context.SaveChangesAsync();

            // Mock the factory to return the seeded context
            var mockFactory = new Mock<IDbContextFactory<ArchiveAirQualityDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(context);

            // Mock dependencies
            var mockLogger = new Mock<ILoggingService>();
            var mockDialog = new Mock<IUserDialogService>();

            // Create ViewModel
            var viewModel = new ArchiveAirQualityViewModel(
                mockFactory.Object,
                mockLogger.Object,
                mockDialog.Object
            );

            // Act
            await viewModel.LoadDataAsync();

            // Assert
            Assert.Equal(2, viewModel.TableData.Count);
            Assert.Contains(viewModel.TableData, x => x.Id == 1 && x.Nitrogen_dioxide == 15);
            Assert.Contains(viewModel.TableData, x => x.Id == 2 && x.PM10_particulate_matter == 25);
        }
    }
}
