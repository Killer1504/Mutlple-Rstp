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
        try 
        {
            base.OnStartup(e);
            
            // Initialize LibVLCSharp
            Core.Initialize();

            // Initialize Database & Run Migration
            var configService = new Services.ConfigService();
            var migrationService = new Services.Database.DatabaseMigrationService(configService);
            migrationService.Initialize();
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"Application failed to start.\n\nError: {ex.Message}", 
                "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            System.Environment.Exit(1);
        }
    }
}
