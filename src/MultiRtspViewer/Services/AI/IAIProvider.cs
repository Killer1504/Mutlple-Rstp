using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using MultiRtspViewer.Models.AI;

namespace MultiRtspViewer.Services.AI
{
    public interface IAIProvider : IDisposable
    {
        /// <summary>
        /// Human readable name of the model/provider (e.g. "YOLOv8 Nano")
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether the model handles GPU acceleration
        /// </summary>
        bool IsGpuEnabled { get; }

        /// <summary>
        /// Initialize the model (load weights, warm up)
        /// </summary>
        Task InitializeAsync(string modelPath);

        /// <summary>
        /// Run inference on a frame.
        /// </summary>
        /// <param name="bitmap">The input image.</param>
        /// <returns>List of detections found.</returns>
        Task<List<DetectionResult>> DetectAsync(Bitmap bitmap);
    }
}
