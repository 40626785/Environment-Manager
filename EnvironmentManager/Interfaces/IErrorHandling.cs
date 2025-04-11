using System;

namespace EnvironmentManager.Interfaces;

public interface IErrorHandling
{
    string DisplayError { get; set; }
    void HandleError(Exception e, string message);
}
