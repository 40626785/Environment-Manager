using System;
using Microsoft.Extensions.DependencyInjection;

namespace EnvironmentManager.Helpers
{
    /// <summary>
    /// Represents the Ioc database context.
    /// </summary>
    public static class Ioc
    {
        public static T Resolve<T>() where T : class =>
            App.Services.GetService<T>() ?? throw new InvalidOperationException($"Unable to resolve {typeof(T)}");
    }
}