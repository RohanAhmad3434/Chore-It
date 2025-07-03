namespace Parent_Child.DTOs
{
    public class UserAchievementDto
    {
        public int Id { get; set; }
        public int AchievementId { get; set; }
        public string AchievementTitle { get; set; }
        public string? IconUrl { get; set; }
        public DateTime DateAchieved { get; set; }
    }
}
