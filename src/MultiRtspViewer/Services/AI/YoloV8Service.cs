using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using MultiRtspViewer.Models.AI;

namespace MultiRtspViewer.Services.AI
{
    public class YoloV8Service : IAIProvider
    {
        private InferenceSession? _session;
        private readonly YoloParser _parser = new YoloParser();
        private const int ModelSize = 640;

        public string Name => "YOLOv8 Nano (Standard)";
        public bool IsGpuEnabled { get; private set; } = false;

        public async Task InitializeAsync(string modelPath)
        {
            await Task.Run(() =>
            {
                var options = new SessionOptions();
                try
                {
                    // Try to use DirectML (GPU) first
                    options.AppendExecutionProvider_DML(0);
                    IsGpuEnabled = true;
                }
                catch
                {
                    Console.WriteLine("Warning: GPU (DirectML) not found. Falling back to CPU.");
                    IsGpuEnabled = false;
                }

                _session = new InferenceSession(modelPath, options);
            });
        }

        public async Task<List<DetectionResult>> DetectAsync(Bitmap bitmap)
        {
            if (_session == null) return new List<DetectionResult>();

            return await Task.Run(() =>
            {
                var input = ResizeAndNormalize(bitmap);
                var inputName = _session.InputMetadata.Keys.First();
                
                var inputTensor = new DenseTensor<float>(input, new[] { 1, 3, ModelSize, ModelSize });
                
                using var results = _session.Run(new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(inputName, inputTensor) });
                
                // YOLOv8 output: [1, 84, 8400] usually
                var outputTensor = results.First().AsTensor<float>();
                var outputArray = outputTensor.ToArray();

                // Dimensions from tensor shape
                // shape[1] is channels (84), shape[2] is anchors (8400)
                int channels = outputTensor.Dimensions[1];
                int anchors = outputTensor.Dimensions[2];

                return _parser.Parse(outputArray, channels, anchors);
            });
        }

        // Image Pre-processing
        private float[] ResizeAndNormalize(Bitmap original)
        {
            // 1. Resize to 640x640
            using var resized = new Bitmap(ModelSize, ModelSize);
            using (var g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = InterpolationMode.Bilinear;
                g.DrawImage(original, 0, 0, ModelSize, ModelSize);
            }

            // 2. Convert to Float Array (Normalized 0..1)
            var result = new float[3 * ModelSize * ModelSize];
            
            // Lock bits for speed could be faster, but GetPixel is safer for first pass
            // For production, use LockBits.
            // Let's use a slightly faster unsafe block later if needed. For now simple loop.
            
            // Standard loop is too slow for real-time. We MUST use LockBits or unsafe.
            // Let's do a reasonably fast Managed version for now to avoid unsafe in this turn.
            
            var data = resized.LockBits(new Rectangle(0, 0, ModelSize, ModelSize), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            int stride = data.Stride;
            int bytes = Math.Abs(stride) * ModelSize;
            byte[] rgbValues = new byte[bytes];
            
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, rgbValues, 0, bytes);
            resized.UnlockBits(data);

            // Planar format: RRRRR... GGGGG... BBBBB...
            int pixelCount = ModelSize * ModelSize;

            for (int i = 0; i < pixelCount; i++)
            {
                // Format24bppRgb is stored as B, G, R
                int bIndex = i * 3;
                int gIndex = (i * 3) + 1;
                int rIndex = (i * 3) + 2;

                // Watch out for padding if stride != width * 3
                // Ideally calculate x, y from i
                int x = i % ModelSize;
                int y = i / ModelSize;
                int fileIndex = (y * stride) + (x * 3);

                byte b = rgbValues[fileIndex];
                byte g = rgbValues[fileIndex + 1];
                byte r = rgbValues[fileIndex + 2];

                result[i] = r / 255.0f;              // R plane
                result[pixelCount + i] = g / 255.0f; // G plane
                result[2 * pixelCount + i] = b / 255.0f; // B plane
            }

            return result;
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
