using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Parent_Child.DTOs
{
    public class AchievementUpdateDto
    {
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public int RewardId { get; set; }

        [Required]
        public int CompletionThreshold { get; set; }

        // ✅ Optional updated icon
        public IFormFile? IconFile { get; set; }
    }
}
