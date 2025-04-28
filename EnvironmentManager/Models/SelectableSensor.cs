using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace EnvironmentManager.Models
{
    public partial class SelectableSensor : ObservableObject
    {
        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private Sensor sensor = null!; 
        public bool IsActive
        {
            get => Sensor?.IsActive ?? false;
            set
            {
                if (Sensor != null)
                {
                    Sensor.IsActive = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
