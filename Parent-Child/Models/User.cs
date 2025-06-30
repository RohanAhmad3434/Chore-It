
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Parent_Child.Models;


//public class User
//{
//    public int Id { get; set; }

//    [Required]
//    public string FullName { get; set; }

//    [Required]
//    [EmailAddress]
//    public string Email { get; set; }

//    [Required]
//    public string PasswordHash { get; set; }

//    [Required]
//    public string Role { get; set; } = "Child"; // Default to Child

//    public bool IsGoogleUser { get; set; } = false;

//    // ✅ Parent-child relationship
//    public int? ParentId { get; set; }        // FK to parent User Id
//    public User? Parent { get; set; }         // Navigation property

//    public List<User>? Children { get; set; } // For parent to access their children

//    public List<TaskItem>? Tasks { get; set; }
//}



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
    public string Relation { get; set; }

    public List<TaskItem>? Tasks { get; set; }
    public List<Achievement>? Achievements { get; set; } // NEW: link achievements
}
