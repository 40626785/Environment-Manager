using System;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace EnvironmentManager.Helpers;

public class Ioc
{
    public static T Resolve<T>() where T : class =>
            App.Services.GetService<T>() ?? throw new InvalidOperationException($"Unable to resolve {typeof(T)}");
}


