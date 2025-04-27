# Sensor Monitoring

## Overview

The sensor monitoring system is a critical component of the Environment Manager application, enabling users to track sensor statuses in real-time across different locations. This page documents the monitoring-related files in the project and their roles in implementing this functionality.

## Related User Story

This feature addresses the need for real-time monitoring of environmental sensors within facility management systems. It allows operators to quickly identify problems with sensors and respond proactively to connectivity issues or environmental anomalies.

[Link to GitHub Issue](https://github.com/40626785/Environment-Manager/issues/4)

## Sensor Monitoring Files

### Models

#### `Models/SensorStatus.cs`

This file defines the SensorStatus entity model that represents point-in-time status records for sensors in the system.

**Key Features:**
- Defines properties for status identification (StatusId, SensorId)
- Tracks operational status (ConnectivityStatus, StatusTimestamp)
- Monitors battery levels (BatteryLevelPercentage)
- Records error metrics (ErrorCount, WarningCount)
- Implements proper initialization of default values
- Creates relationship to parent Sensor entity

### ViewModels

#### `ViewModels/SensorMonitoringViewModel.cs`

This ViewModel manages the collection of sensor statuses and provides real-time monitoring capabilities.

**Key Features:**
- Implements MVVM pattern with ObservableObject and ObservableProperty attributes
- Provides LoadSensorStatusesAsync method to retrieve sensor statuses from the database
- Includes automatic refresh functionality with configurable intervals
- Tracks metrics such as total, online, offline, and degraded sensor counts
- Implements proper resource management with cancellation tokens
- Handles dependency injection for database contexts

### Views

#### `Views/SensorMonitoringPage.xaml` and `Views/SensorMonitoringPage.xaml.cs`

These files implement the sensor monitoring UI and code-behind.

**Key Features:**
- Displays real-time sensor statuses with visual indicators
- Provides summary statistics (total, online, offline counts)
- Includes auto-refresh toggle and manual refresh button
- Shows timestamps for last refresh operation
- Binds to the SensorMonitoringViewModel
- Manages view lifecycle events (OnAppearing, OnDisappearing)

## Data Flow

1. When the SensorMonitoringPage is loaded, it initializes a SensorMonitoringViewModel
2. The ViewModel loads the latest SensorStatus entries for each Sensor from the database
3. The UI displays the current status of all sensors with appropriate visual indicators
4. The auto-refresh system periodically updates the display at configurable intervals
5. Users can manually trigger a refresh or navigate to sensor details

## Unit Tests

The monitoring functionality is thoroughly tested with unit tests:

### `EnvironmentManager.Test/SensorMonitoringViewModelTests.cs`

Tests the sensor monitoring functionality.

**Key Tests:**
- Loading sensor statuses from the database
- Calculating summary metrics correctly
- Auto-refresh functionality
- Handling of various connectivity states

## Design Patterns and Principles

- **Observer Pattern**: UI elements observe changes in the ViewModel's properties
- **MVVM Pattern**: Clear separation between UI (Views), business logic (ViewModels), and data (Models)
- **Dependency Injection**: DbContext is injected for better testability
- **Resource Management**: Proper handling of refresh cycles and cancellation

## Performance Considerations

- Asynchronous operations for database access
- Efficient query patterns to minimize database load
- Configurable refresh intervals to balance responsiveness and resource usage
- Cleanup of resources when the page is no longer visible

## Class Diagram

A detailed class diagram of the sensor monitoring system can be found in [Sensor Monitoring Class Diagram](../docs/sensorMonitoringClassDiagram.md).

## Code Conventions

- Asynchronous operations for database access (LoadSensorStatusesAsync)
- Proper exception handling and logging
- Use of ObservableProperty and RelayCommand attributes for MVVM implementation
- Consistent naming conventions following C# standards 