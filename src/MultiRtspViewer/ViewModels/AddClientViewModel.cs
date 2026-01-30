using CommunityToolkit.Mvvm.ComponentModel;

namespace MultiRtspViewer.ViewModels
{
    public partial class AddClientViewModel : ObservableObject
    {
        [ObservableProperty]
        private string clientName = "";

        // Should check validity?
        // Basic validation: Not empty
    }
}
