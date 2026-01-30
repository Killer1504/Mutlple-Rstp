using Microsoft.EntityFrameworkCore;
using MultiRtspViewer.Models.Database;
using System.IO;

namespace MultiRtspViewer.Services.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<Camera> Cameras { get; set; }

        public string DbPath { get; }

        public AppDbContext()
        {
            var folder = System.Environment.SpecialFolder.LocalApplicationData;
            var path = System.Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "MultiRtspViewer", "multirtsp.db");
            
            // Ensure directory exists
            var directory = Path.GetDirectoryName(DbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
