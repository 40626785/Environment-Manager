namespace EnvironmentManager.Test;

using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using Moq;
    
public class MaintenanceViewModelTests
{
    [Fact]
    public void IsOverdue_Overdue_SetTrue()
    {
        // Arrange
        var contextMock = new Mock<MaintenanceDbContext>();
        Maintenance maintenance = new Maintenance();
        maintenance.Overdue = false;
        maintenance.DueDate = DateTime.Now.AddDays(-1);
        var entityEntryMock = TestUtils.MockEntry(maintenance);
        bool reloadCalled = false;
        entityEntryMock.Setup(e => e.Reload()).Callback(() => reloadCalled = true);
        contextMock.Setup(context => context.Entry(It.IsAny<Maintenance>())).Returns(entityEntryMock.Object); 
        MaintenanceViewModel viewModel = new MaintenanceViewModel(contextMock.Object,maintenance);
        
        // Action
        viewModel.IsOverdue();

        //Assert
        contextMock.Verify(mock => mock.SaveChanges(), Times.Once());
        Assert.True(maintenance.Overdue);
    }

    [Fact]
    public void IsOverdue_NotDue_SetFalse()
    {
        // Arrange
        var contextMock = new Mock<MaintenanceDbContext>();
        Maintenance maintenance = new Maintenance();
        maintenance.Overdue = false;
        maintenance.DueDate = DateTime.Now;
        var entityEntryMock = TestUtils.MockEntry(maintenance);
        bool reloadCalled = false;
        entityEntryMock.Setup(e => e.Reload()).Callback(() => reloadCalled = true);
        contextMock.Setup(context => context.Entry(It.IsAny<Maintenance>())).Returns(entityEntryMock.Object); 
        MaintenanceViewModel viewModel = new MaintenanceViewModel(contextMock.Object,maintenance);
        
        // Action
        viewModel.IsOverdue();

        //Assert
        contextMock.Verify(mock => mock.SaveChanges(), Times.Once());
        Assert.False(maintenance.Overdue);
    }
}