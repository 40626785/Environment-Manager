# Environment Manager Code Metrics

This directory contains code metrics analysis tools and reports for the Environment Manager project.

## Overview

The code metrics system provides comprehensive quality analysis for the codebase, including:

- Code coverage (line and branch)
- Documentation coverage
- Cyclomatic complexity
- Maintainability index
- Lines of code statistics

## GitHub Actions Workflow

The metrics are generated automatically through a GitHub Actions workflow (`.github/workflows/code-metrics.yml`). The workflow runs on:

- Each push to feature/code-metrics, main, or develop branches
- Pull requests to main/develop branches
- Manual trigger via GitHub Actions UI

## Reports Generated

The workflow generates the following reports:

1. **Metrics Summary** - Overview of all key metrics with targets
2. **Documentation Coverage Report** - Details of documentation coverage by file
3. **Code Complexity Report** - Detailed complexity analysis by file
4. **Test Coverage Report** - Test coverage details generated using dotnet-coverage

## Test Coverage

The workflow attempts to collect test coverage data if the build succeeds. If the build fails, other code metrics are still collected, but test coverage will be reported as 0%.

When test coverage is collected, the following metrics are reported:
- **Line Coverage** - Percentage of code lines executed by tests
- **Branch Coverage** - Percentage of code branches (if/else, switch cases, etc.) executed by tests

The workflow uses:
- **dotnet-coverage** to collect coverage data during test execution
- **ReportGenerator** to generate HTML and Markdown reports

## Contributing to Metrics

When contributing to the project, aim to maintain or improve these metrics:

- Line Coverage: >80%
- Documentation Coverage: >90% 
- Cyclomatic Complexity: <15
- Maintainability Index: >80

## Manual Analysis

To run the metrics analysis locally:

1. Install required tools:
   ```
   dotnet tool install --global dotnet-reportgenerator-globaltool
   dotnet tool install --global dotnet-coverage
   ```

2. Run the tests with coverage:
   ```
   dotnet-coverage collect -f xml -o metrics/reports/coverage.xml "dotnet test EnvironmentManager.Test/EnvironmentManager.Test.csproj --configuration Release"
   ```

3. Generate the coverage report:
   ```
   reportgenerator -reports:metrics/reports/coverage.xml -targetdir:metrics/reports/coverage -reporttypes:Html;MarkdownSummary
   ```

4. View the reports in the `metrics/reports` directory

For more detailed analysis, review the GitHub Actions workflow file for the complete set of PowerShell scripts used to analyze code quality. 