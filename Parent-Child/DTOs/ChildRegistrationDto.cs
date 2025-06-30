using System.ComponentModel.DataAnnotations;

namespace Parent_Child.DTOs
{
    public class ChildRegistrationDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string Relation { get; set; }
    }

}
