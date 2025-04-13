namespace EnvironmentManager.Test;
using Moq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

public static class TestUtils 
{
    public static Mock<EntityEntry<T>> MockEntry<T> (T obj) 
        where T : class 
    {
        //documented solution for mocking EntityFramework. Retrieved from: https://github.com/dotnet/efcore/issues/33095
        var stateManagerMock = new Mock<IStateManager>();
        stateManagerMock.Setup(x => x.CreateEntityFinder(It.IsAny<IEntityType>())).Returns(new Mock<IEntityFinder>().Object);
        stateManagerMock.Setup(x => x.ValueGenerationManager).Returns(new Mock<IValueGenerationManager>().Object);
        stateManagerMock.Setup(x => x.InternalEntityEntryNotifier).Returns(new Mock<IInternalEntityEntryNotifier>().Object);

        var entityTypeMock = new Mock<IRuntimeEntityType>();
        var keyMock = new Mock<IKey>();
        keyMock.Setup(x => x.Properties).Returns([]);
        entityTypeMock.Setup(x => x.FindPrimaryKey()).Returns(keyMock.Object);
        entityTypeMock.Setup(e => e.EmptyShadowValuesFactory).Returns(() => new Mock<ISnapshot>().Object);
        
        var internalEntityEntry = new InternalEntityEntry(stateManagerMock.Object, entityTypeMock.Object, obj);
        var entityEntryMock = new Mock<EntityEntry<T>>(internalEntityEntry);
        return entityEntryMock;
    }

    public static Mock<DbSet<T>> MockDbSet<T> (List<T> list) 
        where T : class 
    {
        //documented solution for correctly mocking EntityFramework DbSets. Retrieved from: https://learn.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking
        IQueryable<T> queryableList = list.AsQueryable();
        var dbSet = new Mock<DbSet<T>>();
        dbSet.As<IQueryable<T>>().Setup(set => set.Provider).Returns(queryableList.Provider);
        dbSet.As<IQueryable<T>>().Setup(set => set.Expression).Returns(queryableList.Expression);
        dbSet.As<IQueryable<T>>().Setup(set => set.ElementType).Returns(queryableList.ElementType);
        dbSet.As<IQueryable<T>>().Setup(set => set.GetEnumerator()).Returns(() => queryableList.GetEnumerator());
        return dbSet;
    }
}