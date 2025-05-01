using System;

namespace EnvironmentManager.Interfaces;

public interface ILoggingService
{
    Task LogErrorAsync(string errorMessage);
    Task LogMessageAsync(string message);
}
