using System.Windows;

namespace MultiRtspViewer;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // We moved heavy init to MainViewModel.InitializeAsync to prevent UI hang
    }
}
