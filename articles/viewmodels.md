# ViewModels

The Environment Manager follows the MVVM (Model-View-ViewModel) pattern. This article documents the key ViewModels and their responsibilities.

## AddSensorViewModel

Handles the creation of new sensors in the system.

### Properties and Dependencies
```csharp
public partial class AddSensorViewModel : ObservableObject
{
    private readonly SensorDbContext _sensorContext;
    private readonly LocationDbContext _locationContext;
    
    [ObservableProperty]
    private ObservableCollection<Location> _locations;
    
    [ObservableProperty]
    private Location _selectedLocation;
}
```

### Validation
- Validates sensor name
- Checks battery level
- Validates sensor URL
- Ensures required fields are filled

## EditSensorViewModel

Manages the editing of existing sensors.

### Key Features
```csharp
[QueryProperty(nameof(SensorId), "id")]
public partial class EditSensorViewModel : ObservableObject
{
    private readonly SensorDbContext _sensorContext;
    private readonly LocationDbContext _locationContext;
    
    [ObservableProperty]
    private int _sensorId;
    
    // Properties and validation logic
}
```

### Functionality
- Loads existing sensor data
- Validates modifications
- Updates sensor information
- Manages location changes

## MaintenanceViewModel

Handles individual maintenance tasks.

### Properties
```csharp
public partial class MaintenanceViewModel : ObservableObject, IQueryAttributable, IErrorHandling
{
    public DateTime DueDate { get; set; }
    public int Id { get; }
    public bool Overdue { get; set; }
    public int Priority { get; set; }
    public string Description { get; set; }
}
```

### Error Handling
```csharp
public void HandleError(Exception e, string message)
{
    Trace.WriteLine(e.Message);
    DisplayError = message;
}
```

## AllMaintenanceViewModel

Manages the collection of maintenance tasks.

### Collection Management
```csharp
public class AllMaintenanceViewModel : ObservableObject, IQueryAttributable, IErrorHandling
{
    public ObservableCollection<MaintenanceViewModel> AllMaintenance { get; }
    public ICommand RefreshCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand NewTicketCommand { get; }
}
```

### Sorting
```csharp
private void SortCollection() 
{
    ObservableCollection<MaintenanceViewModel> sorted = 
        new ObservableCollection<MaintenanceViewModel>(
            AllMaintenance.OrderByDescending(i => i.Priority));
            
    foreach (MaintenanceViewModel viewModel in sorted)
    {
        AllMaintenance.Remove(viewModel);
        AllMaintenance.Insert(0, viewModel);
    }
}
```

## Testing

The ViewModels are thoroughly tested using unit tests:

### AddSensorViewModel Tests
```csharp
[Fact]
public void AddSensor_ValidData_AddsToDatabase()
{
    // Arrange
    var sensorContextMock = new Mock<SensorDbContext>();
    var locationContextMock = new Mock<LocationDbContext>();
    
    // Act & Assert
    // Test sensor addition logic
}
```

### EditSensorViewModel Tests
```csharp
[Fact]
public async Task UpdateSensor_ValidData_UpdatesDatabase()
{
    // Arrange
    var sensorContextMock = new Mock<SensorDbContext>();
    var locationContextMock = new Mock<LocationDbContext>();
    
    // Act & Assert
    // Test sensor update logic
}
```

## Best Practices

1. **Property Change Notifications**
   - Use ObservableObject base class
   - Implement INotifyPropertyChanged
   - Use [ObservableProperty] attribute

2. **Command Pattern**
   - Use ICommand interface
   - Implement async commands where needed
   - Handle command execution states

3. **Error Handling**
   - Implement IErrorHandling interface
   - Provide meaningful error messages
   - Log errors appropriately

4. **Validation**
   - Validate all user inputs
   - Provide immediate feedback
   - Use validation dictionaries
