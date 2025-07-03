using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parent_Child.Models
{
    public class UserAchievement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public int AchievementId { get; set; }
        [ForeignKey("AchievementId")]
        public Achievement Achievement { get; set; }

        public DateTime DateAchieved { get; set; } = DateTime.UtcNow;
    }
}
