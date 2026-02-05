using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiRtspViewer.Models.Database
{
    public class Camera
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string RtspUrl { get; set; } = string.Empty;

        public int Position { get; set; } = 0;

        // Optional metadata
        public string Status { get; set; } = "Offline"; 

        // AI Configuration (Added for AI Detection feature)
        public bool IsAiEnabled { get; set; } = false;
        public bool DetectPerson { get; set; } = true;
        public bool DetectVehicle { get; set; } = false;
    }
}
