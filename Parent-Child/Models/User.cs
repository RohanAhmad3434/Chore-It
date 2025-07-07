using System.ComponentModel.DataAnnotations;

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
        public string Role { get; set; } = "Child"; // Parent or Child

        public bool IsGoogleUser { get; set; } = false;

        public DateTime DateOfBirth { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }


        // ✅ Many-to-Many navigation properties
        public List<ParentChild>? Parents { get; set; } // This user as Child has multiple Parents
        public List<ParentChild>? Children { get; set; } // This user as Parent has multiple Children

        public List<TaskItem>? Tasks { get; set; }
    }
}

