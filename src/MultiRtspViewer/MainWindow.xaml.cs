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