using System.ComponentModel.DataAnnotations;

namespace Parent_Child.DTOs
{
    public class RegisterDto
    {
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; } = "Child";

        public bool IsGoogleUser { get; set; } = false;
    }

}
