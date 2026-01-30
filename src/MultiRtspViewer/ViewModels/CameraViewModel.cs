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
        
        // Exponential backoff intervals: 1s, 2s, 5s, 10s
        private readonly int[] _backoffIntervalsMs = { 1000, 2000, 5000, 10000 };
        private int _currentBackoffIndex = 0;

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
            _mediaPlayer.EncounteredError += (s, e) => HandleDisconnection("Error", ConnectionStatus.Error);
            _mediaPlayer.EndReached += (s, e) => HandleDisconnection("Ended", ConnectionStatus.Offline);
            
            _mediaPlayer.Opening += (s, e) => UpdateStatus("Connecting...", ConnectionStatus.Connecting);
            _mediaPlayer.Playing += (s, e) => 
            {
                UpdateStatus("Online", ConnectionStatus.Connected);
                UpdateHeartbeat();
                ResetReconnect();
            };
            
            _mediaPlayer.Stopped += (s, e) => 
            {
                if (_isIntentionalStop) 
                    UpdateStatus("Offline", ConnectionStatus.Offline);
                else 
                    HandleDisconnection("Stopped", ConnectionStatus.Offline);
            };
        }

        private void UpdateStatus(string status, ConnectionStatus connectionStatus)
        {
            // Ensure UI update happens on the main thread
            Application.Current?.Dispatcher.InvokeAsync(() => 
            {
                Model.Status = status;
                Model.ConnectionStatus = connectionStatus;
            });
        }

        private void UpdateHeartbeat()
        {
            Application.Current?.Dispatcher.InvokeAsync(() => 
            {
                Model.LastHeartbeat = DateTime.Now;
            });
        }

        private void HandleDisconnection(string reason, ConnectionStatus status)
        {
            if (_isIntentionalStop) return;

            UpdateStatus($"Disconnected ({reason})", status);

            // Start reconnection process if not already active
            if (_reconnectCts == null)
            {
                _reconnectCts = new CancellationTokenSource();
                _ = ReconnectLoopAsync(_reconnectCts.Token, reason);
            }
        }

        private async Task ReconnectLoopAsync(CancellationToken token, string reason)
        {
            UpdateStatus($"Reconnecting...", ConnectionStatus.Reconnecting);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    // Increment reconnect attempts (async to avoid UI blocking)
                    await Application.Current?.Dispatcher.InvokeAsync(() => Model.ReconnectAttempts++);

                    // Get current backoff interval
                    int backoffMs = _backoffIntervalsMs[_currentBackoffIndex];
                    UpdateStatus($"Retry in {backoffMs / 1000}s... (Attempt {Model.ReconnectAttempts})", ConnectionStatus.Reconnecting);
                    
                    await Task.Delay(backoffMs, token);

                    if (token.IsCancellationRequested) break;

                    UpdateStatus("Attempting reconnect...", ConnectionStatus.Connecting);
                    
                    // Call the async play method (it already runs on background thread with timeout)
                    await PlayInternalAsync();

                    // Move to next backoff interval (capped at last interval)
                    _currentBackoffIndex = Math.Min(_currentBackoffIndex + 1, _backoffIntervalsMs.Length - 1);
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
            _currentBackoffIndex = 0; // Reset backoff to first interval
            
            Application.Current?.Dispatcher.InvokeAsync(() => 
            {
                Model.ReconnectAttempts = 0;
            });
        }

        [ObservableProperty]
        private bool isFullQuality = false;

        [RelayCommand]
        public void Play()
        {
            _isIntentionalStop = false;
            _ = PlayInternalAsync(); // Fire and forget
        }

        private async Task PlayInternalAsync()
        {
            if (string.IsNullOrEmpty(Model.RtspUrl)) return;

            try
            {
                // Run the entire play operation on a background thread with timeout
                var playTask = Task.Run(() =>
                {
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
                        // Will be handled by timeout or event handlers
                    }
                });

                // Wait for play operation with 5 second timeout
                var completedTask = await Task.WhenAny(playTask, Task.Delay(5000));
                
                if (completedTask != playTask)
                {
                    // Timeout occurred
                    UpdateStatus("Connection timeout", ConnectionStatus.Error);
                    HandleDisconnection("Timeout", ConnectionStatus.Error);
                }
            }
            catch (Exception)
            {
                UpdateStatus("Invalid URL", ConnectionStatus.Error);
                HandleDisconnection("Bad URL", ConnectionStatus.Error);
            }
        }

        private void PlayInternal()
        {
            // Legacy synchronous wrapper for compatibility
            _ = PlayInternalAsync();
        }

        [RelayCommand]
        public void Stop()
        {
            _isIntentionalStop = true;
            ResetReconnect();
            _mediaPlayer.Stop();
        }

        // Eco Mode: Pause stream to save resources
        public void Pause()
        {
            if (_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Pause();
                UpdateStatus("Paused (Eco Mode)", ConnectionStatus.Connected);
            }
        }

        // Eco Mode: Resume stream
        public void Resume()
        {
            if (_mediaPlayer.CanPause && !_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Play();
                UpdateStatus("Online", ConnectionStatus.Connected);
                UpdateHeartbeat();
            }
        }

        public void Dispose()
        {
            _reconnectCts?.Cancel();
            _mediaPlayer?.Dispose();
        }
    }
}
