using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MultiRtspViewer.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MultiRtspViewer.ViewModels
{
    public partial class CameraViewModel : ObservableObject, IDisposable
    {
        private readonly LibVLC _libVLC;
        private readonly AppSettings _settings;
        private MediaPlayer _mediaPlayer;
        private CancellationTokenSource? _reconnectCts;
        private bool _isIntentionalStop = false;
        private int _currentBackoffMs = 2000;

        [ObservableProperty]
        private CameraModel model;

        public MediaPlayer MediaPlayer => _mediaPlayer;

        public CameraViewModel(CameraModel model, LibVLC libVLC, AppSettings settings)
        {
            Model = model;
            _libVLC = libVLC;
            _settings = settings;
            _mediaPlayer = new MediaPlayer(_libVLC);

            // Wire up events
            _mediaPlayer.EncounteredError += (s, e) => HandleDisconnection("Error");
            _mediaPlayer.EndReached += (s, e) => HandleDisconnection("Ended");
            
            _mediaPlayer.Opening += (s, e) => UpdateStatus("Connecting...");
            _mediaPlayer.Playing += (s, e) => 
            {
                UpdateStatus("Online");
                ResetReconnect();
            };
            
            _mediaPlayer.Stopped += (s, e) => 
            {
                if (_isIntentionalStop) UpdateStatus("Offline");
                else HandleDisconnection("Stopped");
            };
        }

        private void UpdateStatus(string status)
        {
            // Ensure UI update happens on the main thread
            Application.Current?.Dispatcher.InvokeAsync(() => Model.Status = status);
        }

        private void HandleDisconnection(string reason)
        {
            if (_isIntentionalStop) return;

            // Start reconnection process if not already active
            if (_reconnectCts == null)
            {
                _reconnectCts = new CancellationTokenSource();
                _ = ReconnectLoopAsync(_reconnectCts.Token, reason);
            }
        }

        private async Task ReconnectLoopAsync(CancellationToken token, string reason)
        {
            UpdateStatus($"Retry ({reason})...");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    UpdateStatus($"Retry in {_currentBackoffMs / 1000}s...");
                    await Task.Delay(_currentBackoffMs, token);

                    if (token.IsCancellationRequested) break;

                    UpdateStatus("Reconnecting...");
                    Application.Current?.Dispatcher.Invoke(PlayInternal);

                    // Increase backoff for next attempt (capped at 30s)
                    _currentBackoffMs = Math.Min(_currentBackoffMs * 2, 30000);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    // Ignore internal errors, keep retrying
                }
            }
        }

        private void ResetReconnect()
        {
            _reconnectCts?.Cancel();
            _reconnectCts = null;
            _currentBackoffMs = 2000; // Reset backoff
        }

        [ObservableProperty]
        private bool isFullQuality = false;

        [RelayCommand]
        public void Play()
        {
            _isIntentionalStop = false;
            PlayInternal();
        }

        private void PlayInternal()
        {
            if (string.IsNullOrEmpty(Model.RtspUrl)) return;

            try
            {
                using var media = new Media(_libVLC, new Uri(Model.RtspUrl));
                
                // Base speed/latency options
                media.AddOption($":network-caching={_settings.NetworkCaching}");
                media.AddOption($":clock-jitter={_settings.ClockJitter}");
                media.AddOption(":clock-synchro=0");
                media.AddOption(":no-video-title-show");

                if (_settings.UseHardwareAcceleration)
                {
                    media.AddOption(":avcodec-hw=any");
                }

                if (!IsFullQuality)
                {
                    // Optimization for Grid View
                    media.AddOption($":avcodec-lowres={_settings.AvcodecLowres}");
                    if (_settings.DisableAudioInGrid)
                    {
                        media.AddOption(":no-audio");
                    }

                    if (_settings.LowMemoryMode)
                    {
                        media.AddOption(":avcodec-skip-idct=4");      // Fastest/Lowest RAM IDCT
                        media.AddOption(":avcodec-skiploopfilter=4"); // Skip deblocking
                        media.AddOption(":avcodec-fast");              // Enable fast-path
                        media.AddOption(":no-overlay");                // Disable visual overlays in VLC
                        media.AddOption(":no-snapshot");               // Disable frame capturing
                    }
                }
                
                _mediaPlayer.Play(media);
            }
            catch (Exception)
            {
                UpdateStatus("Invalid URL");
                HandleDisconnection("Bad URL");
            }
        }

        [RelayCommand]
        public void Stop()
        {
            _isIntentionalStop = true;
            ResetReconnect();
            _mediaPlayer.Stop();
        }

        public void Dispose()
        {
            _reconnectCts?.Cancel();
            _mediaPlayer?.Dispose();
        }
    }
}
