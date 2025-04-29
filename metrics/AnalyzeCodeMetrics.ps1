#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Analyzes the Environment Manager codebase for code quality metrics.
.DESCRIPTION
    This script uses the Microsoft.CodeAnalysis.Metrics package to analyze
    the .NET codebase for cyclomatic complexity, maintainability index,
    and other key code quality metrics.
#>

param(
    [string]$ProjectPath = (Join-Path (Split-Path -Parent $PSScriptRoot) "EnvironmentManager"),
    [string]$OutputPath = (Join-Path (Split-Path -Parent $PSScriptRoot) "metrics/reports")
)

# Install required tools if not already installed
if (-not (Get-Command dotnet-metrics -ErrorAction SilentlyContinue)) {
    Write-Host "Installing Microsoft.CodeAnalysis.Metrics tool..."
    dotnet tool install --global dotnet-metrics
}

# Ensure output directory exists
if (-not (Test-Path $OutputPath)) {
    New-Item -Path $OutputPath -ItemType Directory -Force | Out-Null
}

# Analyze metrics for the project
$jsonOutput = Join-Path $OutputPath "metrics_detailed.json"
$htmlOutput = Join-Path $OutputPath "metrics_report.html"

Write-Host "Analyzing code metrics for $ProjectPath..."
dotnet metrics $ProjectPath -o $jsonOutput --format json

# Generate HTML report
Write-Host "Generating HTML report..."
$jsonData = Get-Content $jsonOutput | ConvertFrom-Json

# Function to calculate average complexity
function Get-AverageComplexity {
    param($metrics)
    
    $totalComplexity = 0
    $methodCount = 0
    
    foreach ($class in $metrics.CodeMetricsClasses) {
        foreach ($method in $class.MethodMetrics) {
            $totalComplexity += $method.CyclomaticComplexity
            $methodCount++
        }
    }
    
    if ($methodCount -gt 0) {
        return [math]::Round($totalComplexity / $methodCount, 2)
    }
    
    return 0
}

# Function to calculate average maintainability
function Get-AverageMaintainability {
    param($metrics)
    
    $totalMaintainability = 0
    $methodCount = 0
    
    foreach ($class in $metrics.CodeMetricsClasses) {
        foreach ($method in $class.MethodMetrics) {
            $totalMaintainability += $method.MaintainabilityIndex
            $methodCount++
        }
    }
    
    if ($methodCount -gt 0) {
        return [math]::Round($totalMaintainability / $methodCount, 2)
    }
    
    return 0
}

# Generate a simple HTML report
$avgComplexity = Get-AverageComplexity $jsonData
$avgMaintainability = Get-AverageMaintainability $jsonData

$htmlContent = @"
<!DOCTYPE html>
<html>
<head>
    <title>Environment Manager Code Metrics Report</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; }
        h1 { color: #333; }
        .summary { margin: 20px 0; }
        .metric { margin-bottom: 10px; }
        .metric-name { font-weight: bold; }
        .good { color: green; }
        .warning { color: orange; }
        .bad { color: red; }
        table { border-collapse: collapse; width: 100%; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
        tr:nth-child(even) { background-color: #f9f9f9; }
    </style>
</head>
<body>
    <h1>Environment Manager Code Metrics Report</h1>
    
    <div class="summary">
        <h2>Summary</h2>
        <div class="metric">
            <span class="metric-name">Average Cyclomatic Complexity:</span> 
            <span class="$($avgComplexity -lt 15 ? 'good' : ($avgComplexity -lt 20 ? 'warning' : 'bad'))">$avgComplexity</span>
        </div>
        <div class="metric">
            <span class="metric-name">Average Maintainability Index:</span> 
            <span class="$($avgMaintainability -gt 80 ? 'good' : ($avgMaintainability -gt 70 ? 'warning' : 'bad'))">$avgMaintainability</span>
        </div>
    </div>
    
    <h2>Detailed Class Metrics</h2>
    <table>
        <tr>
            <th>Namespace</th>
            <th>Class</th>
            <th>Maintainability Index</th>
            <th>Cyclomatic Complexity</th>
            <th>Depth of Inheritance</th>
            <th>Class Coupling</th>
        </tr>
"@

foreach ($assembly in $jsonData) {
    foreach ($namespace in $assembly.Namespaces) {
        foreach ($class in $namespace.Types) {
            $htmlContent += @"
        <tr>
            <td>$($namespace.Name)</td>
            <td>$($class.Name)</td>
            <td class="$($class.MaintainabilityIndex -gt 80 ? 'good' : ($class.MaintainabilityIndex -gt 70 ? 'warning' : 'bad'))">$($class.MaintainabilityIndex)</td>
            <td class="$($class.CyclomaticComplexity -lt 15 ? 'good' : ($class.CyclomaticComplexity -lt 20 ? 'warning' : 'bad'))">$($class.CyclomaticComplexity)</td>
            <td>$($class.DepthOfInheritance)</td>
            <td>$($class.ClassCoupling)</td>
        </tr>
"@
        }
    }
}

$htmlContent += @"
    </table>
</body>
</html>
"@

Set-Content -Path $htmlOutput -Value $htmlContent -Force

Write-Host "Analysis complete. Reports saved to:"
Write-Host "  - JSON: $jsonOutput"
Write-Host "  - HTML: $htmlOutput" 