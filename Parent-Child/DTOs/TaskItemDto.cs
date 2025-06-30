namespace Parent_Child.DTOs
{
    public class TaskItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Priority { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }

        public int? RewardId { get; set; }
        public string? RewardTitle { get; set; }

        public bool IsCompleted { get; set; } = false;
        public bool IsApproved { get; set; } = false;
    }
}
