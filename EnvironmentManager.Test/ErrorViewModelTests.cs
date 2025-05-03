using Xunit;
using Moq;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Models;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentManager.Test
{
    public class ErrorViewModelTests
    {
        private DbContextOptions<ErrorDbContext> CreateTestOptions()
        {
            return new DbContextOptionsBuilder<ErrorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private async Task<ErrorDbContext> CreateInMemoryDbAsync(DbContextOptions<ErrorDbContext> options)
        {
            var context = new ErrorDbContext(options);
            await context.Errors.AddRangeAsync(new List<ErrorEntry>
            {
                new ErrorEntry { ErrorID = 1, ErrorDateTime = DateTime.Today.AddDays(-1), ErrorMessage = "Error A" },
                new ErrorEntry { ErrorID = 2, ErrorDateTime = DateTime.Today, ErrorMessage = "Error B" },
                new ErrorEntry { ErrorID = 3, ErrorDateTime = DateTime.Today.AddDays(-2), ErrorMessage = "Error C" }
            });
            await context.SaveChangesAsync();
            return context;
        }

        [Fact]
        public async Task LoadDataAsync_LoadsAllErrorEntries()
        {
            var options = CreateTestOptions();
            var context = await CreateInMemoryDbAsync(options);

            var mockFactory = new Mock<IDbContextFactory<ErrorDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(() => new ErrorDbContext(options));

            var mockDialog = new Mock<IUserDialogService>();
            var viewModel = new ErrorViewModel(mockFactory.Object, mockDialog.Object);

            await viewModel.LoadDataAsync();

            Assert.Equal(3, viewModel.TableData.Count);
        }

        [Fact]
        public async Task ApplyFiltersAsync_FiltersByDateAndId()
        {
            var options = CreateTestOptions();
            var context = await CreateInMemoryDbAsync(options);

            var mockFactory = new Mock<IDbContextFactory<ErrorDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(() => new ErrorDbContext(options));

            var mockDialog = new Mock<IUserDialogService>();
            var viewModel = new ErrorViewModel(mockFactory.Object, mockDialog.Object)
            {
                StartDate = DateTime.Today.AddDays(-2),
                EndDate = DateTime.Today,
                IsDateFilterEnabled = true,
                StartIdText = "2",
                EndIdText = "3"
            };

            await viewModel.ApplyFiltersAsync();

            Assert.Equal(2, viewModel.TableData.Count);
            Assert.Contains(viewModel.TableData, e => e.ErrorID == 2);
            Assert.Contains(viewModel.TableData, e => e.ErrorID == 3);
        }

        [Fact]
        public async Task DeleteFilteredAsync_DeletesEntries_WhenConfirmed()
        {
            var options = CreateTestOptions();
            var context = await CreateInMemoryDbAsync(options);

            var mockFactory = new Mock<IDbContextFactory<ErrorDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(() => new ErrorDbContext(options));

            var mockDialog = new Mock<IUserDialogService>();
            mockDialog.Setup(d => d.ShowConfirmation(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .ReturnsAsync(true);
            mockDialog.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);

            var viewModel = new ErrorViewModel(mockFactory.Object, mockDialog.Object);
            await viewModel.LoadDataAsync();

            await viewModel.DeleteFilteredAsync();

            using var verifyContext = new ErrorDbContext(options);
            var remaining = await verifyContext.Errors.CountAsync();
            Assert.Equal(0, remaining);
        }

        [Fact]
        public async Task ExportToCsvAsync_CreatesCsvFile()
        {
            var options = CreateTestOptions();
            var context = await CreateInMemoryDbAsync(options);

            var mockFactory = new Mock<IDbContextFactory<ErrorDbContext>>();
            mockFactory.Setup(f => f.CreateDbContext()).Returns(() => new ErrorDbContext(options));

            var mockDialog = new Mock<IUserDialogService>();
            mockDialog.Setup(d => d.ShowAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(Task.CompletedTask);

            var viewModel = new ErrorViewModel(mockFactory.Object, mockDialog.Object);
            await viewModel.LoadDataAsync();

            var tempFilePath = Path.Combine(Path.GetTempPath(), $"ErrorExport_{Guid.NewGuid()}.csv");
            await viewModel.ExportToCsvAsync(tempFilePath);

            Assert.True(File.Exists(tempFilePath), "CSV file was not created.");
            File.Delete(tempFilePath);
        }
    }
}
