using Parent_Child.Models;

namespace Parent_Child.DTOs
{
    public class ChildDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsGoogleUser { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }

}
