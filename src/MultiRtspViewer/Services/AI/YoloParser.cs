using System;
using System.Collections.Generic;
using System.Linq;
using MultiRtspViewer.Models.AI;

namespace MultiRtspViewer.Services.AI
{
    public class YoloParser
    {
        // Standard COCO Labels (Index 0 is Person)
        private static readonly string[] Labels = new[]
        {
            "person", "bicycle", "car", "motorcycle", "airplane", "bus", "train", "truck", "boat", "traffic light",
            "fire hydrant", "stop sign", "parking meter", "bench", "bird", "cat", "dog", "horse", "sheep", "cow",
            "elephant", "bear", "zebra", "giraffe", "backpack", "umbrella", "handbag", "tie", "suitcase", "frisbee",
            "skis", "snowboard", "sports ball", "kite", "baseball bat", "baseball glove", "skateboard", "surfboard",
            "tennis racket", "bottle", "wine glass", "cup", "fork", "knife", "spoon", "bowl", "banana", "apple",
            "sandwich", "orange", "broccoli", "carrot", "hot dog", "pizza", "donut", "cake", "chair", "couch",
            "potted plant", "bed", "dining table", "toilet", "tv", "laptop", "mouse", "remote", "keyboard", "cell phone",
            "microwave", "oven", "toaster", "sink", "refrigerator", "book", "clock", "vase", "scissors", "teddy bear",
            "hair drier", "toothbrush"
        };

        public List<DetectionResult> Parse(float[] output, int dimensions, int rows, float confidenceThreshold = 0.3f)
        {
            // Output layout is usually [1, 4+Classes, 8400]
            // We receive it flattened, but conceptually it's Transposed? 
            // Standard YOLOv8 export: [Batch=1, Channels=84, Anchors=8400]
            // Channels: 0=x, 1=y, 2=w, 3=h, 4=class0, 5=class1...
            
            var detections = new List<DetectionResult>();

            // Stride is the number of boxes (Anchors)
            int numAnchors = rows; // 8400
            int numChannels = dimensions; // 84

            for (int i = 0; i < numAnchors; i++)
            {
                // Find the best class confidence
                float maxScore = 0;
                int maxClassId = -1;

                // Classes start at index 4
                for (int c = 0; c < numChannels - 4; c++)
                {
                    // Access items in column-major-like format if flattened by C# usually:
                    // But usually FloatArray output is standard row-major.
                    // Let's assume standard [1, 84, 8400] flattening:
                    // index = (channel * numAnchors) + anchorIndex
                    
                    float score = output[((4 + c) * numAnchors) + i];
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxClassId = c;
                    }
                }

                if (maxScore < confidenceThreshold) continue;

                // Extract Box
                float x = output[(0 * numAnchors) + i];
                float y = output[(1 * numAnchors) + i];
                float w = output[(2 * numAnchors) + i];
                float h = output[(3 * numAnchors) + i];

                // YOLO returns center (x,y) and width/height relative to 640 image
                // We convert to top-left (x,y)
                float xMin = x - (w / 2);
                float yMin = y - (h / 2);

                if (maxClassId >= 0 && maxClassId < Labels.Length)
                {
                    detections.Add(new DetectionResult(Labels[maxClassId], maxScore, xMin, yMin, w, h));
                }
            }

            return NMS(detections);
        }

        // Non-Maximum Suppression
        private List<DetectionResult> NMS(List<DetectionResult> boxes, float iouThreshold = 0.45f)
        {
            var results = new List<DetectionResult>();
            var sorted = boxes.OrderByDescending(b => b.Confidence).ToList();

            while (sorted.Count > 0)
            {
                var current = sorted[0];
                results.Add(current);
                sorted.RemoveAt(0);

                for (int i = sorted.Count - 1; i >= 0; i--)
                {
                    if (ComputeIOU(current, sorted[i]) > iouThreshold)
                    {
                        sorted.RemoveAt(i);
                    }
                }
            }
            return results;
        }

        private float ComputeIOU(DetectionResult boxA, DetectionResult boxB)
        {
            float intersectionX1 = Math.Max(boxA.X, boxB.X);
            float intersectionY1 = Math.Max(boxA.Y, boxB.Y);
            float intersectionX2 = Math.Min(boxA.X + boxA.Width, boxB.X + boxB.Width);
            float intersectionY2 = Math.Min(boxA.Y + boxA.Height, boxB.Y + boxB.Height);

            float intersectionArea = Math.Max(0, intersectionX2 - intersectionX1) * Math.Max(0, intersectionY2 - intersectionY1);
            float boxAArea = boxA.Width * boxA.Height;
            float boxBArea = boxB.Width * boxB.Height;

            return intersectionArea / (boxAArea + boxBArea - intersectionArea);
        }
    }
}
