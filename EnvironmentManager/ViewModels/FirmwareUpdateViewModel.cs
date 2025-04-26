using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Models;
using EnvironmentManager.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using EnvironmentManager.Interfaces;

namespace EnvironmentManager.ViewModels
{
    /// <summary>
    /// ViewModel for updating firmware versions across selected sensors.
    /// </summary>
    public partial class FirmwareUpdateViewModel : ObservableObject, IErrorHandling
    {
        private readonly SensorDbContext _context;

        [ObservableProperty]
        private ObservableCollection<SelectableSensor> sensors = new();

        [ObservableProperty]
        private string firmwareVersion = string.Empty;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        public IAsyncRelayCommand UpdateFirmwareCommand { get; }
        public IAsyncRelayCommand LoadSensorsCommand { get; }
        public string DisplayError { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Initializes a new instance of the FirmwareUpdateViewModel.
        /// </summary>
        public FirmwareUpdateViewModel(SensorDbContext context)
        {
            _context = context;
            UpdateFirmwareCommand = new AsyncRelayCommand(UpdateFirmwareAsync);
            LoadSensorsCommand = new AsyncRelayCommand(LoadSensorsAsync);
        }

        /// <summary>
        /// Loads all available sensors from the database for selection.
        /// </summary>
        public async Task LoadSensorsAsync()
        {
            try
            {
                var sensorList = await _context.Sensors.AsNoTracking().ToListAsync();
                Sensors = new ObservableCollection<SelectableSensor>(
                    sensorList.Select(s => new SelectableSensor { Sensor = s, IsSelected = false }));
            }
            catch (Exception ex)
            {
                HandleError(ex, "Failed to load sensors.");
            }
        }

        /// <summary>
        /// Updates the firmware version of selected sensors.
        /// </summary>
        public async Task UpdateFirmwareAsync()
        {
            if (string.IsNullOrWhiteSpace(FirmwareVersion))
            {
                StatusMessage = "Firmware version cannot be empty.";
                return;
            }

            var selectedSensors = Sensors.Where(s => s.IsSelected).Select(s => s.Sensor).ToList();

            if (!selectedSensors.Any())
            {
                StatusMessage = "Please select at least one sensor.";
                return;
            }

            try
            {
                foreach (var sensor in selectedSensors)
                {
                    var tracked = await _context.Sensors.FindAsync(sensor.SensorId);
                    if (tracked != null)
                    {
                        tracked.FirmwareVersion = FirmwareVersion;
                    }
                }

                await _context.SaveChangesAsync();

                // Reset selections after successful update
                foreach (var item in Sensors)
                    item.IsSelected = false;

                FirmwareVersion = string.Empty;
                StatusMessage = $"Firmware updated for {selectedSensors.Count} sensor(s).";
            }
            catch (Exception ex)
            {
                HandleError(ex, "Firmware update failed.");
            }
        }

        /// <summary>
        /// Handles errors by logging and updating the status message.
        /// </summary>
        public void HandleError(Exception ex, string message)
        {
            Trace.WriteLine($"[FirmwareUpdateViewModel] {ex.Message}");
            StatusMessage = message;
        }
    }
}