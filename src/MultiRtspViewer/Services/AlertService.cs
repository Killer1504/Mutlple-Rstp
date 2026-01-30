using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MultiRtspViewer.Models;
using MultiRtspViewer.ViewModels;
using Newtonsoft.Json;

namespace MultiRtspViewer.Services
{
    public class AlertService : IDisposable
    {
        private readonly System.Timers.Timer _timer;
        private readonly ILogService _logService;
        private IEnumerable<CameraViewModel> _cameras;
        private AppSettings _settings;
        private readonly HttpClient _httpClient;
        
        // Track which cameras we have already sent an alert for (during this specific outage)
        // Key: Camera ID, Value: Time alert was sent
        private readonly Dictionary<string, DateTime> _activeAlerts = new();

        public AlertService(ILogService logService)
        {
            _logService = logService;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);

            // Check every 1 minute
            _timer = new System.Timers.Timer(60000);
            _timer.Elapsed += async (s, e) => await CheckMonitoringAsync();
        }

        public void Initialize(IEnumerable<CameraViewModel> cameras, AppSettings settings)
        {
            _cameras = cameras;
            _settings = settings;
            _timer.Start();
            _logService.Log("Alert Monitoring Service started.");
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private async Task CheckMonitoringAsync()
        {
            if (_cameras == null || _settings == null || !_settings.AlertsEnabled) return;

            foreach (var cam in _cameras)
            {
                // Logic:
                // 1. Is Camera Offline?
                // 2. Is it EcoMode paused? (If so, ignore)
                // 3. Has it been offline > Threshold?
                // 4. Have we already alerted?

                if (cam.Model.ConnectionStatus == ConnectionStatus.Connected)
                {
                    // If we had an active alert for this camera, clear it (it recovered!)
                    if (_activeAlerts.ContainsKey(cam.Model.Id))
                    {
                        var downtime = DateTime.Now - _activeAlerts[cam.Model.Id];
                        _logService.Log($"Camera '{cam.Model.Name}' recovered after {downtime.TotalMinutes:F1} min.");
                        _activeAlerts.Remove(cam.Model.Id);
                        
                        // Optional: Send "Recovered" alert? (Maybe later)
                    }
                    continue;
                }

                // If not connected...
                if (cam.Model.ConnectionStatus == ConnectionStatus.Error || cam.Model.ConnectionStatus == ConnectionStatus.Offline)
                {
                    // Check if we have a LastHeartbeat (if null, maybe it never connected, so don't alert yet? or alert immediately?)
                    // Let's assume if LastHeartbeat is null, we use the time the app started? No, safer to ignore until first connect.
                    if (cam.Model.LastHeartbeat == null) continue;

                    var timeSinceHeartbeat = DateTime.Now - cam.Model.LastHeartbeat.Value;
                    
                    // If outage > threshold
                    if (timeSinceHeartbeat.TotalMinutes >= _settings.AlertThresholdMinutes)
                    {
                        // Check if already alerted
                        if (!_activeAlerts.ContainsKey(cam.Model.Id))
                        {
                            // FIRE ALERT
                            var msg = $"🚨 **ALERT**: Camera '{cam.Model.Name}' has been down for {timeSinceHeartbeat.TotalMinutes:F0} minutes.";
                            _logService.LogWarning($"Triggering External Alert: {msg}");
                            
                            bool success = await SendAlertAsync(msg);
                            
                            if (success)
                            {
                                _activeAlerts[cam.Model.Id] = DateTime.Now; // Mark as sent
                            }
                        }
                    }
                }
            }
        }

        public async Task<bool> SendAlertAsync(string message)
        {
            try
            {
                switch (_settings.Provider)
                {
                    case AlertProvider.Discord:
                        return await SendDiscordAlert(message);
                    case AlertProvider.Telegram:
                        return await SendTelegramAlert(message);
                    case AlertProvider.GenericWebhook:
                        return await SendGenericAlert(message);
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                _logService.LogError("Failed to send external alert", ex);
                return false;
            }
        }

        private async Task<bool> SendDiscordAlert(string message)
        {
            if (string.IsNullOrWhiteSpace(_settings.WebhookUrl)) return false;

            var payload = new { content = message };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_settings.WebhookUrl, content);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> SendTelegramAlert(string message)
        {
            if (string.IsNullOrWhiteSpace(_settings.TelegramBotToken) || string.IsNullOrWhiteSpace(_settings.TelegramChatId)) return false;

            var url = $"https://api.telegram.org/bot{_settings.TelegramBotToken}/sendMessage";
            var payload = new { chat_id = _settings.TelegramChatId, text = message };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> SendGenericAlert(string message)
        {
            if (string.IsNullOrWhiteSpace(_settings.WebhookUrl)) return false;

            var payload = new 
            { 
                event_type = "camera_offline", 
                message = message, 
                timestamp = DateTime.Now 
            };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_settings.WebhookUrl, content);
            return response.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _httpClient?.Dispose();
        }
    }
}
