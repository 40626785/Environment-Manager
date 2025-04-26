using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Diagnostics;
using System.Text;
using Xunit;
using System.Linq.Expressions;

namespace EnvironmentManager.Test
{
    public class AnomalyDetectionViewModelTests
    {
        [Fact]
        public async Task LoadAnomaliesAsync_LoadsDetectedAnomalies()
        {
            // Arrange
            var sensorList = new List<Sensor>
            {
                new Sensor { SensorId = 1, SensorName = "Sensor A", BatteryLevelPercentage = 15, ConnectivityStatus = "Offline" }
            };

            var mockSensorDbSet = TestUtils.MockDbSet(sensorList);
            var mockContext = new Mock<SensorDbContext>();
            mockContext.Setup(c => c.Sensors).Returns(mockSensorDbSet.Object);

            var viewModel = new AnomalyDetectionViewModel(mockContext.Object);

            // Act
            await viewModel.LoadAnomaliesAsync();

            // Assert
            Assert.Equal(2, viewModel.Anomalies.Count); // One for low battery, one for offline
        }

        [Fact]
        public void HandleError_StoresErrorMessage()
        {
            // Arrange
            var mockContext = new Mock<SensorDbContext>();
            var viewModel = new AnomalyDetectionViewModel(mockContext.Object);

            // Act
            viewModel.HandleError(new Exception(), "Test error");

            // Assert
            Assert.Equal("Test error", viewModel.DisplayError);
        }

        [Fact]
        public void HandleError_OutputsToTrace()
        {
            // Arrange
            var mockContext = new Mock<SensorDbContext>();
            var viewModel = new AnomalyDetectionViewModel(mockContext.Object);
            var builder = new StringBuilder();
            using var writer = new StringWriter(builder);
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new TextWriterTraceListener(writer));

            string exceptionMessage = "Simulated anomaly failure";

            // Act
            viewModel.HandleError(new Exception(exceptionMessage), "ignored");
            Trace.Flush();

            // Assert
            var traceOutput = builder.ToString();
            Assert.Contains(exceptionMessage, traceOutput);
        }
    }
}
