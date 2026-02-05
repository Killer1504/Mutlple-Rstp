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

                // Run migrations for schema updates
                MigrateAiColumns(db);

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

        private void MigrateAiColumns(AppDbContext db)
        {
            try
            {
                // Check each column individually using SQLite PRAGMA
                var conn = db.Database.GetDbConnection();
                bool hasConnectionOpen = conn.State == System.Data.ConnectionState.Open;
                if (!hasConnectionOpen) conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "PRAGMA table_info(Cameras)";
                    var columns = new System.Collections.Generic.List<string>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(reader["name"].ToString() ?? "");
                        }
                    }

                    if (!columns.Contains("IsAiEnabled"))
                    {
                        Console.WriteLine("Adding column: IsAiEnabled");
                        db.Database.ExecuteSqlRaw("ALTER TABLE Cameras ADD COLUMN IsAiEnabled INTEGER NOT NULL DEFAULT 0");
                    }
                    if (!columns.Contains("DetectPerson"))
                    {
                        Console.WriteLine("Adding column: DetectPerson");
                        db.Database.ExecuteSqlRaw("ALTER TABLE Cameras ADD COLUMN DetectPerson INTEGER NOT NULL DEFAULT 1");
                    }
                    if (!columns.Contains("DetectVehicle"))
                    {
                        Console.WriteLine("Adding column: DetectVehicle");
                        db.Database.ExecuteSqlRaw("ALTER TABLE Cameras ADD COLUMN DetectVehicle INTEGER NOT NULL DEFAULT 0");
                    }
                }

                if (!hasConnectionOpen) conn.Close();
                Console.WriteLine("AI column migration check completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration Error: {ex.Message}");
                // Non-fatal, but we might want to log this properly
            }
        }
    }
}
