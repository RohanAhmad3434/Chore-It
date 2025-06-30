using System.ComponentModel.DataAnnotations;

namespace Parent_Child.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Priority { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public int AssignedToId { get; set; } // ✅ FK to child user

        public int? RewardId { get; set; }

        [Required]
        public int UserId { get; set; }   // ✅ FK to parent user (creator)

        public bool IsCompleted { get; set; } = false;
        public bool IsApproved { get; set; } = false;
    }
}
