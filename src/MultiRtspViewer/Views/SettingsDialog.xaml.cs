using System.Windows;
using MultiRtspViewer.ViewModels;

namespace MultiRtspViewer.Views
{
    public partial class SettingsDialog : Window
    {
        public SettingsDialog(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ((SettingsViewModel)DataContext).Save();
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
