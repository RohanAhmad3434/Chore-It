using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parent_Child.Models
{
    public class Achievement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } // e.g. "Gold Star Rewarder"

        public string? Description { get; set; } // e.g. "Completed Reward1 three times"

        // ✅ The Reward this achievement is based on
        public int RewardId { get; set; }

        [ForeignKey("RewardId")]
        public Reward Reward { get; set; }

        // ✅ The threshold e.g. 3 times
        public int CompletionThreshold { get; set; }

        public string? IconUrl { get; set; }
    }
}
