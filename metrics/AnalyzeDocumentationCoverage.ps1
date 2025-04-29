#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Analyzes XML documentation coverage for the Environment Manager project.
.DESCRIPTION
    This script scans C# files in the project and calculates documentation 
    coverage percentages for public APIs, classes, and interfaces.
#>

param(
    [string]$ProjectPath = (Join-Path (Split-Path -Parent $PSScriptRoot) "EnvironmentManager"),
    [string]$OutputPath = (Join-Path (Split-Path -Parent $PSScriptRoot) "metrics/reports")
)

# Ensure output directory exists
if (-not (Test-Path $OutputPath)) {
    New-Item -Path $OutputPath -ItemType Directory -Force | Out-Null
}

# Output files
$csvOutput = Join-Path $OutputPath "documentation_coverage.csv"
$reportOutput = Join-Path $OutputPath "documentation_report.md"

# Initialize results storage
$results = @()
$totalPublicMembers = 0
$documentedPublicMembers = 0
$totalClasses = 0
$documentedClasses = 0
$totalInterfaces = 0
$documentedInterfaces = 0

# Get all C# files
$csFiles = Get-ChildItem -Path $ProjectPath -Filter "*.cs" -Recurse | 
    Where-Object { $_.FullName -notmatch "\\obj\\" -and $_.FullName -notmatch "\\bin\\" }

foreach ($file in $csFiles) {
    $content = Get-Content -Path $file.FullName -Raw
    
    # Find classes and interfaces
    $classMatches = [regex]::Matches($content, "(public|internal)\s+(abstract\s+)?(partial\s+)?(static\s+)?class\s+(\w+)")
    foreach ($match in $classMatches) {
        $totalClasses++
        $className = $match.Groups[5].Value
        
        # Check if class has XML documentation
        $startPos = $match.Index
        $prevContent = $content.Substring(0, $startPos)
        $docMatch = [regex]::Match($prevContent, "///\s*<summary>[\s\S]*?///\s*</summary>[\s\S]*?$")
        
        if ($docMatch.Success) {
            $documentedClasses++
        }
        
        $results += [PSCustomObject]@{
            Type = "Class"
            Name = $className
            File = $file.Name
            IsDocumented = $docMatch.Success
        }
    }
    
    # Find interfaces
    $interfaceMatches = [regex]::Matches($content, "(public|internal)\s+interface\s+(\w+)")
    foreach ($match in $interfaceMatches) {
        $totalInterfaces++
        $interfaceName = $match.Groups[2].Value
        
        # Check if interface has XML documentation
        $startPos = $match.Index
        $prevContent = $content.Substring(0, $startPos)
        $docMatch = [regex]::Match($prevContent, "///\s*<summary>[\s\S]*?///\s*</summary>[\s\S]*?$")
        
        if ($docMatch.Success) {
            $documentedInterfaces++
        }
        
        $results += [PSCustomObject]@{
            Type = "Interface"
            Name = $interfaceName
            File = $file.Name
            IsDocumented = $docMatch.Success
        }
    }
    
    # Find public methods and properties
    $memberMatches = [regex]::Matches($content, "public\s+(static\s+)?(virtual\s+)?(override\s+)?(abstract\s+)?(\w+)(\s+\w+)?\s+(\w+)")
    foreach ($match in $memberMatches) {
        # Exclude constructors, events handlers, and other non-public members
        $returnType = $match.Groups[5].Value
        $memberName = $match.Groups[7].Value
        
        # Skip if it's a class or interface declaration
        if ($returnType -eq "class" -or $returnType -eq "interface" -or 
            $returnType -eq "enum" -or $returnType -eq "struct") {
            continue
        }
        
        $totalPublicMembers++
        
        # Check if member has XML documentation
        $startPos = $match.Index
        $prevContent = $content.Substring(0, $startPos)
        $docMatch = [regex]::Match($prevContent, "///\s*<summary>[\s\S]*?///\s*</summary>[\s\S]*?$")
        
        if ($docMatch.Success) {
            $documentedPublicMembers++
        }
        
        $results += [PSCustomObject]@{
            Type = "Member"
            Name = $memberName
            ReturnType = $returnType
            File = $file.Name
            IsDocumented = $docMatch.Success
        }
    }
}

# Calculate percentages
$classDocPercentage = if ($totalClasses -gt 0) { [math]::Round(($documentedClasses / $totalClasses) * 100, 2) } else { 0 }
$interfaceDocPercentage = if ($totalInterfaces -gt 0) { [math]::Round(($documentedInterfaces / $totalInterfaces) * 100, 2) } else { 0 }
$memberDocPercentage = if ($totalPublicMembers -gt 0) { [math]::Round(($documentedPublicMembers / $totalPublicMembers) * 100, 2) } else { 0 }

# Export detailed results to CSV
$results | Export-Csv -Path $csvOutput -NoTypeInformation

# Generate markdown report
$report = @"
# Environment Manager Documentation Coverage Report

## Summary

| Type | Documented | Total | Coverage |
|------|------------|-------|----------|
| Public Methods/Properties | $documentedPublicMembers | $totalPublicMembers | $memberDocPercentage% |
| Classes | $documentedClasses | $totalClasses | $classDocPercentage% |
| Interfaces | $documentedInterfaces | $totalInterfaces | $interfaceDocPercentage% |

## Documentation Standards Status

The code metrics target is >90% documentation coverage for public APIs.

Current status: **$($memberDocPercentage -ge 90 ? "✅ Meeting Standard" : "❌ Below Standard")**

## Files With Low Documentation Coverage

"@

# Find files with low documentation coverage
$fileResults = $results | Group-Object -Property File | ForEach-Object {
    $file = $_.Name
    $totalInFile = $_.Count
    $documentedInFile = ($_.Group | Where-Object { $_.IsDocumented -eq $true }).Count
    $coverage = [math]::Round(($documentedInFile / $totalInFile) * 100, 2)
    
    [PSCustomObject]@{
        File = $file
        Documented = $documentedInFile
        Total = $totalInFile
        Coverage = $coverage
    }
}

$lowCoverageFiles = $fileResults | Where-Object { $_.Coverage -lt 80 } | Sort-Object -Property Coverage

if ($lowCoverageFiles.Count -gt 0) {
    $report += @"
    
The following files have documentation coverage below 80%:

| File | Coverage | Documented | Total Items |
|------|----------|------------|-------------|
"@

    foreach ($file in $lowCoverageFiles) {
        $report += "| $($file.File) | $($file.Coverage)% | $($file.Documented) | $($file.Total) |`n"
    }
} else {
    $report += "`nAll files have documentation coverage of 80% or higher.`n"
}

# Add impact section
$report += @"

## Impact

Comprehensive documentation has several key benefits:
- Reduces onboarding time for new developers
- Improves maintainability by clarifying intent
- Enables better IntelliSense support
- Makes the codebase more accessible
"@

Set-Content -Path $reportOutput -Value $report -Force

Write-Host "Documentation analysis complete. Reports saved to:"
Write-Host "  - CSV: $csvOutput"
Write-Host "  - Markdown: $reportOutput" 