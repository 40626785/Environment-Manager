using System;

namespace EnvironmentManager.Interfaces;
/// <summary>
/// Enforces a consistent approach for Error Handling
/// </summary>
public interface IErrorHandling
{
    string DisplayError { get; set; }
    void HandleError(Exception e, string message);
}
