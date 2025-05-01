using Xunit;
using Moq;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Services;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnvironmentManager.Test
{
    public class AirQualityAdminViewModelTests
    {
        [Fact]
        public async Task LoadDataAsync_PopulatesTableData_WhenRecordsExist()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AirQualityDbContext>()
                .UseInMemoryDatabase("AirQualityTestDb_Load")
                .Options;

            var context = new AirQualityDbContext(options);

            var testData = new List<AirQualityRecord>
            {
                new AirQualityRecord
                {
                    Id = 1,
                    Date = DateTime.Today,
                    Time = TimeSpan.FromHours(8),
                    Nitrogen_dioxide = 10,
                    Sulphur_dioxide = 6,
                    PM2_5_particulate_matter = 12,
                    PM10_particulate_matter = 22
                },
                new AirQualityRecord
                {
                    Id = 2,
                    Date = DateTime.Today.AddDays(-1),
                    Time = TimeSpan.FromHours(9),
                    Nitrogen_dioxide = 14,
                    Sulphur_dioxide = 5,
                    PM2_5_particulate_matter = 10,
                    PM10_particulate_matter = 20
                }
            };

            await context.AirQuality.AddRangeAsync(testData);
            await context.SaveChangesAsync();

            var mockFactory = new Mock<IDbContextFactory<AirQualityDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(context);

            var mockLogger = new Mock<ILoggingService>();
            var mockDialog = new Mock<IUserDialogService>();

            var viewModel = new AirQualityAdminViewModel(
                mockFactory.Object,
                mockLogger.Object,
                mockDialog.Object
            );

            // Act
            await viewModel.LoadDataAsync();

            // Assert
            Assert.Equal(2, viewModel.TableData.Count);
            Assert.Contains(viewModel.TableData, x => x.Id == 1 && x.Nitrogen_dioxide == 10);
            Assert.Contains(viewModel.TableData, x => x.Id == 2 && x.PM10_particulate_matter == 20);
        }
    }
}
