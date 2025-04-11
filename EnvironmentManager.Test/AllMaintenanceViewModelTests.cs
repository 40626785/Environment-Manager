namespace EnvironmentManager.Test;

using System.Collections.ObjectModel;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using EnvironmentManager.ViewModels;
using Moq;
    
public class AllMaintenanceViewModelTests
{
    [Fact]
    public void SortCollection_OnInstantiation_SortList()
    {
        //Arrange
        var contextMock = new Mock<MaintenanceDbContext>();
        Random rand = new Random();
        List<Maintenance> testList = new List<Maintenance>();
        for(var i = 0; i <= 5; i++) //creates dummy data with random priority to ensure jumbled order
        {
            Maintenance maintenance = new Maintenance();
            maintenance.Id = i;
            maintenance.Priority = rand.Next(1,4);
            testList.Add(maintenance);
            var entityEntryMock = TestUtils.MockEntry(maintenance);
            contextMock.Setup(context => context.Entry(maintenance)).Returns(entityEntryMock.Object); 
        }
        var dbSet = TestUtils.MockDbSet(testList);
        contextMock.Setup(c => c.Maintenance).Returns(dbSet.Object);
        //Action
        AllMaintenanceViewModel allMaintenance = new AllMaintenanceViewModel(contextMock.Object); //SortCollection function invoked as part of constructor
        ObservableCollection<MaintenanceViewModel> producedCollection = allMaintenance.AllMaintenance; //Get sorted collection
        //Assert
        for(var i = 1; i < producedCollection.Count; i++){
            Assert.True(producedCollection[i].Priority >= producedCollection [i-1].Priority); //verify order, sorted in ascending
        }
    }
}