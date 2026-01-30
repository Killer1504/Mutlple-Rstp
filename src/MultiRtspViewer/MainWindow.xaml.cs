using MultiRtspViewer.ViewModels;
using System.Windows;

namespace MultiRtspViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Start all cameras after UI is fully loaded
            Loaded += MainWindow_Loaded;
            
            // Eco Mode: Pause/Resume on minimize/restore
            StateChanged += MainWindow_StateChanged;
            
            // Position notification popup
            SizeChanged += (s, e) => PositionNotificationPopup();
            Loaded += (s, e) => PositionNotificationPopup();
        }

        private void PositionNotificationPopup()
        {
            // Position at bottom-right corner with 20px margins
            NotificationPopup.HorizontalOffset = ActualWidth - 270; // 250px width + 20px margin
            NotificationPopup.VerticalOffset = ActualHeight - 110; // ~90px height + 20px margin
        }

        private void MainWindow_StateChanged(object? sender, System.EventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                if (WindowState == WindowState.Minimized && viewModel.Settings.EcoMode)
                {
                    // Pause all streams to save resources
                    viewModel.PauseAllCameras();
                }
                else if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
                {
                    // Resume all streams
                    viewModel.ResumeAllCameras();
                }
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                await viewModel.InitializeAsync();
                
                // Delay slightly to ensure VideoView controls are ready
                Dispatcher.InvokeAsync(() =>
                {
                    viewModel.StartAllCameras();
                }, System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                // Ensure proper disposal
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.Dispose();
                }
            }
        }
    }
}