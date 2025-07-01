using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Parent_Child.DTOs
{
    public class CompleteTaskDto
    {
        [Required]
        public IFormFile PhotoFile { get; set; }
    }
}
