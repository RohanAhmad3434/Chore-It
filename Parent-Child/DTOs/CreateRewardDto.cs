using System.ComponentModel.DataAnnotations;

namespace Parent_Child.DTOs
{
    public class CreateRewardDto
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
    }

}
