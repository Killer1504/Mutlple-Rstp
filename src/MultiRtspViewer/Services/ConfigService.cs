using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MultiRtspViewer.Services
{
    public class ConfigService
    {
        private readonly string _configPath;

        public ConfigService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "MultiRtspViewer");
            Directory.CreateDirectory(folder);
            _configPath = Path.Combine(folder, "cameras.json");
        }

        public void SaveCameras(IEnumerable<Models.CameraModel> cameras)
        {
            try
            {
                string json = JsonConvert.SerializeObject(cameras, Formatting.Indented);
                File.WriteAllText(_configPath, json);
            }
            catch (Exception)
            {
                // Log error in production
            }
        }

        public List<Models.CameraModel> LoadCameras()
        {
            try
            {
                if (!File.Exists(_configPath)) return new List<Models.CameraModel>();

                string json = File.ReadAllText(_configPath);
                return JsonConvert.DeserializeObject<List<Models.CameraModel>>(json) ?? new List<Models.CameraModel>();
            }
            catch
            {
                return new List<Models.CameraModel>();
            }
        }
    }
}
