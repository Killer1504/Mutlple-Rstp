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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                // Delay slightly to ensure VideoView controls are ready
                Dispatcher.InvokeAsync(() =>
                {
                    viewModel.StartAllCameras();
                }, System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }
    }
}