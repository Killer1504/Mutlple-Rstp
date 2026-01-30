using System;
using System.Linq;
using MultiRtspViewer.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace MultiRtspViewer.Services.Database
{
    public class DatabaseMigrationService
    {
        private readonly ConfigService _legacyConfigService;

        public DatabaseMigrationService(ConfigService legacyConfigService)
        {
            _legacyConfigService = legacyConfigService;
        }

        public void Initialize()
        {
            using (var db = new AppDbContext())
            {
                // Ensure database exists
                db.Database.EnsureCreated();

                // Check if we need to migrate
                if (!db.Clients.Any() && !db.Cameras.Any())
                {
                    MigrateLegacyData(db);
                }
            }
        }

        private void MigrateLegacyData(AppDbContext db)
        {
            var legacyCameras = _legacyConfigService.LoadCameras();
            if (legacyCameras == null || legacyCameras.Count == 0) return;

            // Create a default client for legacy data
            var defaultClient = new Client
            {
                Name = "Default Client",
                Description = "Imported from legacy configuration",
                CreatedAt = DateTime.Now
            };

            db.Clients.Add(defaultClient);
            db.SaveChanges(); // Save to get the Id

            // Migrate cameras
            foreach (var camModel in legacyCameras)
            {
                var camera = new Camera
                {
                    ClientId = defaultClient.Id,
                    Name = camModel.Name,
                    RtspUrl = camModel.RtspUrl,
                    Position = 0, // Preserve order if possible, but 0 for now
                    Status = "Offline"
                };
                db.Cameras.Add(camera);
            }

            db.SaveChanges();
            Console.WriteLine($"Migrated {legacyCameras.Count} cameras to 'Default Client'");
        }
    }
}
