namespace Parent_Child.DTOs
{
    public class AchievementDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int RewardId { get; set; }
        public string RewardTitle { get; set; }
        public int CompletionThreshold { get; set; }
        public string? IconUrl { get; set; }
    }
}
