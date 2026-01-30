using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiRtspViewer.Models;

namespace MultiRtspViewer.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private AppSettings settings;

        public SettingsViewModel(AppSettings currentSettings)
        {
            Settings = currentSettings;
        }

        [RelayCommand]
        public void Save()
        {
            Settings.Save();
        }
    }
}
