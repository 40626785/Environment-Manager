using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Models;
using EnvironmentManager.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class FirmwareUpdateViewModel : ObservableObject
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

    public FirmwareUpdateViewModel(SensorDbContext context)
    {
        _context = context;
        UpdateFirmwareCommand = new AsyncRelayCommand(UpdateFirmwareAsync);
        LoadSensorsCommand = new AsyncRelayCommand(LoadSensorsAsync);
    }

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
            Debug.WriteLine($"[FirmwareUpdate] Failed to load sensors: {ex.Message}");
        }
    }

    private async Task UpdateFirmwareAsync()
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

            // Optional: Clear selection
            foreach (var item in Sensors)
                item.IsSelected = false;

            FirmwareVersion = string.Empty;
            StatusMessage = $"Firmware updated for {selectedSensors.Count} sensor(s).";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[FirmwareUpdate] Failed: {ex.Message}");
            StatusMessage = "Firmware update failed.";
        }
    }
}
