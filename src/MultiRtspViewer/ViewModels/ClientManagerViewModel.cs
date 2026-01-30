using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiRtspViewer.Models;
using MultiRtspViewer.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MultiRtspViewer.ViewModels
{
    public partial class ClientManagerViewModel : ObservableObject
    {
        private readonly ClientService _clientService;

        [ObservableProperty]
        private ObservableCollection<ClientModel> clients = new();

        public ClientManagerViewModel()
        {
            _clientService = new ClientService();
            LoadClients();
        }

        public void LoadClients()
        {
            var dbClients = _clientService.GetAllClients();
            Clients.Clear();
            foreach (var dbClient in dbClients)
            {
                Clients.Add(new ClientModel
                {
                    Id = dbClient.Id,
                    Name = dbClient.Name,
                    Description = dbClient.Description,
                    CameraCount = dbClient.Cameras.Count
                });
            }
        }

        [RelayCommand]
        public void RenameClient(ClientModel client)
        {
            // Simple input dialog for rename
            var inputDialog = new Views.AddClientDialog(); // Reuse creation dialog for now, or just InputBox?
            // Reusing AddClientDialog gives good UI, but title says "New Client". 
            // Better to have a dedicated Rename logic or generic InputDialog.
            
            // For MVP: I'll use a Microsoft.VisualBasic.Interaction.InputBox equivalent or just create a quick custom one?
            // Actually, let's just use AddClientDialog but change Title/ViewModel if we could.
            // Since AddClientDialog is tightly coupled to AddClientViewModel, let's make a new EditClientDialog later?
            
            // To be fast, I'll update client directly? No, UI first.
            // I'll assume we can use a simple InputBox for now, OR simply create a 'ClientManager' view where the Name is a TextBox.
            
            // Let's make the list item have an "Edit" button that turns the TextBlock into a TextBox?  Too complex for now.
            // I'll create a simple Rename logic: 
            // Just Prompt for new name.
            
            // Using AddClientDialog as a proxy for Rename:
            var vm = new AddClientViewModel { ClientName = client.Name };
            var dialog = new Views.AddClientDialog 
            { 
                DataContext = vm, 
                Title = "Rename Client",
                Owner = Application.Current.MainWindow // Ensure centering works
            }; 
            
            if (dialog.ShowDialog() == true)
            {
                client.Name = vm.ClientName;
                var dbClient = _clientService.GetClientById(client.Id);
                if (dbClient != null)
                {
                    dbClient.Name = client.Name;
                    _clientService.UpdateClient(dbClient);
                }
            }
        }

        [RelayCommand]
        public void DeleteClient(ClientModel client)
        {
            var result = MessageBox.Show(
                $"Delete '{client.Name}' and all {client.CameraCount} cameras?",
                "Delete Client",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _clientService.DeleteClient(client.Id);
                Clients.Remove(client);
            }
        }
    }
}
