﻿﻿using EnvironmentManager.Views; // Add this using statement

namespace EnvironmentManager;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(SensorPage), typeof(SensorPage));
        // Add other routes here if needed in the future
	}
}
