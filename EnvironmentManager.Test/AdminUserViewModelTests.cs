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
    public class HistoricalDataViewerViewModelTests
    {
        private DbContextOptions<HistoricalDataDbContext> CreateOptions() =>
            new DbContextOptionsBuilder<HistoricalDataDbContext>()
                .UseInMemoryDatabase(databaseName: $"HistoricalDataDb_{Guid.NewGuid()}")
                .Options;

        [Fact]
        public async Task LoadAirQualityDataAsync_LoadsAllRecords()
        {
            var options = CreateOptions();
            var context = new HistoricalDataDbContext(options);
            context.ArchiveAirQuality.AddRange(
                new ArchiveAirQuality { Date = DateTime.Today, Nitrogen_dioxide = 10 },
                new ArchiveAirQuality { Date = DateTime.Today.AddDays(-1), Nitrogen_dioxide = 15 }
            );
            await context.SaveChangesAsync();

            var viewModel = new HistoricalDataViewerViewModel(context, new Mock<ILoggingService>().Object);
            await viewModel.LoadAirQualityDataAsync();

            Assert.Equal(2, viewModel.AirQualityData.Count);
        }

        [Fact]
        public async Task LoadWaterQualityDataAsync_LoadsAllRecords()
        {
            var options = CreateOptions();
            var context = new HistoricalDataDbContext(options);
            context.ArchiveWaterQuality.AddRange(
                new ArchiveWaterQuality { Date = DateTime.Today, Nitrate_mg_l_1 = 5 },
                new ArchiveWaterQuality { Date = DateTime.Today.AddDays(-1), Nitrate_mg_l_1 = 8 }
            );
            await context.SaveChangesAsync();

            var viewModel = new HistoricalDataViewerViewModel(context, new Mock<ILoggingService>().Object);
            await viewModel.LoadWaterQualityDataAsync();

            Assert.Equal(2, viewModel.WaterQualityData.Count);
        }

        [Fact]
        public async Task LoadWeatherDataAsync_LoadsAllRecords()
        {
            var options = CreateOptions();
            var context = new HistoricalDataDbContext(options);
            context.ArchiveWeatherData.AddRange(
                new ArchiveWeatherData { Date_Time = DateTime.Today, Temperature_2m = 20 },
                new ArchiveWeatherData { Date_Time = DateTime.Today.AddDays(-1), Temperature_2m = 22 }
            );
            await context.SaveChangesAsync();

            var viewModel = new HistoricalDataViewerViewModel(context, new Mock<ILoggingService>().Object);
            await viewModel.LoadWeatherDataAsync();

            Assert.Equal(2, viewModel.WeatherData.Count);
        }
    }
}
