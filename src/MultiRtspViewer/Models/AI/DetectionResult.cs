namespace MultiRtspViewer.Models.AI
{
    public class DetectionResult
    {
        public string Label { get; set; } = string.Empty;
        public float Confidence { get; set; }
        
        // Bounding box coordinates in normalized specific form (0.0 to 1.0)
        // or pixel coordinates? Let's use normalized to be resolution independent.
        // Or wait, UI usually needs pixel. Let's stick effectively to pixels or generic rect.
        // Let's use x, y, w, h in pure float pixels relative to the frame sent.
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public DetectionResult(string label, float confidence, float x, float y, float w, float h)
        {
            Label = label;
            Confidence = confidence;
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
    }
}
