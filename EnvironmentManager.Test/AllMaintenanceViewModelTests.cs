namespace EnvironmentManager.Test;

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using EnvironmentManager.Interfaces;
using Moq;
    
public class AllMaintenanceViewModelTests
{
    [Fact]
    public void SortCollection_OnInstantiation_SortList()
    {
        //Arrange
        var mockDataStore = new Mock<IMaintenanceDataStore>();
        mockDataStore.Setup(x => x.RetrieveAll()).Returns(testData());
        //Action
        AllMaintenanceViewModel allMaintenance = new AllMaintenanceViewModel(mockDataStore.Object); //SortCollection function invoked as part of constructor
        ObservableCollection<MaintenanceViewModel> producedCollection = allMaintenance.AllMaintenance; //Get sorted collection
        //Assert
        for(var i = 1; i < producedCollection.Count; i++){
            Assert.True(producedCollection[i].Priority >= producedCollection [i-1].Priority); //verify order, sorted in ascending
        }
    }

    [Fact]
    public void HandleError_Store()
    {
        // Arrange
        var mockDataStore = new Mock<IMaintenanceDataStore>();
        mockDataStore.Setup(x => x.RetrieveAll()).Returns(testData());
        AllMaintenanceViewModel allMaintenance = new AllMaintenanceViewModel(mockDataStore.Object);
        
        // Action
        string exceptionMessage = "test exception";
        string errorMessage = "test message";
        try
        {
            throw new Exception(exceptionMessage);
        }
        catch(Exception e)
        {
            allMaintenance.HandleError(e, errorMessage);
        }
        //Assert
        Assert.Equal(allMaintenance.DisplayError, errorMessage);
    }

    [Fact]
    public void HandleError_Trace()
    {
        // Arrange
        var mockDataStore = new Mock<IMaintenanceDataStore>();
        mockDataStore.Setup(x => x.RetrieveAll()).Returns(testData());
        AllMaintenanceViewModel allMaintenance = new AllMaintenanceViewModel(mockDataStore.Object);
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
            allMaintenance.HandleError(e, errorMessage);
        }
        //Assert
        string traceContents = builder.ToString().Replace("\n","");
        Assert.Equal(exceptionMessage, traceContents);
    }

    private List<Maintenance> testData() {
        Random rand = new Random();
        List<Maintenance> testList = new List<Maintenance>();
        for(var i = 0; i <= 5; i++) //creates dummy data with random priority to ensure jumbled order
        {
            Maintenance maintenance = new Maintenance();
            maintenance.Id = i;
            maintenance.Priority = rand.Next(1,4);
            testList.Add(maintenance);
        }
        return testList;
    }
}