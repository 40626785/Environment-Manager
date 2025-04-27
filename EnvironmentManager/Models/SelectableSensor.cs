using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EnvironmentManager.Models
{
    public partial class SelectableSensor : ObservableObject
    {
        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private Sensor sensor = null;
    }
}
