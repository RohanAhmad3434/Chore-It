using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parent_Child.Models
{
    public class TaskItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }
        public string? Priority { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // ✅ AssignedTo: Child (FK to Users table)
        public int? AssignedToId { get; set; }

        [ForeignKey("AssignedToId")]
        public User? AssignedTo { get; set; }

        // ✅ Created by: Parent (FK to Users table)
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        // ✅ Optional reward
        public int? RewardId { get; set; }
        public Reward? Reward { get; set; }

        public bool IsCompleted { get; set; } = false;
        public bool IsApproved { get; set; } = false;

        // ✅ NEW: Completion photo URL (for task verification)
        public string? CompletionPhotoUrl { get; set; }

        // ✅ NEW: DateTime when task was marked completed
        public DateTime? CompletedOn { get; set; }
    }
}


