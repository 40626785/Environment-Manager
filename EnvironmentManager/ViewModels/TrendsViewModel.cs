using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvironmentManager.Data;
using EnvironmentManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Maui.Storage;
using EnvironmentManager.Graphs;


namespace EnvironmentManager.ViewModels
{
    public partial class TrendsViewModel : ObservableObject
    {
        private readonly ReadingsDbContext _readingsContext;

        public ObservableCollection<Reading> FilteredReadings { get; } = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string currentCategory = "Air"; // Default category

        [ObservableProperty]
        private string displayError;

        [ObservableProperty]
        private string temperatureTrendSummary;

        public IDrawable TemperatureTrendDrawable { get; }

        public event EventHandler TemperatureTrendUpdated;


        public TrendsViewModel(ReadingsDbContext readingsContext)
        {
            _readingsContext = readingsContext;
            TemperatureTrendDrawable = new TemperatureTrendGraph(this);
        }

        public async Task LoadDataAsync(string category)
        {
            try
            {
                IsBusy = true;

                // Fetch data asynchronously
                var readings = await _readingsContext.Readings
                    .AsNoTracking()
                    .Where(r => r.Category == category)
                    .OrderByDescending(r => r.Timestamp)
                    .ToListAsync();

                // Update the UI on the main thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    FilteredReadings.Clear();
                    foreach (var reading in readings)
                    {
                        FilteredReadings.Add(reading);
                    }
                });
            }
            catch (Exception ex)
            {
                DisplayError = $"Failed to load data: {ex.Message}";
                Debug.WriteLine($"[TrendsViewModel] {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public async Task LoadCategoryAsync(string category)
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                FilteredReadings.Clear();
                DisplayError = string.Empty;

                var data = await _readingsContext.Readings
                    .AsNoTracking()
                    .Where(r => r.Category == category)
                    .OrderByDescending(r => r.Timestamp)
                    .ToListAsync();

                if (data.Count == 0)
                {
                    DisplayError = $"No {category} data available.";
                }
                else
                {
                    foreach (var item in data)
                        FilteredReadings.Add(item);
                }

                if (CurrentCategory == "Weather")
                {
                    CalculateTemperatureTrend();
                }
                else
                {
                    TemperatureTrendSummary = "";
                    TemperatureTrendUpdated?.Invoke(this, EventArgs.Empty); // Also clear the graph
                }


                CalculateTemperatureTrend();
            }
            catch (Exception ex)
            {
                DisplayError = $"Failed to load {category} data: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SwitchCategoryAsync(string category)
        {
            if (CurrentCategory != category)
            {
                CurrentCategory = category;
                await LoadCategoryAsync(category);
            }
        }

        [RelayCommand]
        public async Task ExportDataAsync()
        {
            try
            {
                if (FilteredReadings.Count == 0)
                {
                    DisplayError = "No data available to export.";
                    return;
                }

                var csvLines = new List<string>();

                // Add CSV header
                csvLines.Add("Timestamp,Category,Temperature,Humidity,WindSpeed,WindDirection,PM25,PM10,NitrogenDioxide,SulphurDioxide,Nitrate,Nitrite,Phosphate,EColi,Enterococci");

                // Add each reading
                foreach (var reading in FilteredReadings)
                {
                    var line = $"{reading.Timestamp:yyyy-MM-dd HH:mm:ss},{reading.Category},{reading.Temperature},{reading.Humidity},{reading.WindSpeed},{reading.WindDirection},{reading.PM25},{reading.PM10},{reading.NitrogenDioxide},{reading.SulphurDioxide},{reading.Nitrate},{reading.Nitrite},{reading.Phosphate},{reading.EColi},{reading.Enterococci}";
                    csvLines.Add(line);
                }

                // Create CSV string
                var csvContent = string.Join(Environment.NewLine, csvLines);

                // Create file path (inside app cache folder)
                var fileName = $"readings_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                // Write file
                File.WriteAllText(filePath, csvContent);

                // Show a success message (you could later trigger sharing)
                Debug.WriteLine($"Exported to {filePath}");
                await Application.Current.MainPage.DisplayAlert("Export Successful", $"Data exported to {filePath}", "OK");
            }
            catch (Exception ex)
            {
                DisplayError = $"Export failed: {ex.Message}";
                Debug.WriteLine($"[TrendsViewModel] Export failed: {ex.Message}");
            }
        }

        private void CalculateTemperatureTrend()
        {
            if (CurrentCategory != "Weather")
            {
                TemperatureTrendSummary = string.Empty; // Hide it
                return;
            }

            if (FilteredReadings.Count == 0)
            {
                TemperatureTrendSummary = "No data.";
                return;
            }

            var ordered = FilteredReadings.OrderBy(r => r.Timestamp).ToList();
            var first = ordered.FirstOrDefault()?.Temperature;
            var last = ordered.LastOrDefault()?.Temperature;

            if (first == null || last == null)
            {
                TemperatureTrendSummary = "Insufficient temperature data.";
                return;
            }

            var difference = last - first;
            if (difference > 0)
                TemperatureTrendSummary = $"Temperature average increased by {difference:F1}°C.";
            else if (difference < 0)
                TemperatureTrendSummary = $"Temperature average decreased by {Math.Abs(difference.Value):F1}°C.";
            else
                TemperatureTrendSummary = "Temperature remained stable.";
        }
        
    }
}