using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiRtspViewer.Models;
using System.Threading.Tasks;

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

        [RelayCommand]
        public async Task TestAlert()
        {
            // Create temporary services for testing
            var logService = new Services.LogService();
            using var alertService = new Services.AlertService(logService);
            
            // Initialize with empty cameras but current settings
            alertService.Initialize(new System.Collections.Generic.List<CameraViewModel>(), Settings);
            
            try
            {
                bool success = await alertService.SendAlertAsync("🔔 This is a TEST alert from MultiRtspViewer.");
                
                if (success)
                    System.Windows.MessageBox.Show("Test alert sent successfully!", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                else
                    System.Windows.MessageBox.Show("Failed to send test alert. Please check your URL/Token and logs.", "Connection Failed", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"Error sending alert: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
