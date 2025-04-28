using Microsoft.Maui.Graphics;
using EnvironmentManager.ViewModels;

namespace EnvironmentManager.Graphs
{
    public class TemperatureTrendGraph : IDrawable
    {
        private readonly TrendsViewModel _viewModel;

        public TemperatureTrendGraph(TrendsViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var readings = _viewModel.FilteredReadings
                .OrderBy(r => r.Timestamp)
                .Where(r => r.Temperature.HasValue)
                .ToList();

            if (readings.Count < 2)
                return;

            float width = dirtyRect.Width;
            float height = dirtyRect.Height;
            float xStep = width / (readings.Count - 1);

            var temps = readings.Select(r => (float)r.Temperature!.Value).ToList();
            float minTemp = temps.Min();
            float maxTemp = temps.Max();
            float range = Math.Max(1, maxTemp - minTemp);

            // Draw the temperature trend line
            for (int i = 0; i < temps.Count - 1; i++)
            {
                float x1 = i * xStep;
                float y1 = height - ((temps[i] - minTemp) / range) * height;
                float x2 = (i + 1) * xStep;
                float y2 = height - ((temps[i + 1] - minTemp) / range) * height;

                canvas.StrokeColor = Colors.Blue;
                canvas.StrokeSize = 2;
                canvas.DrawLine(x1, y1, x2, y2);
            }
        }
    }
}