using LibVLCSharp.Shared;
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
        // Initialize LibVLCSharp
        Core.Initialize();

        // Initialize Database & Run Migration
        try 
        {
            var configService = new Services.ConfigService();
            var migrationService = new Services.Database.DatabaseMigrationService(configService);
            migrationService.Initialize();
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"Database initialization failed: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
