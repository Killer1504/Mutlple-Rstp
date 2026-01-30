using System.Collections.Generic;
using System.Linq;
using MultiRtspViewer.Models.Database;
using MultiRtspViewer.Services.Database;

namespace MultiRtspViewer.Services
{
    public class CameraService
    {
        public List<Camera> GetCamerasByClient(int clientId)
        {
            using (var db = new AppDbContext())
            {
                return db.Cameras
                         .Where(c => c.ClientId == clientId)
                         .OrderBy(c => c.Position)
                         .ToList();
            }
        }

        public Camera AddCamera(int clientId, string name, string rtspUrl)
        {
            using (var db = new AppDbContext())
            {
                var camera = new Camera
                {
                    ClientId = clientId,
                    Name = name,
                    RtspUrl = rtspUrl,
                    Position = 0 // Default, logic for position handling can be added later
                };
                
                // Determine next position
                var count = db.Cameras.Count(c => c.ClientId == clientId);
                camera.Position = count;

                db.Cameras.Add(camera);
                db.SaveChanges();
                return camera;
            }
        }

        public void UpdateCamera(Camera camera)
        {
            using (var db = new AppDbContext())
            {
                db.Cameras.Update(camera);
                db.SaveChanges();
            }
        }

        public void DeleteCamera(int id)
        {
            using (var db = new AppDbContext())
            {
                var camera = db.Cameras.Find(id);
                if (camera != null)
                {
                    db.Cameras.Remove(camera);
                    db.SaveChanges();
                }
            }
        }

        public void UpdateCameraPositions(List<Camera> cameras)
        {
            using (var db = new AppDbContext())
            {
                foreach (var cam in cameras)
                {
                    // Attach if not tracked, or just update directly
                    var existing = db.Cameras.Find(cam.Id);
                    if (existing != null)
                    {
                        existing.Position = cam.Position;
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
