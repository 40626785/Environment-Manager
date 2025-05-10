using Moq;
using Xunit;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.Tests
{
    public class HistoricalDataViewerViewModelTests
    {
        private readonly Mock<IDbContextFactory<HistoricalDataDbContext>> _dbContextFactoryMock;
        private readonly Mock<ILoggingService> _loggingServiceMock;
        private readonly HistoricalDataViewerViewModel _viewModel;
        private readonly Mock<HistoricalDataDbContext> _dbContextMock;

        public HistoricalDataViewerViewModelTests()
        {
            // Mock the DbContextFactory and DbContext
            _dbContextFactoryMock = new Mock<IDbContextFactory<HistoricalDataDbContext>>();
            _loggingServiceMock = new Mock<ILoggingService>();
            _dbContextMock = new Mock<HistoricalDataDbContext>();

            // Setup mock for creating DbContext
            _dbContextFactoryMock.Setup(f => f.CreateDbContext()).Returns(_dbContextMock.Object);

            // Initialize ViewModel with the mocked DbContext
            _viewModel = new HistoricalDataViewerViewModel(_dbContextMock.Object, _loggingServiceMock.Object);
        }

        [Fact]
        public async Task LoadAirQualityDataAsync_ShouldLoadDataCorrectly()
        {
            // Arrange: Setup mocked data
            var airQualityData = new List<ArchiveAirQuality>
            {
                new ArchiveAirQuality { Id = 1, Date = DateTime.Now, Nitrogen_dioxide = 5.5 },
                new ArchiveAirQuality { Id = 2, Date = DateTime.Now.AddDays(-1), Nitrogen_dioxide = 7.5 }
            };

            // Mock the DbContext to return the mocked data
            _dbContextMock.Setup(x => x.Set<ArchiveAirQuality>().AsQueryable())
                .Returns(airQualityData.AsQueryable());

            // Act: Call LoadAirQualityDataAsync
            await _viewModel.LoadAirQualityDataAsync();

            // Assert: Verify that the data was loaded correctly
            Assert.Equal(2, _viewModel.AirQualityData.Count);  // Ensure there are 2 records loaded
            Assert.Equal(1, _viewModel.AirQualityData[0].Id);  // Ensure the first record has Id = 1
            Assert.Equal(2, _viewModel.AirQualityData[1].Id);  // Ensure the second record has Id = 2
        }

        [Fact]
        public async Task ApplyAirQualityFilterAsync_ShouldApplyYearFilter()
        {
            // Arrange
            _viewModel.SelectedYear = DateTime.Now.Year;
            _viewModel.SelectedMonth = "All"; // Apply filter for the current year

            var airQualityData = new List<ArchiveAirQuality>
            {
                new ArchiveAirQuality { Id = 1, Date = DateTime.Now, Nitrogen_dioxide = 5.5 },
                new ArchiveAirQuality { Id = 2, Date = DateTime.Now.AddYears(-1), Nitrogen_dioxide = 7.5 }
            };

            _dbContextMock.Setup(x => x.Set<ArchiveAirQuality>().AsQueryable())
                .Returns(airQualityData.AsQueryable());

            // Act
            await _viewModel.ApplyAirQualityFilterAsync();

            // Assert
            Assert.Equal(1, _viewModel.AirQualityData.Count); // Only 1 record should match the current year filter
            Assert.Equal(1, _viewModel.AirQualityData[0].Id); // Ensure the correct record is returned
        }

        [Fact]
        public async Task ApplyAirQualityFilterAsync_ShouldApplyMonthFilter()
        {
            // Arrange
            _viewModel.SelectedYear = DateTime.Now.Year;
            _viewModel.SelectedMonth = "January"; // Apply specific month filter

            var airQualityData = new List<ArchiveAirQuality>
            {
                new ArchiveAirQuality { Id = 1, Date = new DateTime(DateTime.Now.Year, 1, 1), Nitrogen_dioxide = 5.5 },
                new ArchiveAirQuality { Id = 2, Date = new DateTime(DateTime.Now.Year, 2, 1), Nitrogen_dioxide = 7.5 }
            };

            _dbContextMock.Setup(x => x.Set<ArchiveAirQuality>().AsQueryable())
                .Returns(airQualityData.AsQueryable());

            // Act
            await _viewModel.ApplyAirQualityFilterAsync();

            // Assert
            Assert.Equal(1, _viewModel.AirQualityData.Count); // Only one record should match the January filter
            Assert.Equal(1, _viewModel.AirQualityData[0].Id); // Ensure the correct record is returned
        }
    }
}
