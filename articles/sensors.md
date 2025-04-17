# Sensor Management

## Overview

The sensor management system is a core component of the Environment Manager application, allowing users to add, edit, and monitor environmental sensors across different locations. This page documents the sensor-related files in the project and their roles in implementing this functionality.

## Related User Story

This feature addresses the need for comprehensive sensor management within environmental monitoring systems. It allows facility managers to track and maintain sensor networks efficiently.

[Link to GitHub Issue](https://github.com/40626785/Environment-Manager/issues/3)



## Sensor-Related Files

### Models

#### `Models/Sensor.cs`

This file defines the Sensor entity model that represents environmental sensors in the system.

**Key Features:**
- Defines properties for sensor identification (SensorId, SensorName)
- Tracks technical details (Model, Manufacturer, SensorType, FirmwareVersion)
- Monitors operational status (IsActive, ConnectivityStatus, BatteryLevelPercentage)
- Stores connectivity information (DataSource, SensorUrl)
- Links to location through LocationId foreign key
- Implements data annotations for validation

### Data Access

#### `Data/SensorDbContext.cs`

This file implements the database context for sensor-related entities using Entity Framework Core.

**Key Features:**
- Defines DbSet for Sensors
- Configures entity relationships in OnModelCreating
- Establishes one-to-many relationship between Location and Sensor
- Implements proper deletion behavior (Restrict) to maintain data integrity
- Follows repository pattern principles

### ViewModels

#### `ViewModels/SensorViewModel.cs`

This ViewModel manages the collection of sensors and provides operations for the main sensor listing page.

**Key Features:**
- Implements MVVM pattern with ObservableObject and ObservableProperty attributes
- Provides LoadSensorsAsync method to retrieve sensors from the database
- Includes navigation methods to add and edit pages
- Manages sensor selection and property display
- Handles sensor collection display and filtering

#### `ViewModels/AddSensorViewModel.cs`

This ViewModel handles the creation of new sensors in the system.

**Key Features:**
- Manages form data for creating new sensors
- Loads available locations for sensor placement
- Implements validation logic for sensor properties
- Provides error messages and validation state
- Handles the database save operation
- Uses dependency injection for database contexts

#### `ViewModels/EditSensorViewModel.cs`

This ViewModel manages the editing of existing sensors.

**Key Features:**
- Loads sensor data by ID from the database
- Provides form fields for all editable properties
- Implements validation logic similar to AddSensorViewModel
- Handles the update operation to the database
- Manages sensor status changes

### Views

#### `Views/SensorPage.xaml` and `Views/SensorPage.xaml.cs`

These files implement the main sensor listing page UI and code-behind.

**Key Features:**
- Displays the collection of sensors in a ListView
- Provides filtering and sorting options
- Includes navigation to add and edit pages
- Binds to the SensorViewModel

#### `Views/AddSensorPage.xaml` and `Views/AddSensorPage.xaml.cs`

These files implement the UI for adding new sensors.

**Key Features:**
- Provides form fields for all sensor properties
- Includes location selection dropdown
- Displays validation errors
- Binds to the AddSensorViewModel

#### `Views/EditSensorPage.xaml` and `Views/EditSensorPage.xaml.cs`

These files implement the UI for editing existing sensors.

**Key Features:**
- Similar to AddSensorPage but pre-populated with sensor data
- Includes additional options for managing sensor status
- Handles sensor deactivation
- Binds to the EditSensorViewModel

## Unit Tests

The sensor functionality is thoroughly tested with unit tests:

### `EnvironmentManager.Test/SensorViewModelTests.cs`

Tests the main sensor listing functionality.

**Key Tests:**
- Loading sensors from the database
- Filtering and sorting sensors
- Navigation between pages

### `EnvironmentManager.Test/AddSensorViewModelTests.cs`

Tests the sensor creation functionality.

**Key Tests:**
- Validation of sensor properties
- Location loading and selection
- Database save operations
- Error handling

### `EnvironmentManager.Test/EditSensorViewModelTests.cs`

Tests the sensor editing functionality.

**Key Tests:**
- Loading existing sensor data
- Validation during editing
- Update operations
- Status change handling

## Design Patterns and Principles

- **Repository Pattern**: SensorDbContext abstracts data access logic
- **MVVM Pattern**: Clear separation between UI (Views), business logic (ViewModels), and data (Models)
- **Dependency Injection**: Services and contexts are injected for better testability
- **SOLID Principles**: Classes have single responsibilities and depend on abstractions

## Code Conventions

- Asynchronous operations for database access (LoadSensorsAsync, SaveSensorAsync)
- Proper exception handling and validation
- Use of ObservableProperty and RelayCommand attributes for MVVM implementation
- Consistent naming conventions following C# standards
