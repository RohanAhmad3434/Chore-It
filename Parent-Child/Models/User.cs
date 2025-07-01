using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Parent_Child.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } = "Child";

        public bool IsGoogleUser { get; set; } = false;

        public int? ParentId { get; set; }
        public User? Parent { get; set; }
        public List<User>? Children { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string? Relation { get; set; }

        public List<TaskItem>? Tasks { get; set; }
       
    }

}

