﻿using EnvironmentManager.Views; // Add this using statement
using Microsoft.Maui.Controls;

namespace EnvironmentManager;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute("home", typeof(HomePage));
        Routing.RegisterRoute("maintenance", typeof(AllMaintenancePage));
        Routing.RegisterRoute("sensors", typeof(SensorPage));
        Routing.RegisterRoute("about", typeof(AboutPage));
        Routing.RegisterRoute("EditSensor", typeof(EditSensorPage));
        Routing.RegisterRoute("AddSensor", typeof(AddSensorPage));
	}
}
