using Microsoft.EntityFrameworkCore;
using MultiRtspViewer.Models.Database;
using MultiRtspViewer.Services.Database;
using System.Collections.Generic;
using System.Linq;

namespace MultiRtspViewer.Services
{
    public class ClientService
    {
        public List<Client> GetAllClients()
        {
            using (var db = new AppDbContext())
            {
                return db.Clients.Include(c => c.Cameras).ToList();
            }
        }

        public Client? GetClientById(int id)
        {
            using (var db = new AppDbContext())
            {
                return db.Clients.Include(c => c.Cameras).FirstOrDefault(c => c.Id == id);
            }
        }

        public Client CreateClient(string name, string description = "")
        {
            using (var db = new AppDbContext())
            {
                var client = new Client
                {
                    Name = name,
                    Description = description
                };
                db.Clients.Add(client);
                db.SaveChanges();
                return client;
            }
        }

        public void UpdateClient(Client client)
        {
            using (var db = new AppDbContext())
            {
                db.Clients.Update(client);
                db.SaveChanges();
            }
        }

        public void DeleteClient(int id)
        {
            using (var db = new AppDbContext())
            {
                var client = db.Clients.Find(id);
                if (client != null)
                {
                    db.Clients.Remove(client);
                    db.SaveChanges();
                }
            }
        }
    }
}
