namespace EnvironmentManager.Test;

using System.Diagnostics;
using System.Text;
using EnvironmentManager.Data;
using EnvironmentManager.Interfaces;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using Moq;
    
public class MaintenanceViewModelTests
{
    [Fact]
    public void IsOverdue_Overdue_SetTrue()
    {
        // Arrange
        var mockDataStore = new Mock<IMaintenanceDataStore>();
        Maintenance maintenance = new Maintenance();
        maintenance.Overdue = false;
        maintenance.DueDate = DateTime.Now.AddDays(-1);
        MaintenanceViewModel viewModel = new MaintenanceViewModel(mockDataStore.Object,maintenance);
        
        // Action
        viewModel.IsOverdue();

        //Assert
        mockDataStore.Verify(mock => mock.Save(), Times.Once());
        Assert.True(maintenance.Overdue);
    }

    [Fact]
    public void IsOverdue_NotDue_SetFalse()
    {
        // Arrange
        var mockDataStore = new Mock<IMaintenanceDataStore>();
        Maintenance maintenance = new Maintenance();
        maintenance.Overdue = false;
        maintenance.DueDate = DateTime.Now;
        MaintenanceViewModel viewModel = new MaintenanceViewModel(mockDataStore.Object,maintenance);
        
        // Action
        viewModel.IsOverdue();

        //Assert
        mockDataStore.Verify(mock => mock.Save(), Times.Once());
        Assert.False(maintenance.Overdue);
    }

    [Fact]
    public void HandleError_Store()
    {
        // Arrange
        var mockDataStore = new Mock<IMaintenanceDataStore>();
        MaintenanceViewModel maintenance = new MaintenanceViewModel(mockDataStore.Object);
        
        // Action
        string exceptionMessage = "test exception";
        string errorMessage = "test message";
        try
        {
            throw new Exception(exceptionMessage);
        }
        catch(Exception e)
        {
            maintenance.HandleError(e, errorMessage);
        }
        //Assert
        Assert.Equal(maintenance.DisplayError, errorMessage);
    }

    [Fact]
    public void HandleError_Trace()
    {
        // Arrange
        var mockDataStore = new Mock<IMaintenanceDataStore>();
        MaintenanceViewModel maintenance = new MaintenanceViewModel(mockDataStore.Object);
        StringBuilder builder = new StringBuilder();
        StringWriter writer = new StringWriter(builder);
        TextWriterTraceListener listener = new TextWriterTraceListener(writer);
        Trace.Listeners.Add(listener);
        
        // Action
        string exceptionMessage = "test exception";
        string errorMessage = "test message";
        try
        {
            throw new Exception(exceptionMessage);
        }
        catch(Exception e)
        {
            maintenance.HandleError(e, errorMessage);
        }
        //Assert
        string traceContents = builder.ToString().Replace("\n","");
        Assert.Equal(exceptionMessage, traceContents);
    }
}