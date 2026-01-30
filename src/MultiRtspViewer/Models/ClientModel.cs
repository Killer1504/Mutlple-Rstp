using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace MultiRtspViewer.Models
{
    public partial class ClientModel : ObservableObject
    {
        public int Id { get; set; } // Database ID

        [ObservableProperty]
        private string name = "";

        [ObservableProperty]
        private string description = "";

        // UI-only properties
        [ObservableProperty]
        private bool isSelected;
        
        [ObservableProperty]
        private int cameraCount;

        public ClientModel() { }
    }
}
