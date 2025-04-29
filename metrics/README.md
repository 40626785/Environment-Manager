# Code Quality Metrics

This directory contains tools for measuring, tracking, and reporting code quality metrics for the Environment Manager project.

## Key Metrics Tracked

- **Cyclomatic Complexity**: Measures code complexity by counting the number of linearly independent paths through a program's source code. Our target is an average of less than 15.
- **Maintainability Index**: A composite metric that indicates how maintainable the source code is. Our target is above 80.
- **Documentation Coverage**: Percentage of public API members that have XML documentation. Our target is above 90%.
- **Testing Coverage**: Percentage of code covered by automated tests. Our target is above 85%.

## Available Tools

- **AnalyzeCodeMetrics.ps1**: Analyzes source code for complexity and maintainability metrics
- **AnalyzeDocumentationCoverage.ps1**: Calculates documentation coverage percentages
- **CodeMetricsReport.ps1**: Generates a comprehensive metrics report

## Directory Structure

- `/config`: Configuration files for metrics tools
- `/reports`: Generated reports and raw metrics data

## Usage

To generate a complete metrics report:

```
cd metrics
./CodeMetricsReport.ps1
```

To analyze specific metrics:

```
# For code complexity and maintainability
./AnalyzeCodeMetrics.ps1

# For documentation coverage
./AnalyzeDocumentationCoverage.ps1
```

## CI/CD Integration

These metrics are automatically analyzed during CI/CD pipeline execution. Each pull request is validated against our quality targets before approval.

## How Metrics Translate to Business Value

- **Low Cyclomatic Complexity**: Reduces bugs and maintenance costs
- **High Maintainability Index**: Enables faster feature development and modifications
- **Complete Documentation**: Accelerates developer onboarding
- **Comprehensive Test Coverage**: Ensures system reliability and reduces regressions 