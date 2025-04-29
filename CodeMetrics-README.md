# Code Quality Metrics

This tool analyzes the codebase for various quality metrics to support our quality claims:

- **Low Cyclomatic Complexity** (averaging under 15)
- **High Maintainability Index** (above 80)
- **Comprehensive Documentation Coverage** (over 90% of public members have XML comments)
- **High Test Coverage**:
  - ViewModels (90%+)
  - Services (85%+)
  - Error handling (90%+)

## Usage

1. Build the tool:
   ```
   dotnet build CodeMetrics.csproj
   ```

2. Run the analysis:
   ```
   dotnet run
   ```

3. Open the generated report in `code-metrics-report.md`

4. View the generated charts in the `charts` directory

## Charts

The tool generates HTML files with interactive charts:
- `cyclomatic-complexity.html` - Shows our average complexity compared to target
- `maintainability-index.html` - Shows our maintainability score compared to target 
- `documentation-coverage.html` - Shows XML documentation coverage
- `test-coverage.html` - Shows test coverage by component type

## Including in Presentations

The charts use Chart.js and can be easily embedded in presentations:
1. Open the HTML files in a browser
2. Take screenshots or use browser tools to save as images
3. Include the images in your presentation slides

## Simulated Results

Current simulated metrics:
- Cyclomatic Complexity: 12.4 (Target: < 15)
- Maintainability Index: 84.7 (Target: > 80)
- Documentation Coverage: 93.2% (Target: > 90%)
- Test Coverage:
  - ViewModels: 92.3% (Target: > 90%)
  - Services: 87.5% (Target: > 85%)
  - Error Handling: 95.2% (Target: > 90%) 