using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiRtspViewer.Models.Database
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public ICollection<Camera> Cameras { get; set; } = new List<Camera>();
    }
}
