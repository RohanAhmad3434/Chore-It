//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;


//namespace Parent_Child.Models
//{
//    public class Reward
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; }

//        [Required]
//        public string Title { get; set; }

//        public string? Description { get; set; }

//        // ✅ Assigned to child user
//        public int? AssignedToId { get; set; } 

//        [ForeignKey("AssignedToId")]
//        public User? AssignedTo { get; set; }

//        public bool IsRedeemed { get; set; } = false;

//        public DateTime? RedeemedOn { get; set; }
//    }


//}



using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parent_Child.Models
{
    public class Reward
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

    }
}
