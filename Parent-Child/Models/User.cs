//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//namespace Parent_Child.Models
//{
//    public class User
//    {
//        public int Id { get; set; }

//        [Required]
//        public string FullName { get; set; }

//        [Required, EmailAddress]
//        public string Email { get; set; }

//        [Required]
//        public string PasswordHash { get; set; }

//        [Required]
//        public string Role { get; set; } = "Child"; // Parent or Child

//        public bool IsGoogleUser { get; set; } = false;

//        public int? ParentId { get; set; }
//        public User? Parent { get; set; }
//        public List<User>? Children { get; set; }

//        public DateTime DateOfBirth { get; set; }
//        public string? Relation { get; set; } // Father or Mother etc.

//        public List<TaskItem>? Tasks { get; set; }

//    }

//}

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

        // ✅ Many-to-Many navigation properties
        public List<ParentChild>? Parents { get; set; } // This user as Child has multiple Parents
        public List<ParentChild>? Children { get; set; } // This user as Parent has multiple Children

        public List<TaskItem>? Tasks { get; set; }
    }
}

