using Xunit;
using Moq;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Models;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EnvironmentManager.Test
{
    public class LogViewModelTests
    {
        private async Task<LogDbContext> CreateInMemoryDbAsync()
        {
            var options = new DbContextOptionsBuilder<LogDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new LogDbContext(options);

            await context.Logs.AddRangeAsync(new List<LogEntry>
            {
                new LogEntry { LogID = 1, LogDateTime = DateTime.Today.AddDays(-1), LogMessage = "System started" },
                new LogEntry { LogID = 2, LogDateTime = DateTime.Today, LogMessage = "User logged in" },
                new LogEntry { LogID = 3, LogDateTime = DateTime.Today.AddDays(-2), LogMessage = "Error occurred" }
            });
            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task LoadDataAsync_LoadsLogEntries()
        {
            var context = await CreateInMemoryDbAsync();

            var mockFactory = new Mock<IDbContextFactory<LogDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(context);

            var mockDialog = new Mock<IUserDialogService>();
            var viewModel = new LogViewModel(mockFactory.Object, mockDialog.Object);

            await viewModel.LoadDataAsync();

            Assert.Equal(3, viewModel.TableData.Count);
        }

        [Fact]
        public async Task ApplyFiltersAsync_FiltersByDateAndId()
        {
            var context = await CreateInMemoryDbAsync();

            var mockFactory = new Mock<IDbContextFactory<LogDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(context);

            var mockDialog = new Mock<IUserDialogService>();
            var viewModel = new LogViewModel(mockFactory.Object, mockDialog.Object)
            {
                IsDateFilterEnabled = true,
                StartDate = DateTime.Today.AddDays(-2),
                EndDate = DateTime.Today,
                StartIdText = "2",
                EndIdText = "3"
            };

            await viewModel.ApplyFiltersAsync();

            Assert.Equal(2, viewModel.TableData.Count);
            Assert.Contains(viewModel.TableData, l => l.LogID == 2);
            Assert.Contains(viewModel.TableData, l => l.LogID == 3);

            Assert.Equal(2, viewModel.TableData[0].LogID);
        }

        [Fact]
        public async Task DeleteFilteredAsync_DeletesEntries_WhenConfirmed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LogDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var seededContext = new LogDbContext(options);
            await seededContext.Logs.AddRangeAsync(new List<LogEntry>
    {
        new LogEntry { LogID = 1, LogDateTime = DateTime.Today, LogMessage = "Test1" },
        new LogEntry { LogID = 2, LogDateTime = DateTime.Today, LogMessage = "Test2" }
    });
            await seededContext.SaveChangesAsync();

            // Mock factory to always return NEW instances of context
            var mockFactory = new Mock<IDbContextFactory<LogDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(() => new LogDbContext(options));

            var mockDialog = new Mock<IUserDialogService>();
            mockDialog.Setup(d => d.ShowConfirmation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .ReturnsAsync(true);
            mockDialog.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);

            var viewModel = new LogViewModel(mockFactory.Object, mockDialog.Object);
            await viewModel.LoadDataAsync();

            // Act
            await viewModel.DeleteFilteredAsync();

            //  Use a fresh context for assertions
            using var verifyContext = new LogDbContext(options);
            int remaining = await verifyContext.Logs.CountAsync();

            // Assert
            Assert.Equal(0, remaining); // All records deleted
        }


        [Fact]
        public async Task ExportToCsvAsync_CreatesCsvFile()
        {
            var context = await CreateInMemoryDbAsync();

            var mockFactory = new Mock<IDbContextFactory<LogDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(context);

            var mockDialog = new Mock<IUserDialogService>();
            mockDialog.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);

            var viewModel = new LogViewModel(mockFactory.Object, mockDialog.Object);
            await viewModel.LoadDataAsync();



            string tempPath = Path.Combine(Path.GetTempPath(), "TestLogExport.csv");
            await viewModel.ExportToCsvAsync(tempPath);

            Assert.True(File.Exists(tempPath), "CSV file was not created.");
        }
    }
}

