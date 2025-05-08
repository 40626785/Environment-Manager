using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Models;
using EnvironmentManager.Data;
using System.Collections.ObjectModel;

namespace EnvironmentManager.Test
{
    public class ResolvedAlertsViewModelTests : IDisposable
    {
        private DbContextOptions<AlertDbContext> _options;
        private AlertDbContext _context;
        private Mock<IDbContextFactory<AlertDbContext>> _mockFactory;

        public ResolvedAlertsViewModelTests()
        {
            _options = new DbContextOptionsBuilder<AlertDbContext>()
                .UseInMemoryDatabase(databaseName: $"ResolvedAlertsTestDb_{Guid.NewGuid()}")
                .EnableSensitiveDataLogging()
                .Options;

            _context = new AlertDbContext(_options);
            _mockFactory = new Mock<IDbContextFactory<AlertDbContext>>();
            _mockFactory.Setup(f => f.CreateDbContext()).Returns(() => new AlertDbContext(_options));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private async Task SeedResolvedAlertsAsync()
        {
            _context.AlertTable.RemoveRange(_context.AlertTable);
            await _context.SaveChangesAsync();

            await _context.AlertTable.AddRangeAsync(new List<Alert>
            {
                new Alert
                {
                    AlertId = 1,
                    LocationId = 1,
                    Date_Time = DateTime.Now,
                    Parameter = "Temperature",
                    Value = 25.0,
                    Deviation = 0.5,
                    Message = "Resolved Alert A",
                    CreatedAt = DateTime.Now,
                    IsResolved = true
                },
                new Alert
                {
                    AlertId = 2,
                    LocationId = 2,
                    Date_Time = DateTime.Now,
                    Parameter = "Pressure",
                    Value = 101.0,
                    Deviation = 0.2,
                    Message = "Resolved Alert B",
                    CreatedAt = DateTime.Now,
                    IsResolved = true
                },
                new Alert
                {
                    AlertId = 3,
                    LocationId = 3,
                    Date_Time = DateTime.Now,
                    Parameter = "Humidity",
                    Value = 50.0,
                    Deviation = 1.0,
                    Message = "Unresolved Alert C",
                    CreatedAt = DateTime.Now,
                    IsResolved = false
                }
            });
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task LoadResolvedAlerts_ShouldLoadOnlyResolvedAlerts()
        {
            // Arrange
            await SeedResolvedAlertsAsync();
            var viewModel = new ResolvedAlertsViewModel(_mockFactory.Object);

            // Act
            await viewModel.LoadResolvedAlertsCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal(2, viewModel.ResolvedAlerts.Count);
            Assert.Contains(viewModel.ResolvedAlerts, a => a.Message == "Resolved Alert A");
            Assert.Contains(viewModel.ResolvedAlerts, a => a.Message == "Resolved Alert B");
        }

        [Fact]
        public async Task LoadResolvedAlerts_ShouldHandleError()
        {
            // Arrange
            var mockFactory = new Mock<IDbContextFactory<AlertDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Throws(new Exception("Database error"));

            var viewModel = new ResolvedAlertsViewModel(mockFactory.Object);

            // Act
            await viewModel.LoadResolvedAlertsCommand.ExecuteAsync(null);

            // Assert
            Assert.Empty(viewModel.ResolvedAlerts); // Should not load any data
        }

        [Fact]
        public async Task DeleteResolvedAlert_ShouldRemoveAlert()
        {
            // Arrange
            await SeedResolvedAlertsAsync();
            var viewModel = new ResolvedAlertsViewModel(_mockFactory.Object);
            await viewModel.LoadResolvedAlertsCommand.ExecuteAsync(null);

            var alert = viewModel.ResolvedAlerts.FirstOrDefault(a => a.AlertId == 1);
            Assert.NotNull(alert);

            // Act
            await viewModel.DeleteResolvedAlertCommand.ExecuteAsync(alert.AlertId);

            // Assert
            Assert.DoesNotContain(viewModel.ResolvedAlerts, a => a.AlertId == alert.AlertId);

            // Check the database to confirm deletion
            using var verifyContext = new AlertDbContext(_options);
            var deletedAlert = await verifyContext.AlertTable.FindAsync(alert.AlertId);
            Assert.Null(deletedAlert);
        }

        [Fact]
        public async Task DeleteResolvedAlert_ShouldHandleError()
        {
            // Arrange
            var mockFactory = new Mock<IDbContextFactory<AlertDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Throws(new Exception("Database error"));

            var viewModel = new ResolvedAlertsViewModel(mockFactory.Object);

            // Act
            await viewModel.DeleteResolvedAlertCommand.ExecuteAsync(1);

            // Assert
            Assert.Empty(viewModel.ResolvedAlerts); // No change expected
        }
    }
}
